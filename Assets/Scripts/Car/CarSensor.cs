using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single ray cats shoot from front of the car.
/// </summary>
public class CarSensor : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

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

    public void ShootRaycast ()
    {
        int layerMask = LayerMask.GetMask (GlobalConst.WALL_TAG);

        RaycastHit hit;
        Vector3 hitLocalPos = transform.position + transform.forward * Length;

        if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, Length, layerMask))
        {
            Value = hit.distance / Length;
            Value = 1f - Value;
            hitLocalPos = hit.point;

            Debug.DrawLine (transform.position, hit.point, Color.red);
        }
        else
        {
            Value = 0f;
        }

        if (lineRenderer != null)
        {
            lineRenderer.SetPositions (new Vector3 [2] { transform.position, hitLocalPos});
        }
    }
}
