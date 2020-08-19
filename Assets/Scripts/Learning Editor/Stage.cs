using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] MeshFilter roadMeshFilter;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject gatePrefab;

    [SerializeField] Transform wallsContainer;
    [SerializeField] Transform gatesContainer;

    [SerializeField] Transform trackBeginning;
    [SerializeField] GrassCreator grassCreator;

    List<GameObject> wallsRight = new List<GameObject> ();
    List<GameObject> wallsLeft = new List<GameObject> ();
    List<Gate> gates = new List<Gate> ();
    StageModel stageModel = new StageModel ();

    public StageModel StageModel
    {
        get { return stageModel; }
    }

    public int GetLastGateIndex ()
    {
        return gates [gates.Count - 1].Index;
    }

    public List <StageNode> GetStageNodes ()
    {
        return stageModel.Nodes;
    }

    public float GetDistanceFromBeginning (Vector3 pos)
    {
        return stageModel.GetDistanceFromBeginning (pos);
    }

    public void SetBeizerCurverFactor (float val)
    {
        stageModel.BezierCurveFactor = val;
    }
    public void SynchronizeModelWithFlags (List<StageNode> nodes)
    {
        stageModel.SetNodes (nodes);
        RefreshGeometry ();
    }

    public void MakeAndAddAction (StageAction stageAction)
    {
        stageModel.MakeAndAddAction (stageAction);
    }

    public void AddAction (StageAction stageAction)
    {
        stageModel.AddAction (stageAction);
    }

    public bool CanUndoLastAction ()
    {
        return stageModel.CanUndoLastAction ();
    }

    public bool CanMakeStepForward ()
    {
        return stageModel.CanMakeStepForward ();
    }

    public void LoadStageWithId (int stageId)
    {
        float bezierCurveFactor = 3.5f;

        if (stageModel != null)
        {
            bezierCurveFactor = stageModel.BezierCurveFactor;
        }

        StageModel newStageModel = SaveManager.Instance.LoadStage (stageId);

        if (newStageModel != null)
        {
            stageModel = newStageModel;
        }
        else
        {
            stageModel = new StageModel ();
        }

        stageModel.BezierCurveFactor = bezierCurveFactor;
        stageModel.RefreshPointsRightAndLeft ();
        RefreshGeometry ();
    }

    public void LoadStage (StageModel newStageModel)
    {
        if (newStageModel == null)
        {
            return;
        }

        float bezierCurveFactor = 3.5f;

        if (stageModel != null)
        {
            bezierCurveFactor = stageModel.BezierCurveFactor;
        }

        stageModel = newStageModel;
        stageModel.BezierCurveFactor = bezierCurveFactor;
        stageModel.RefreshPointsRightAndLeft ();
        RefreshGeometry ();
    }

    public void SaveCurrentStage ()
    {
        SaveManager.Instance.SaveStage (stageModel, SaveManager.Instance.CurrentOpenedStageId);
    }

    public void ClearStage ()
    {
        stageModel.SetNodes (new List<StageNode> ());
        createWalls ();
    }

    public bool UndoLastAction ()
    {
        bool result = false;

        if (stageModel.CanUndoLastAction ())
        {
            result = true;
            stageModel.UndoLastAction ();
        }

        return result;
    }

    public bool MakeStepForward ()
    {
        bool result = false;

        if (stageModel.CanMakeStepForward ())
        {
            result = true;
            stageModel.MakeStepForward ();
        }

        return result;
    }

    public void RefreshGeometry ()
    {
        createWalls ();
        createRoadMesh ();
        refreshTrackBeginningPosition ();
    }

    public void PrepareStage ()
    {
        createGates ();
        grassCreator.DisableGrassOnRoad ();
        grassCreator.OnCameraZoomUpdated ();
    }

    void createWalls ()
    {
        createWalls (stageModel.PointsRight, wallsRight);
        createWalls (stageModel.PointsLeft, wallsLeft);
        createGates ();
    }

    void createWalls (List<Vector3> points, List<GameObject> walls)
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

                if (Mathf.Abs (d - 1f) >= GlobalConst.EPSILON)
                {
                    if (helpIndex >= walls.Count)
                    {
                        GameObject newObject = Instantiate (wallPrefab);
                        newObject.SetActive (true);
                        newObject.transform.SetParent (wallsContainer, true);
                        newObject.transform.localScale = Vector3.one;
                        walls.Add (newObject);
                    }

                    float dist = Vector3.Distance (startPoint, points [i - 1]);
                    walls [helpIndex].transform.localPosition = startPoint;
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
                newObject.SetActive (true);
                newObject.transform.SetParent (wallsContainer, true);
                newObject.transform.localScale = Vector3.one;
                walls.Add (newObject);
            }

            float dist2 = Vector3.Distance (startPoint, points [points.Count - 1]);
            walls [helpIndex].transform.localPosition = startPoint;
            walls [helpIndex].transform.LookAt (points [points.Count - 1], Vector3.up);
            Vector3 scale2 = walls [helpIndex].transform.localScale;
            scale2.z = dist2;
            walls [helpIndex].transform.localScale = scale2;
            helpIndex++;

            if (walls.Count > helpIndex)
            {
                int diff = walls.Count - helpIndex;

                for (int i = 0; i < diff; i++)
                {
                    GameObject tmp = walls [walls.Count - 1].gameObject;
                    walls.RemoveAt (walls.Count - 1);
                    Destroy (tmp);
                }
            }
        }
        else
        {
            for (int i = 0; i < wallsLeft.Count; i++)
            {
                Destroy (wallsLeft [i].gameObject);
            }

            for (int i = 0; i < wallsRight.Count; i++)
            {
                Destroy (wallsRight [i].gameObject);
            }

            wallsRight.Clear ();
            wallsLeft.Clear ();
        }
    }

    void createGates ()
    {
        List<StageNode> nodes = stageModel.Nodes;
        List<Vector3> positions = new List<Vector3> ();
        List<Vector3> forwards = new List<Vector3> ();
        List<float> widths = new List<float> ();
        const float d = 2f;

        for (int i = 0; i < nodes.Count - 1; i++)
        {
            Vector3 dir = nodes [i + 1].Position - nodes [i].Position;
            float mag = dir.magnitude;
            dir.Normalize ();
            Vector3 pos = nodes [i].Position;

            for (float j = d; j < mag - nodes [i + 1].Width; j += d)
            {
                float lengthNormalized = j / mag;
                float width = Mathf.Lerp (nodes [i].Width, nodes [i + 1].Width, lengthNormalized);
                pos += dir * d;

                forwards.Add (dir);
                positions.Add (pos);
                widths.Add (width);
            }
        }

        for (int i = 0; i < positions.Count; i++)
        {
            if (i >= gates.Count)
            {
                GameObject newObject = Instantiate (gatePrefab);
                newObject.SetActive (true);
                newObject.transform.SetParent (gatesContainer, false);
                gates.Add (newObject.GetComponent<Gate> ());
            }

            gates [i].transform.localPosition = positions [i];
            gates [i].transform.forward = forwards [i];
            Vector3 scale = gates [i].transform.localScale;
            gates [i].transform.localScale = new Vector3 (widths [i] * 2f, scale.y, scale.z);
            gates [i].Index = i + 1;
        }

        int diff = gates.Count - positions.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                GameObject tmp = gates [gates.Count - 1].gameObject;
                gates.RemoveAt (gates.Count - 1);
                Destroy (tmp);
            }
        }
    }

    void createRoadMesh ()
    {
        List<Vector3> points = new List<Vector3> ();
        List<Vector3> pointsLeft = stageModel.PointsLeft;
        points.AddRange (pointsLeft);
        List<Vector3> pointsRight = stageModel.PointsRight;
        List<Vector3> pointsRightReversed = new List<Vector3> ();

        for (int i = pointsRight.Count - 1; i >= 0; i--)
        {
            pointsRightReversed.Add (pointsRight [i]);
        }

        points.AddRange (pointsRightReversed);
        Vector2 [] vertices2D = new Vector2 [points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            vertices2D [i] = new Vector2 (points [i].x, points [i].z);
        }

        Triangulator tr = new Triangulator (vertices2D);
        int [] indices = tr.Triangulate ();

        // Create the Vector3 vertices
        Vector3 [] vertices = new Vector3 [vertices2D.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices [i] = new Vector3 (vertices2D [i].x, 0, vertices2D [i].y);
        }

        // Create the mesh
        Mesh msh = new Mesh ();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals ();
        msh.RecalculateBounds ();

        roadMeshFilter.mesh = msh;
        meshCollider.sharedMesh = msh;
    }

    void refreshTrackBeginningPosition ()
    {
        List<StageNode> nodes = stageModel.Nodes;

        if (nodes.Count > 0)
        {
            Vector3 pos = nodes [0].Position;
            Vector3 forward = Vector3.forward;

            if (nodes.Count > 1)
            {
                forward = nodes [1].Position - pos;
            }

            trackBeginning.position = pos;
            trackBeginning.forward = forward;
        }
    }
}
