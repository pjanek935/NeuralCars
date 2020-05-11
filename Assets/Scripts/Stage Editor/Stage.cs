using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] StageEditor stageEditor;

    [Range (0, 3.5f)]
    [SerializeField] float bezierDistanceFactor = 0.25f;

    [SerializeField] GameObject wallPrefab;

    List<GameObject> wallsRight = new List<GameObject> ();
    List<GameObject> wallsLeft = new List<GameObject> ();

    StageModel stageModel = new StageModel ();

    // Start is called before the first frame update
    void OnEnable()
    {
        refresh ();

        if (stageEditor != null)
        {
            stageEditor.OnFlagsRefreshed += refresh;
        }
    }

    private void OnDisable ()
    {
        if (stageEditor != null)
        {
            stageEditor.OnFlagsRefreshed -= refresh;
        }
    }

    private void OnValidate ()
    {
        if (Application.isPlaying)
        {
            refresh ();
        }
    }

    void refresh ()
    {
        List<StageNode> nodes = stageEditor.GetNodes ();
        stageModel.SetNodes (nodes, bezierDistanceFactor);
        createWalls ();
    }

    void createWalls (List <Vector3> points, List <GameObject> walls)
    {
        if (points != null && points.Count > 1)
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

                if (Mathf.Abs (d - 1f) < StageConsts.Epsilon)
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

    void createWalls ()
    {
        createWalls (stageModel.PointsRight, wallsRight);
        createWalls (stageModel.PointsLeft, wallsLeft);
    }
}
