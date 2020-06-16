using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSensor : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    public float Length
    {
        get { return length; }
    }

    public float Value
    {
        get;
        private set;
    }

    [SerializeField] float length = 10f;
    [SerializeField] float angle = 0f;

    private void OnValidate ()
    {
        Init (length, angle);
    }

    public void Init (float length, float angle)
    {
        SetAngle (angle);
        this.length = length;

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
        this.length = length;
    }

    public void SetAngle (float angle)
    {
        transform.localEulerAngles = new Vector3 (0f, angle, 0f);
    }

    public void ShootRaycast ()
    {
        int layerMask = LayerMask.GetMask ("Wall");

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
