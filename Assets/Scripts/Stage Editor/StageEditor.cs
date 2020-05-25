using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditor : MonoBehaviour
{
    [SerializeField] StageFloor stageFloor;
    [SerializeField] FlagEditor flagEditor;
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] GameObject flagPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject gatePrefab;

    [SerializeField] TimelinePanel timelinePanel;
    [SerializeField] TopPanelController topPanelController;
    [Range (0, 3.5f)] [SerializeField] float bezierDistanceFactor = 0.25f;
    
    [SerializeField] Transform flagsContainer;
    [SerializeField] Transform wallsContainer;
    [SerializeField] Transform gatesContainer;

    List<GameObject> wallsRight = new List<GameObject> ();
    List<GameObject> wallsLeft = new List<GameObject> ();
    List<Gate> gates = new List<Gate> ();
    StageModel stageModel = new StageModel ();
    List<Flag> flags = new List<Flag> ();
    
    public Flag CurrentSelectedFlag
    {
        get;
        private set;
    }

    public List <Flag> Flags
    {
        get
        {
            return flags;
        }
    }

    private void Awake ()
    {
        if (flagEditor != null)
        {
            flagEditor.OnDeleteClicked += flagEditorOnDeleteClicked;
            flagEditor.OnWidthDownClicked += flagEditorOnWidthDownClicked;
            flagEditor.OnWidthUpClicked += flagEditorOnWidthUpClicked;
        }

        if (stageFloor != null)
        {
            stageFloor.OnFloorClicked += onFloorClicked;
        }

        if (timelinePanel != null)
        {
            timelinePanel.OnBackClicked += onBackClicked;
            timelinePanel.OnForwardClicked += onForwardClicked;
        }

        if (topPanelController != null)
        {
            topPanelController.OnLoadClicked += onLoadStageClicked;
            topPanelController.OnSaveClicked += onSaveStageClicked;
        }
    }

    void onLoadStageClicked (int stageId)
    {
        StageModel newStageModel = SaveManager.Instance.LoadStage (stageId);

        if (newStageModel != null)
        {
            stageModel = newStageModel;
            stageModel.RefreshPointsRightAndLeft (bezierDistanceFactor);
            synchornizeFlagsWithModel ();
        }
    }

    void onSaveStageClicked (int stageId)
    {
        SaveManager.Instance.SaveStage (stageModel, stageId);
    }

    private void OnValidate ()
    {
        if (Application.isPlaying)
        {
            stageModel.BezierCurveFactor = bezierDistanceFactor;
            synchronizeModelWithFlags ();
            timelinePanel.Refresh (stageModel);
        }
    }

    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            string json = StageModelToJson ();
            Debug.Log (json);
        }
    }

    public string StageModelToJson ()
    {
        string result = JsonUtility.ToJson (stageModel);

        return result;
    }

    void onBackClicked ()
    {
        if (stageModel.CanUndoLastAction ())
        {
            stageModel.UndoLastAction ();
            synchornizeFlagsWithModel ();
        }
    }

    void onForwardClicked ()
    {
        if (stageModel.CanMakeStepForward ())
        {
            stageModel.MakeStepForward ();
            synchornizeFlagsWithModel ();
        }
    }

    void onFloorClicked (Vector3 pos)
    {
        createNewFlag (pos);
        refreshLineRenderer ();
    }

    void synchronizeModelWithFlags ()
    {
        List<StageNode> nodes = GetNodes ();
        stageModel.SetNodes (nodes, bezierDistanceFactor);
        createWalls ();
        timelinePanel.Refresh (stageModel);
    }

    void synchornizeFlagsWithModel ()
    {
        List<StageNode> nodes = stageModel.Nodes;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i >= flags.Count)
            {
                Flag flag = createNewFlagGameObject ();
                flags.Add (flag);
            }

            flags [i].transform.position = nodes [i].Position;
            flags [i].Width = nodes [i].Width;
        }

        int diff = flags.Count - nodes.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                GameObject tmp = flags [flags.Count - 1].gameObject;
                flags.RemoveAt (flags.Count - 1);
                Destroy (tmp);
            }
        }

        createWalls ();
        timelinePanel.Refresh (stageModel);
        refreshLineRenderer ();
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

                if (Mathf.Abs (d - 1f) < StageConsts.Epsilon)
                {

                }
                else
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
                newObject.SetActive (true);
                newObject.transform.SetParent (wallsContainer, true);
                newObject.transform.localScale = Vector3.one;
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

                for (int i = 0; i < diff; i++)
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
        createGates ();
    }

    void flagEditorOnDeleteClicked ()
    {
        if (CurrentSelectedFlag != null)
        {
            Flags.Remove (CurrentSelectedFlag);
            Destroy (CurrentSelectedFlag.gameObject);
            onFlagMoved (null);
        }
    }

    void flagEditorOnWidthDownClicked ()
    {
        if (CurrentSelectedFlag != null)
        {
            float newWidth = CurrentSelectedFlag.Width - StageConsts.NodeD;

            if (newWidth < StageConsts.MinNodeWidth)
            {
                newWidth = StageConsts.MinNodeWidth;
            }

            if (Mathf.Abs (CurrentSelectedFlag.Width - newWidth) > StageConsts.Epsilon)
            {
                float from = CurrentSelectedFlag.Width;
                CurrentSelectedFlag.Width = newWidth;
                onFlagWidthChanged (CurrentSelectedFlag, from, newWidth);
            }
        }
    }

    void flagEditorOnWidthUpClicked ()
    {
        if (CurrentSelectedFlag != null)
        {
            float newWidth = CurrentSelectedFlag.Width + StageConsts.NodeD;

            if (newWidth > StageConsts.MaxNodeWidth)
            {
                newWidth = StageConsts.MaxNodeWidth;
            }

            if (Mathf.Abs (CurrentSelectedFlag.Width - newWidth) > StageConsts.Epsilon)
            {
                float from = CurrentSelectedFlag.Width;
                CurrentSelectedFlag.Width = newWidth;
                onFlagWidthChanged (CurrentSelectedFlag, from, newWidth);
            }
        }
    }

    void onFlagWidthChanged (Flag flag, float from, float to)
    {
        setNewCurrentFlag (flag);
        refreshLineRenderer ();
        StageAction stageAction = new ChangeWidthAction (flags.IndexOf (flag), from, to);
        stageModel.MakeAndAddAction (stageAction);
        timelinePanel.Refresh (stageModel);
        createWalls ();
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

    public List <StageNode> GetNodes ()
    {
        List<StageNode> nodes = new List<StageNode> ();
        flags.ForEach (f => nodes.Add (new StageNode (f.transform.position, f.Width)));

        return nodes;
    }

    void setNewCurrentFlag (Flag flag)
    {
        CurrentSelectedFlag = flag;

        if (flagEditor != null)
        {
            flagEditor.Setup (flag);
        }
    }

    void createGates ()
    {
        List<StageNode> nodes = stageModel.Nodes;
        List<Vector3> positions = new List<Vector3> ();
        List<Vector3> forwards = new List<Vector3> ();
        List<float> widths = new List<float> ();
        const float d = 2f;

        for (int i = 0; i < nodes.Count - 1; i ++)
        {
            Vector3 dir = nodes [i + 1].Position - nodes [i].Position;
            float mag = dir.magnitude;
            dir.Normalize ();
            Vector3 pos = nodes [i].Position;

            for (float j = d; j < mag - nodes [i+1].Width; j += d)
            {
                float lengthNormalized = j / mag;
                float width = Mathf.Lerp (nodes [i].Width, nodes [i + 1].Width, lengthNormalized);
                pos += dir * d;

                forwards.Add (dir);
                positions.Add (pos);
                widths.Add (width);
            }
        }

        for (int i = 0; i < positions.Count; i ++)
        {
            if (i >= gates.Count)
            {
                GameObject newObject = Instantiate (gatePrefab);
                newObject.SetActive (true);
                newObject.transform.SetParent (gatesContainer, false);
                gates.Add (newObject.GetComponent <Gate> ());
            }

            gates [i].transform.position = positions [i];
            gates [i].transform.forward = forwards [i];
            Vector3 scale = gates [i].transform.localScale;
            gates [i].transform.localScale = new Vector3 (widths [i] * 2f, scale.y, scale.z);
            gates [i].Index = i + 1;
        }

        int diff = gates.Count - positions.Count;
        
        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                GameObject tmp = gates [gates.Count - 1].gameObject;
                gates.RemoveAt (gates.Count - 1);
                Destroy (tmp);
            }
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

    Flag createNewFlagGameObject ()
    {
        GameObject newGameObject = Instantiate (flagPrefab);
        newGameObject.transform.SetParent (flagsContainer, false);
        newGameObject.SetActive (true);
        Flag flag = newGameObject.GetComponent<Flag> (); ;
        flag.OnFlagMoved += onFlagMoved;

        return flag;
    }

    void createNewFlag (Vector3 pos)
    {
        if (flagPrefab == null)
        {
            return;
        }

        Flag flag = createNewFlagGameObject ();
        flag.gameObject.transform.position = pos;

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
                    startPointIndex = -1;
                    flags.Add (flag);
                }
                else
                {
                    startPointIndex = 0;
                    flags.Insert (0, flag);
                }
            }
            else
            {//New flag should be inserted somewhere in between
                flags.Insert (startPointIndex, flag);
            }
            
            flag.OnFlagMoved += onFlagMoved;
            setNewCurrentFlag (flag);
            StageAction stageAction = new CreateNodeAction (startPointIndex, pos, flag.Width);
            stageModel.MakeAndAddAction (stageAction);
            timelinePanel.Refresh (stageModel);
            createWalls ();
        }
    }

    void onFlagMoved (Flag flag)
    {
        setNewCurrentFlag (flag);
        refreshLineRenderer ();
        synchronizeModelWithFlags ();
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

                float d = StageUtilities.DistToLineSegment (p1, p2, pos);

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
