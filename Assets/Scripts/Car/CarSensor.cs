using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single ray cats shoot from front of the car.
/// </summary>
public class CarSensor : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    int layerMask;

    public float Length
    {
        get;
        private set;
    }

    public float Value
    {
        get;
        private set;
    }

    private void OnEnable ()
    {
        layerMask = LayerMask.GetMask (GlobalConst.WALL_TAG);
    }

    public void Init (float length, float angle)
    {
        SetAngle (angle);
        this.Length = length;

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
        }
    }

    public void SetVisible (bool visible)
    {
        lineRenderer.enabled = visible;
    }

    public void SetLength (float length)
    {
        this.Length = length;
    }

    public void SetAngle (float angle)
    {
        transform.localEulerAngles = new Vector3 (0f, angle, 0f);
    }

    public void Disable ()
    {
        Value = 1f;
        lineRenderer.SetPositions (new Vector3 [2] { transform.position, transform.position });
    }

    private void FixedUpdate ()
    {
        if (lineRenderer.enabled)
        {
            Vector3 endPoint = transform.TransformDirection (Vector3.forward) * Length * (1f - Value);
            endPoint = transform.position + endPoint;
            lineRenderer.SetPositions (new Vector3 [2] { transform.position, endPoint });
        }
    }

    public void ShootRaycast ()
    {
        RaycastHit hit;

        if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, Length, layerMask))
        {
            Value = hit.distance / Length;
            Value = 1f - Value;
        }
        else
        {
            Value = 0f;
        }
    }
}
