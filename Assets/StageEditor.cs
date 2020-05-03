using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditor : MonoBehaviour
{
    public delegate void OnStageEditorEventHandler ();
    public event OnStageEditorEventHandler OnFlagsRefreshed;

    [SerializeField] new Camera camera;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject flagPrefab;

    List<Flag> flags = new List<Flag> ();

    public List <Flag> Flags
    {
        get
        {
            return flags;
        }
    }

    public Vector3 [] GetFlagsPositions ()
    {
        Vector3 [] pos = new Vector3 [flags.Count];

        for (int i = 0; i < flags.Count; i++)
        {
            pos [i] = flags [i].transform.position;
        }

        return pos;
    }

    private void OnMouseDown ()
    {
        Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask ("Floor");

        if (Physics.Raycast (raycast, out hit, 1000, layerMask))
        {
            Vector3 pos = hit.point;
            pos.y = 0;
            createNewFlag (pos);
            refreshLineRenderer ();
        }
    }

    void refreshLineRenderer ()
    {
        if (lineRenderer == null)
        {
            return;
        }

        Vector3 [] pos = GetFlagsPositions ();
        lineRenderer.positionCount = flags.Count;
        lineRenderer.SetPositions (pos);
    }

    void createNewFlag (Vector3 pos)
    {
        if (flagPrefab == null)
        {
            return;
        }

        GameObject newGameObject = Instantiate (flagPrefab);
        newGameObject.transform.SetParent (this.transform, false);
        newGameObject.transform.position = pos;
        newGameObject.SetActive (true);
        Flag flag = newGameObject.GetComponent<Flag> ();

        if (flag != null)
        {
            float dist;
            int startPointIndex = findClosestLineSegment (pos, out dist);

            if (startPointIndex != -1)
            {
                if (dist > 2f)
                {
                    startPointIndex = -1;
                }
            }
            
            if (startPointIndex == -1)
            {//New flag should be added at end or beggining
                bool addAtEnd = true;

                if (flags.Count > 1)
                {
                    Vector3 startPoint = flags [0].transform.position;
                    Vector3 endPoint = flags [flags.Count - 1].transform.position;
                    float startPointDist = Vector3.Distance (startPoint, pos);
                    float endPointDist = Vector3.Distance (endPoint, pos);

                    if (startPointDist < endPointDist)
                    {
                        addAtEnd = false;
                    }
                }

                if (addAtEnd)
                {
                    flags.Add (flag);
                }
                else
                {
                    flags.Insert (0, flag);
                }
            }
            else
            {//New flag should be inserted somewhere in between
                flags.Insert (startPointIndex, flag);
            }
            
            flag.OnFlagMoved += onFlagMoved;
            OnFlagsRefreshed?.Invoke ();
        }
    }

    void onFlagMoved (Flag flag)
    {
        refreshLineRenderer ();
        OnFlagsRefreshed?.Invoke ();
    }

    /// <summary>
    /// Returned int is an index of start point of a found line segment (-1 if nothing is found).
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    int findClosestLineSegment (Vector3 pos, out float distance)
    {
        distance = float.MaxValue;
        int result = -1;

        if (flags.Count >= 2)
        {
            for (int i = 1; i < flags.Count; i ++)
            {
                Vector3 p1 = flags [i-1].transform.position;
                Vector3 p2 = flags [i].transform.position;
                Line line = new Line (p1, p2);
                float d = line.DistToLineSegment (pos);

                if (d < distance)
                {
                    distance = d;
                    result = i;
                }
            }
        }

        return result;
    }
}
