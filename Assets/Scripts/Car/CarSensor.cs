﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] float length = 10f;
    [SerializeField] float angle = 0f;

    private void OnValidate ()
    {
        Init (length, angle);
    }

    public void Init (float length, float angle)
    {
        transform.eulerAngles = new Vector3 (0f, angle, 0f);
        this.Length = length;

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
        }
    }

    public void ShootRaycast ()
    {
        int layerMask = LayerMask.NameToLayer ("Wall");
        RaycastHit hit;
        Vector3 hitLocalPos = transform.position + transform.forward * Length;

        if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, Length, layerMask))
        {
            Value = hit.distance / Length;
            Value = 1f - Value;
            hitLocalPos = hit.point;
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