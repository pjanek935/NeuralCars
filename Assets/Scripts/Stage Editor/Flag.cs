using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public delegate void OnFlagEventHandler (Flag flag);
    public event OnFlagEventHandler OnFlagMoved;

    [SerializeField] float width = 5f;
    [SerializeField] new Camera camera;
    [SerializeField] CameraController cameraController;
    [SerializeField] Animation animation;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] StageEditor stageEditor;

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
        }
    }

    private void OnMouseDown ()
    {
        if (cameraController != null && ! cameraController.IsPointerOverGUI ())
        {
            Selected = true;
            OnFlagMoved?.Invoke (this);
        }
    }

    private void OnMouseUp ()
    {
        Selected = false;
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
            Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask ("Floor");

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

                this.transform.position = newPos;

                OnFlagMoved?.Invoke (this);
            }

            if (Input.GetMouseButtonUp (0))
            {
                Selected = false;
            }
        }
    }
}
