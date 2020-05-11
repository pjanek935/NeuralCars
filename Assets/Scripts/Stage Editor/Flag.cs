using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public delegate void OnFlagEventHandler (Flag flag);
    public event OnFlagEventHandler OnFlagMoved;

    [SerializeField] float width = 2f;
    [SerializeField] new Camera camera;
    [SerializeField] CameraController cameraController;

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
        Selected = true;
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
