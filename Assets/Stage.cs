using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Vector3 Start = Vector3.zero;
    public Vector3 End = Vector3.zero;
    public float width = 0.5f;

    public Line (Vector3 start, Vector3 end)
    {
        this.Start = start;
        this.End = end;
    }

    public Vector3 PerpendicularClockwise ()
    {
        Vector3 direction = End - Start;
        return new Vector3 (direction.z, 0, -direction.x);
    }

    public Vector3 PerpendicularCounterClockwise ()
    {
        Vector3 direction = End - Start;
        return new Vector3 (-direction.z, 0, direction.x);
    }

    public Vector3 FindClosestPointOnLineSegment (Vector3 point)
    {
        Vector3 line = End - Start;
        Vector3 dir = point - Start;
        float d = Vector3.Dot (line, dir) / line.sqrMagnitude;
        d = Mathf.Clamp01 (d);
        return Vector3.Lerp (Start, End, d);
    }

    public float DistToLineSegment (Vector3 point)
    {
        Vector3 closestPoint = FindClosestPointOnLineSegment (point);
        return Vector3.Distance (point, closestPoint);
    }

    public float DistToVector (Vector3 p)
    {
        Vector3 d = (End - Start) / Vector3.Distance (End, Start);
        Vector3 v = p - Start;
        float t = Vector3.Dot (v, d);
        Vector3 P = Start + t * d;
        return Vector3.Distance (P, p);
    }
}
//[ExecuteInEditMode]
public class Stage : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] StageEditor stageEditor;

    List<Line> lines = new List<Line> ();
    List<Vector3> pointsRight = new List<Vector3> ();
    List<Vector3> pointsLeft = new List<Vector3> ();
    [SerializeField]  float width = 2f;
    const float epsilon = 0.01f;
    [SerializeField] GameObject wallPrefab;

    List<GameObject> wallsRight = new List<GameObject> ();
    List<GameObject> wallsLeft = new List<GameObject> ();

    Vector3 point = Vector3.zero;
    int index = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        init ();
        refreshWalls ();

        if (stageEditor != null)
        {
            stageEditor.OnFlagsRefreshed += createLineSegmentsFromFlags;
        }
    }

    private void OnDisable ()
    {
        if (stageEditor != null)
        {
            stageEditor.OnFlagsRefreshed -= createLineSegmentsFromFlags;
        }
    }

    private void OnValidate ()
    {
        if (Application.isPlaying)
        {
            init ();
            refreshWalls ();
        }
    }

    // Update is called once per frame
    void Update()
    {
        render ();
    }

    void createLineSegmentsFromFlags ()
    {
        lines.Clear ();
        Vector3 [] pos = stageEditor.GetFlagsPositions ();

        for (int i = 0; i < pos.Length - 1; i ++)
        {
            Line l = new Line (pos [i], pos [i+1]);
            lines.Add (l);
        }

        init ();
        refreshWalls ();
    }

    void init ()
    {
        //lines.Clear ();
        pointsRight.Clear ();
        pointsLeft.Clear ();

        //lines.Add (new Line (Vector3.zero, new Vector3 (5, 0, 5)));
        //lines.Add (new Line (new Vector3 (5, 0, 5), new Vector3 (-4, 0, 5)));
        //lines.Add (new Line (new Vector3 (-4, 0, 5), new Vector3 (-4, 0, 6)));
        //lines.Add (new Line (new Vector3 (-4, 0, 6), new Vector3 (-2, 0, 8)));

        for (int i = 0; i < lines.Count; i++)
        {
            Vector3 direction = lines [i].End - lines [i].Start;
            int length = (int) direction.magnitude;
            direction.Normalize ();
            direction *= 0.5f;
            length = (int) (length / 0.5f);
            Vector3 start = lines [i].Start;
            Vector3 perpenRight = lines [i].PerpendicularCounterClockwise ();
            Vector3 perpenLeft = lines [i].PerpendicularClockwise ();
            perpenRight.Normalize ();
            perpenRight *= width;
            perpenLeft.Normalize ();
            perpenLeft *= width;

            for (int j = 0; j < length + 1; j++)
            {
                pointsRight.Add (start + perpenRight);
                pointsLeft.Add (start + perpenLeft);

                start += direction;
            }
        }

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = pointsRight.Count - 1; j >= 0; j--)
            {
                float dist = lines [i].DistToLineSegment (pointsRight [j]);

                if (dist + epsilon < width)
                {
                    pointsRight.RemoveAt (j);
                }
            }

            for (int j = pointsLeft.Count - 1; j >= 0; j--)
            {
                float dist = lines [i].DistToLineSegment (pointsLeft [j]);

                if (dist + epsilon < width)
                {
                    pointsLeft.RemoveAt (j);
                }
            }
        }
    }

    void render ()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Debug.DrawLine (lines [i].Start, lines [i].End, Color.red, 1f);
        }

        for (int i = 0; i < pointsRight.Count; i++)
        {
            Debug.DrawLine (pointsRight [i], pointsRight [i] + new Vector3 (0.05f, 0f, 0.05f), Color.blue, 1f);
        }

        for (int i = 0; i < pointsLeft.Count; i++)
        {
            Debug.DrawLine (pointsLeft [i], pointsLeft [i] + new Vector3 (0.05f, 0f, 0.05f), Color.yellow, 1f);
        }
    }

    void createWalls (List <Vector3> points, List <GameObject> walls)
    {
        if (points.Count > 1)
        {
            Vector3 startPoint = points [0];
            Vector3 prevDirection = points [1] - startPoint;
            prevDirection.Normalize ();
            int helpIndex = 0;

            for (int i = 2; i < points.Count; i++)
            {
                Vector3 direction = points [i] - points [i - 1];
                direction.Normalize ();
                float d = Vector3.Dot (direction, prevDirection);

                if (Mathf.Abs (d - 1f) < epsilon)
                {

                }
                else
                {
                    if (helpIndex >= walls.Count)
                    {
                        GameObject newObject = Instantiate (wallPrefab);
                        walls.Add (newObject);
                    }

                    float dist = Vector3.Distance (startPoint, points [i - 1]);
                    walls [helpIndex].transform.position = startPoint;
                    walls [helpIndex].transform.LookAt (points [i - 1], Vector3.up);
                    Vector3 scale = walls [helpIndex].transform.localScale;
                    scale.z = dist;
                    walls [helpIndex].transform.localScale = scale;

                    helpIndex++;

                    startPoint = points [i - 1];
                    prevDirection = direction;
                }
            }

            if (helpIndex >= walls.Count)
            {
                GameObject newObject = Instantiate (wallPrefab);
                walls.Add (newObject);
            }

            float dist2 = Vector3.Distance (startPoint, points [points.Count - 1]);
            walls [helpIndex].transform.position = startPoint;
            walls [helpIndex].transform.LookAt (points [points.Count - 1], Vector3.up);
            Vector3 scale2 = walls [helpIndex].transform.localScale;
            scale2.z = dist2;
            walls [helpIndex].transform.localScale = scale2;
            helpIndex++;

            if (walls.Count > helpIndex)
            {
                int diff = walls.Count - helpIndex;

                for (int i = 0; i < diff; i ++)
                {
                    GameObject tmp = walls [walls.Count - 1].gameObject;
                    walls.RemoveAt (walls.Count - 1);
                    Destroy (tmp);
                }
            }
        }
    }

    void refreshWalls ()
    {
        createWalls (pointsRight, wallsRight);
        createWalls (pointsLeft, wallsLeft);
    }
}
