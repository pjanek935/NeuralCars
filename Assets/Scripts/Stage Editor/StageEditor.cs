using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditor : MonoBehaviour
{
    [SerializeField] Stage stage;
    [SerializeField] TimelinePanel timelinePanel;
    [SerializeField] TopPanelController topPanelController;
    [SerializeField] FlagEditor flagEditor;

    [SerializeField] StageFloor stageFloor;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject flagPrefab;
    [SerializeField] Transform flagsContainer;

    [Range (0, 3.5f)] [SerializeField] float bezierDistanceFactor = 0.25f;
    [SerializeField] bool snapToGrid = false;
    [SerializeField] float gridCellSize = 0.5f;
    [SerializeField] float defaultWidth = 8f;

    List<Flag> flags = new List<Flag> ();
    bool editorEnabled = true;

    public Flag CurrentSelectedFlag
    {
        get;
        private set;
    }

    public List<Flag> Flags
    {
        get
        {
            return flags;
        }
    }

    public bool SnapToGrid
    {
        get { return snapToGrid; }
    }

    public float GridCellSize
    {
        get { return gridCellSize; }
    }

    public float DefaultWidth
    {
        get { return defaultWidth; }
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
            topPanelController.OnResetClicked += onResetStageClicked;
            topPanelController.OnClearClicked += onClearStageClicked;
            topPanelController.OnSnapToGridToggleClicked += onSnapToGridClicked;
            topPanelController.OnDefaultWidthDownClicked += onDefaultWidthDownClicked;
            topPanelController.OnDefaultWidthUpClicked += onDefaultWidthUpClicked;
        }

        onLoadStageClicked (SaveManager.Instance.CurrentOpenedStageId);
    }

    public void DisableStageEditor ()
    {
        editorEnabled = false;
        flagsContainer.gameObject.SetActive (false);
        lineRenderer.gameObject.SetActive (false);
    }

    public void EnableStageEditor ()
    {
        editorEnabled = true;
        flagsContainer.gameObject.SetActive (true);
        lineRenderer.gameObject.SetActive (true);
    }

    void onDefaultWidthDownClicked ()
    {
        float newWidth = defaultWidth - GlobalConst.NODE_D;
        newWidth = Mathf.Clamp (newWidth, GlobalConst.MIN_NODE_WIDTH, GlobalConst.MAX_NODE_WIDTH);
        defaultWidth = newWidth;

        refreshViews ();
    }

    void onDefaultWidthUpClicked ()
    {
        float newWidth = defaultWidth + GlobalConst.NODE_D;
        newWidth = Mathf.Clamp (newWidth, GlobalConst.MIN_NODE_WIDTH, GlobalConst.MAX_NODE_WIDTH);
        defaultWidth = newWidth;

        refreshViews ();
    }

    void onResetStageClicked ()
    {
        onLoadStageClicked (SaveManager.Instance.CurrentOpenedStageId);
    }

    void onSnapToGridClicked (bool value)
    {
        snapToGrid = value;
    }

    void onClearStageClicked ()
    {
        stage.ClearStage ();
        synchornizeFlagsWithModel ();
        refreshViews ();
    }

    void onLoadStageClicked (int stageId)
    {
        stage.SetBeizerCurverFactor (bezierDistanceFactor);
        stage.LoadStageWithId (stageId);
        synchornizeFlagsWithModel ();
        refreshViews ();
    }

    void onSaveStageClicked ()
    {
        stage.SaveCurrentStage ();
    }

    private void OnValidate ()
    {
        if (Application.isPlaying)
        {
            stage.SetBeizerCurverFactor (bezierDistanceFactor);
            synchronizeModelWithFlags ();
            refreshViews ();
        }
    }

    private void OnEnable ()
    {
        stage.SetBeizerCurverFactor (bezierDistanceFactor);
    }

    void refreshViews ()
    {
        timelinePanel.Refresh (stage);
        topPanelController.Refresh (stage);
    }

    void onBackClicked ()
    {
        if (stage.UndoLastAction ())
        {
            synchornizeFlagsWithModel ();
        }
    }

    void onForwardClicked ()
    {
        if (stage.MakeStepForward ())
        {
            synchornizeFlagsWithModel ();
        }
    }

    void onFloorClicked (Vector3 pos)
    {
        if (editorEnabled)
        {
            createNewFlag (pos);
            refreshLineRenderer ();
        }
    }

    void synchronizeModelWithFlags ()
    {
        List<StageNode> nodes = GetNodesFromFlags ();
        stage.SetBeizerCurverFactor (bezierDistanceFactor);
        stage.SynchronizeModelWithFlags (nodes);
        refreshViews ();
    }

    void synchornizeFlagsWithModel ()
    {
        List<StageNode> nodes = stage.GetStageNodes ();

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i >= flags.Count)
            {
                Flag flag = createNewFlagGameObject ();
                flags.Add (flag);
            }

            flags [i].transform.localPosition = nodes [i].Position;
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

        refreshViews ();
        refreshLineRenderer ();
    }

    void flagEditorOnDeleteClicked ()
    {
        if (CurrentSelectedFlag != null)
        {
            Flags.Remove (CurrentSelectedFlag);
            Destroy (CurrentSelectedFlag.gameObject);
            setNewCurrentFlag (null);
            refreshLineRenderer ();
            synchronizeModelWithFlags ();
            refreshViews ();
        }
    }

    void flagEditorOnWidthDownClicked ()
    {
        if (CurrentSelectedFlag != null)
        {
            float newWidth = CurrentSelectedFlag.Width - GlobalConst.NODE_D;

            if (newWidth < GlobalConst.MIN_NODE_WIDTH)
            {
                newWidth = GlobalConst.MIN_NODE_WIDTH;
            }

            if (Mathf.Abs (CurrentSelectedFlag.Width - newWidth) > GlobalConst.EPSILON)
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
            float newWidth = CurrentSelectedFlag.Width + GlobalConst.NODE_D;

            if (newWidth > GlobalConst.MAX_NODE_WIDTH)
            {
                newWidth = GlobalConst.MAX_NODE_WIDTH;
            }

            if (Mathf.Abs (CurrentSelectedFlag.Width - newWidth) > GlobalConst.EPSILON)
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
        stage.MakeAndAddAction (stageAction);
        refreshViews ();
        stage.RefreshGeometry ();
    }

    public Vector3 [] GetFlagsPositions ()
    {
        Vector3 [] pos = new Vector3 [flags.Count];

        for (int i = 0; i < flags.Count; i++)
        {
            pos [i] = flags [i].transform.localPosition;
        }

        return pos;
    }

    public List <StageNode> GetNodesFromFlags ()
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
        flag.OnFlagReleased += onFlagReleased;

        return flag;
    }

    void createNewFlag (Vector3 pos)
    {
        if (flagPrefab == null)
        {
            return;
        }

        if (snapToGrid)
        {
            pos.x = pos.x / gridCellSize;
            pos.x = (float) (gridCellSize * (int) pos.x);
            pos.z = pos.z / gridCellSize;
            pos.z = (float) (gridCellSize * (int) pos.z);
        }

        Flag flag = createNewFlagGameObject ();
        flag.gameObject.transform.localPosition = pos;

        if (flag != null)
        {
            float dist;
            int startPointIndex = findClosestLineSegment (pos, out dist);

            if (startPointIndex != -1)
            {
                if (dist > 4f) //MAGIC NUMBER TODO
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
                    Vector3 endPoint = flags [flags.Count - 1].transform.localPosition;
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
            
            flag.OnAddedByUser ();
            setNewCurrentFlag (flag);
            StageAction stageAction = new CreateNodeAction (startPointIndex, pos, flag.Width);
            stage.MakeAndAddAction (stageAction);
            refreshViews ();
            stage.RefreshGeometry ();
        }
    }

    void onFlagMoved (Flag flag)
    {
        setNewCurrentFlag (flag);
        refreshLineRenderer ();
        refreshViews ();
    }

    void onFlagReleased (Flag flag)
    {
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
                Vector3 p1 = flags [i-1].transform.localPosition;
                Vector3 p2 = flags [i].transform.localPosition;

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
