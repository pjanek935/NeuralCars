using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageFloor : MonoBehaviour
{
    public delegate void OnFloorClickedEventHandler (Vector3 pos);
    public event OnFloorClickedEventHandler OnFloorClicked;

    [SerializeField] new Camera camera;
    [SerializeField] CameraController cameraController;
    [SerializeField] List<OnMouseEventListener> listeners = new List<OnMouseEventListener> ();

    private void OnEnable ()
    {
        if (listeners != null)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners [i] != null)
                {
                    listeners [i].OnDown += shootRaycast;
                }
            }
        }
    }

    private void OnDisable ()
    {
        if (listeners != null)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners [i] != null)
                {
                    listeners [i].OnDown -= shootRaycast;
                }
            }
        }
    }

    void shootRaycast ()
    {
        Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask (GlobalConst.FLOOR_LAYER, GlobalConst.ROAD_LAYER);

        if (Physics.Raycast (raycast, out hit, 1000, layerMask))
        {
            if (cameraController != null && !cameraController.IsPointerOverGUI ())
            {
                Vector3 pos = hit.point;
                pos.y = 0;

                if (Vector3.Distance (Vector3.zero, pos) < GlobalConst.STAGE_RADIUS)
                {
                    OnFloorClicked?.Invoke (pos);
                }
            }
        }
    }

    private void OnMouseDown ()
    {
        shootRaycast ();
    }
}
