using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public delegate void OnFlagEventHandler (Flag flag);
    public event OnFlagEventHandler OnFlagMoved;
    public event OnFlagEventHandler OnFlagReleased;

    [SerializeField] float width = 5f;
    [SerializeField] new Camera camera;
    [SerializeField] CameraController cameraController;
    [SerializeField] Animation animation;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] StageEditor stageEditor;

    Vector3 prevMousePos;

    public Vector3 PrevFlagPosition
    {
        get;
        set;
    }

    public float Width
    {
        get
        {
            return width;
        }
        
        set
        {
            width = value;
        }
    }

    public bool Selected
    {
        get;
        private set;
    }

    private void Start ()
    {
        if (stageEditor != null)
        {
            width = stageEditor.DefaultWidth;
            PrevFlagPosition = this.transform.position;
        }
    }

    private void OnMouseDown ()
    {
        if (cameraController != null && ! cameraController.IsPointerOverGUI ())
        {
            if (! Selected)
            {
                PrevFlagPosition = this.transform.position;
            }

            Selected = true;
            OnFlagMoved?.Invoke (this);
        }
    }

    private void OnMouseUp ()
    {
        Selected = false;
        OnFlagReleased?.Invoke (this);
    }

    public void OnAddedByUser ()
    {
        Selected = true;

        if (animation != null)
        {
            animation.Play ();
        }
    }

    public void OnAnimationFinished ()
    {
        if (particleSystem != null)
        {
            particleSystem.Play ();
        }
    }

    private void Update ()
    {
        if (Selected)
        {
            if (Input.GetMouseButtonUp (0) && Selected)
            {
                Selected = false;
                OnFlagReleased?.Invoke (this);
            }
            else if (Vector3.Distance (prevMousePos, Input.mousePosition) > GlobalConst.EPSILON)
            {
                Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                int layerMask = LayerMask.GetMask (GlobalConst.FLOOR_LAYER);

                if (Physics.Raycast (raycast, out hit, 1000, layerMask))
                {
                    Vector3 newPos = hit.point;
                    newPos.y = 0;

                    if (stageEditor.SnapToGrid)
                    {
                        float gridCellSize = stageEditor.GridCellSize;
                        newPos.x = newPos.x / gridCellSize;
                        newPos.x = (float) (gridCellSize * (int) newPos.x);
                        newPos.z = newPos.z / gridCellSize;
                        newPos.z = (float) (gridCellSize * (int) newPos.z);
                    }

                    if (Vector3.Distance (newPos, this.transform.localPosition) > GlobalConst.EPSILON)
                    {
                        if (Vector3.Distance (Vector3.zero, newPos) < GlobalConst.STAGE_RADIUS)
                        {
                            this.transform.localPosition = newPos;
                            OnFlagMoved?.Invoke (this);
                        }
                    }
                }
            }

            prevMousePos = Input.mousePosition;
        }
    }
}
