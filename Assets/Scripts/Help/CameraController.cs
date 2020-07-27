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

    [SerializeField] float maxDistFromCenter = 250f;
    [SerializeField] float maxYPos = 0f;
    [SerializeField] float minYPos = 100f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] float rotationSmoothingFactor = 5f; //The bigger the number the slower camera will rotate
    [SerializeField] EventSystem eventSystem;

    Vector3 prevPointerPos = Vector3.zero;
    Quaternion defaultQuaternion;

    private void Start ()
    {
        defaultQuaternion = this.transform.rotation;
    }

    public CameraState CurrentState
    {
        get;
        private set;
    }

    public void SetDefaultQuaternion ()
    {
        this.transform.rotation = defaultQuaternion;
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

    private void FixedUpdate ()
    {
        transform.rotation = Quaternion.Slerp (transform.rotation, defaultQuaternion, rotationSmoothingFactor * Time.deltaTime);
    }

    void updateZooming ()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (Mathf.Abs (scrollDelta) > 0.1f)
        {
            float deltaY = -scrollDelta * scrollSpeed;
            float deltaZ = -Mathf.Cos (90f-this.transform.localRotation.eulerAngles.x) * deltaY;
            Vector3 newPos = this.transform.position;
            float newY = newPos.y + deltaY;
            newPos.y += deltaY;
            newPos.z += deltaZ;
            newPos.y = Mathf.Clamp (newPos.y, minYPos, maxYPos);

            if (newPos.y != this.transform.position.y)
            {
                updatePos (newPos);
            }
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
        Vector3 deltaPos = currentMousePos - prevPointerPos;
        deltaPos.y = 0;
        deltaPos *= moveSpeed;
        Vector3 newPos = this.transform.position - deltaPos;

        updatePos (newPos);
        prevPointerPos = currentMousePos;
    }

    void updatePos (Vector3 newPos)
    {
        if (Vector3.Distance (Vector3.zero, newPos) > maxDistFromCenter)
        {
            Vector3 dirNorm = newPos.normalized;
            float y = newPos.y;
            newPos = dirNorm * maxDistFromCenter;
            newPos.y = y;
        }

        this.transform.position = newPos;
    }

    void startDragging ()
    {
        CurrentState = CameraState.DRAGGING;
        prevPointerPos = getCurrentMousePosition ();
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
