using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Vector3 Start = Vector3.zero;
    public Vector3 End = Vector3.zero;
    public float StartdWidth = 0.5f;
    public float EndWidth = 0.5f;

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

    // Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
    // intersect the intersection point may be stored in the floats i_x and i_y.
    public static bool GetLineSegmentIntersectionPoint (Vector3 p0, Vector3 p1,
        Vector3 p2, Vector3 p3, out Vector3 intersectionPoint)
    {
        bool result = false;
        intersectionPoint = new Vector3 ();

        Vector3 s1, s2;
        s1.x = p1.x - p0.x;
        s1.z = p1.z - p0.z;
        s2.x = p3.x - p2.x;
        s2.z = p3.z - p2.z;

        float s, t;

        s = (-s1.z * (p0.x - p2.x) + s1.x * (p0.z - p2.z)) / (-s2.x * s1.z + s1.x * s2.z);
        t = (s2.x * (p0.z - p2.z) - s2.z * (p0.x - p2.x)) / (-s2.x * s1.z + s1.x * s2.z);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        { // Collision detected
            intersectionPoint.x = p0.x + (t * s1.x);
            intersectionPoint.z = p0.z + (t * s1.z);

            result = true;
        }

        return result;
    }
}

public class Stage : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] StageEditor stageEditor;

    [Range (0, 2.5f)]
    [SerializeField] float bezierDistanceFactor = 0.25f;

    List<Line> lines = new List<Line> ();
    List<Vector3> pointsRight = new List<Vector3> ();
    List<Vector3> pointsLeft = new List<Vector3> ();
    const float epsilon = 0.01f;
    [SerializeField] GameObject wallPrefab;

    List<GameObject> wallsRight = new List<GameObject> ();
    List<GameObject> wallsLeft = new List<GameObject> ();

    List<Vector3> curvedPoints = new List<Vector3> ();

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
        List<Flag> flags = stageEditor.Flags;

        for (int i = 0; i < flags.Count - 1; i ++)
        {
            Line l = new Line (flags [i].transform.position, flags [i+1].transform.position);
            l.StartdWidth = flags [i].Width;
            l.EndWidth = flags [i + 1].Width;
            lines.Add (l);
        }

        init ();
        refreshWalls ();
    }

    void init ()
    {
        pointsRight.Clear ();
        pointsLeft.Clear ();

        for (int i = 0; i < lines.Count; i ++)
        {
            Vector3 direction = lines [i].End - lines [i].Start;
            direction.Normalize ();

            Vector3 perpenRightDirection = lines [i].PerpendicularCounterClockwise ();
            Vector3 perpenLeftDirection = lines [i].PerpendicularClockwise ();
            perpenRightDirection.Normalize ();
            perpenLeftDirection.Normalize ();

            pointsRight.Add (lines [i].Start + perpenRightDirection * lines [i].StartdWidth);
            pointsLeft.Add (lines [i].Start + perpenLeftDirection * lines [i].StartdWidth);

            pointsRight.Add (lines [i].End + perpenRightDirection * lines [i].EndWidth);
            pointsLeft.Add (lines [i].End + perpenLeftDirection * lines [i].EndWidth);
        }

        List<Vector3> tmp = new List<Vector3> ();

        if (pointsRight.Count >= 4)
        {
            for (int i = 0; i < pointsRight.Count - 3; i += 2)
            {
                Vector3 p0 = pointsRight [i];
                Vector3 p1 = pointsRight [i + 1];
                Vector3 p2 = pointsRight [i + 2];
                Vector3 p3 = pointsRight [i + 3];

                Vector3 result = Vector3.zero;

                if (Line.GetLineSegmentIntersectionPoint (p0, p1, p2, p3, out result))
                {
                    tmp.Add (p0);
                    tmp.Add (result);

                    pointsRight [i + 1] = result;
                    pointsRight [i + 2] = result;
                }
                else
                {
                    tmp.Add (p0);
                    tmp.Add (p1);
                }
            }

            tmp.Add (pointsRight [pointsRight.Count - 2]);
            tmp.Add (pointsRight [pointsRight.Count - 1]);
        }

        pointsRight.Clear ();
        tmp.ForEach (p => pointsRight.Add (p));
        pointsRight = interpolate (pointsRight);
        tmp.Clear ();

        if (pointsLeft.Count >= 4)
        {
            for (int i = 0; i < pointsLeft.Count - 3; i += 2)
            {
                Vector3 p0 = pointsLeft [i];
                Vector3 p1 = pointsLeft [i + 1];
                Vector3 p2 = pointsLeft [i + 2];
                Vector3 p3 = pointsLeft [i + 3];

                Vector3 result = Vector3.zero;

                if (Line.GetLineSegmentIntersectionPoint (p0, p1, p2, p3, out result))
                {
                    tmp.Add (p0);
                    tmp.Add (result);

                    pointsLeft [i + 1] = result;
                    pointsLeft [i + 2] = result;
                }
                else
                {
                    tmp.Add (p0);
                    tmp.Add (p1);
                }
            }

            tmp.Add (pointsLeft [pointsLeft.Count - 2]);
            tmp.Add (pointsLeft [pointsLeft.Count - 1]);
        }

        pointsLeft.Clear ();
        tmp.ForEach (p => pointsLeft.Add (p));
        pointsLeft = interpolate (pointsLeft);

        if (pointsRight.Count >= 4)
        {
            curvedPoints = interpolate (pointsRight);
            //curvedPoints = interpolate (curvedPoints);
        }
    }

    List <Vector3> interpolate (List <Vector3> points)
    {
        List<Vector3> result = new List<Vector3> ();

        if (points != null && points.Count >= 4)
        {
            Vector3 prevDirection = points [1] - points [0]; ;
            prevDirection.Normalize ();
            result.Add (points [0]);

            for (int i = 1; i < points.Count - 2; i ++)
            {
                Vector3 p0 = points [i - 1];
                Vector3 p1 = points [i];
                Vector3 p2 = points [i + 1];
                Vector3 p3 = points [i + 2];

                Vector3 direction = p2 - p1;
                direction.Normalize ();
                Vector3 nextDirection = p3 - p2;
                nextDirection.Normalize ();

                float a1 = Mathf.Abs (Vector3.Angle (prevDirection, direction));
                float a2 = Mathf.Abs (Vector3.Angle (direction, nextDirection));
                float d1 = Vector3.Dot (prevDirection, direction);
                float d2 = Vector3.Dot (prevDirection, nextDirection);
                float dist = Vector3.Distance (p1, p2);

                if (Mathf.Abs (d1 - 1f) >= epsilon && Mathf.Abs (d2 - 1f) >= epsilon)
                {
                    result.Add (points [i]);

                    if (dist >= 0.2 && dist <= 10f)
                    {
                        Vector3 b_p0 = p1;
                        Vector3 b_p1 = p1 + prevDirection * bezierDistanceFactor * (1f - d2) * dist * 0.2f;
                        Vector3 b_p2 = p2 - nextDirection * bezierDistanceFactor * (1f - d2)*dist * 0.2f;
                        Vector3 b_p3 = p2;

                        for (float j = 0.1f; j < 1f; j += 0.1f)
                        {
                            Vector3 newPoint = Curver.cubeBezier3 (b_p0, b_p1, b_p2, b_p3, j);
                            result.Add (newPoint);
                        }
                    }
                    

                    result.Add (points [i + 1]);
                    i++;
                    prevDirection = nextDirection;
                }
                else
                {
                    result.Add (points [i]);
                }

                //float d = Vector3.Dot (direction, prevDirection);

                //if (Mathf.Abs (d - 1f) < epsilon)
                //{
                //    result.Add (points [i]);
                //}
                //else
                //{
                //    result.Add (points [i]);
                //    float dist = Vector3.Distance (points [i], points [i + 1]);

                //    if (/*dist < 1f || dist > 15f*/false)
                //    {
                //        result.Add (points [i]);
                //    }
                //    else
                //    {
                //        Debug.Log ("Angle: " + Mathf.Abs (Vector3.Angle (prevDirection, direction)));
                //        Debug.Log ("Distance: " + dist);
                //        Debug.Log ("");

                //        Vector3 p0 = points [i];
                //        Vector3 p1 = p0 + prevDirection * d * bezierDistanceFactor;
                //        Vector3 p3 = points [i + 1];
                //        Vector3 p2 = p3;

                //        for (float j = 0.1f; j < 1f; j += 0.1f)
                //        {
                //            Vector3 newPoint = Curver.cubeBezier3 (p0, p1, p2, p3, j);
                //            result.Add (newPoint);
                //        }

                //        result.Add (points [i + 1]);
                //        i++;
                //    }

                //    prevDirection = direction;
                //}
            }

            for (int i = points.Count -2 ; i < points.Count; i++)
            {
                result.Add (points [i]);
            }

        }
        else if (points != null)
        {
            for (int i = 0; i < points.Count; i ++)
            {
                result.Add (points [i]);
            }
        }

        return result;
    }

    void render ()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Debug.DrawLine (lines [i].Start, lines [i].End, Color.red, 0.01f);
        }

        for (int i = 0; i < pointsRight.Count - 1; i++)
        {
            Debug.DrawLine (pointsRight [i], pointsRight [i + 1], Color.blue, 0.01f);
        }

        for (int i = 0; i < pointsLeft.Count - 1; i++)
        {
            Debug.DrawLine (pointsLeft [i], pointsLeft [i + 1], Color.yellow, 0.01f);
        }

        for (int i = 0; i < curvedPoints.Count - 1; i++)
        {
            Debug.DrawLine (curvedPoints [i] + new Vector3 (0, 5, 0), curvedPoints [i + 1] + new Vector3 (0, 5, 0), Color.green, 0.01f);
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
