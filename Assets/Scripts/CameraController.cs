using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (Camera))]
public class CameraController : MonoBehaviour
{
    public enum CameraState
    {
        NONE, DRAGGING
    }

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] EventSystem eventSystem;

    Vector3 prevPointerPos = Vector3.zero;
    Vector3 prevRayHitPos = Vector3.zero;

    public CameraState CurrentState
    {
        get;
        private set;
    }

    public bool IsPointerOverGUI ()
    {
        bool result = false;

        if (eventSystem != null)
        {
            result = eventSystem.IsPointerOverGameObject ();
        }

        return result;
        
    }

    private void Update ()
    {
        if (Input.GetMouseButtonDown (1))
        {
            startDragging ();
        }
        else if (Input.GetMouseButtonUp (1))
        {
            stopDragging ();
        }

        updateState ();
        updateZooming ();
    }

    void updateZooming ()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (Mathf.Abs (scrollDelta) > 0.1f)
        {
            float deltaY = -scrollDelta * scrollSpeed;
            float deltaZ = -Mathf.Cos (90f-this.transform.localRotation.eulerAngles.x) * deltaY;
            Vector3 newPos = this.transform.position;
            newPos.y += deltaY;
            newPos.z += deltaZ;
            this.transform.position = newPos;
        }
    }

    void updateState ()
    {
        switch (CurrentState)
        {
            case CameraState.DRAGGING:

                updateDragging ();

                break;
        }
    }

    void updateDragging ()
    {
        Vector3 currentMousePos = getCurrentMousePosition ();
        Vector3 currentRayHitPos = Vector3.zero;
        Camera camera = GetComponent<Camera> ();
        Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (raycast, out hit))
        {
            currentRayHitPos = hit.point;
        }


        Vector3 deltaPos = currentMousePos - prevPointerPos;
        deltaPos.y = 0;
        deltaPos *= moveSpeed;
        Vector3 newPos = this.transform.position - deltaPos;
        this.transform.position = newPos;
        prevPointerPos = currentMousePos;
    }

    void startDragging ()
    {
        CurrentState = CameraState.DRAGGING;
        prevPointerPos = getCurrentMousePosition ();
        Camera camera = GetComponent<Camera> ();
        Ray raycast = camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (raycast, out hit))
        {
            prevRayHitPos = hit.point;
        }
    }

    void stopDragging ()
    {
        CurrentState = CameraState.NONE;
    }

    Vector3 getCurrentMousePosition ()
    {
        Vector3 result;
        result.x = Input.mousePosition.x;
        result.y = 0;
        result.z = Input.mousePosition.y;

        return result;
    }
}
