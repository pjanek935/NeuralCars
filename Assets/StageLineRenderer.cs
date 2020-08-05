using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class StageLineRenderer : MonoBehaviour
{
    [SerializeField] float minWidth = 0.1f;
    [SerializeField] float maxWidh = 3f;
    [SerializeField] CameraController cameraController;

    LineRenderer lineRenderer;

    private void OnEnable ()
    {
        cameraController.OnPositionUpdated += onCameraPosUpdated;
        onCameraPosUpdated ();
    }

    private void OnDisable ()
    {
        cameraController.OnPositionUpdated -= onCameraPosUpdated;
    }

    void onCameraPosUpdated ()
    {
        float camerPosYNormalized = (cameraController.transform.position.y - cameraController.MinYPos) / (cameraController.MaxYPos - cameraController.MinYPos);
        float newLineWidth = minWidth + camerPosYNormalized * (maxWidh - minWidth);
        getLinerRendererIfNeeded ();
        lineRenderer.startWidth = newLineWidth;
        lineRenderer.endWidth = newLineWidth;
    }

    void getLinerRendererIfNeeded ()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer> ();
        }
    }
}
