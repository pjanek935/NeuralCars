﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageFloor : MonoBehaviour
{
    public delegate void OnFloorClickedEventHandler (Vector3 pos);
    public event OnFloorClickedEventHandler OnFloorClicked;

    [SerializeField] new Camera camera;
    [SerializeField] CameraController cameraController;

    private void OnMouseDown ()
    {
        Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask ("Floor");

        if (Physics.Raycast (raycast, out hit, 1000, layerMask))
        {
            if (cameraController != null && !cameraController.IsPointerOverGUI ())
            {
                Vector3 pos = hit.point;
                pos.y = 0;
                OnFloorClicked?.Invoke (pos);
            }
        }
    }
}
