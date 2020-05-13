using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] WheelCollider wheelColliderFL;
    [SerializeField] WheelCollider wheelColliderFR;
    [SerializeField] WheelCollider wheelColliderRL;
    [SerializeField] WheelCollider wheelColliderRR;

    [SerializeField] Transform wheelTransformFL;
    [SerializeField] Transform wheelTransformFR;
    [SerializeField] Transform wheelTransformRL;
    [SerializeField] Transform wheelTransformRR;

    [SerializeField] float maxTorque = 50;
    [SerializeField] float maxSteerAngle = 40;

    float torque = 0;
    float steerAngle = 0;

    // Use this for initialization
    /// <summary>
    /// 
    /// </summary>
    void Start ()
    {
        GetComponent<Rigidbody> ().centerOfMass.Set (0, -0.9f, 0);
    }

    // Update is called once per frame
    void Update ()
    {
        torque = Input.GetAxis ("Vertical"); ;
        steerAngle = Input.GetAxis ("Horizontal");

        wheelColliderRR.motorTorque = maxTorque * torque;
        wheelColliderRL.motorTorque = maxTorque * torque;

        wheelColliderFL.steerAngle = maxSteerAngle * steerAngle;
        wheelColliderFR.steerAngle = maxSteerAngle * steerAngle;

        updateWheelTransforms ();
    }

    void updateWheelTransform (WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelCollider != null && wheelTransform != null)
        {
            Vector3 pos = wheelTransform.position;
            Quaternion quat = wheelTransform.rotation;
            wheelCollider.GetWorldPose (out pos, out quat);
            wheelTransform.position = pos;
            wheelTransform.rotation = quat;
            wheelTransform.Rotate (new Vector3 (0, 0, 90));
        }
    }

    void updateWheelTransforms ()
    {
        updateWheelTransform (wheelColliderFL, wheelTransformFL);
        updateWheelTransform (wheelColliderFR, wheelTransformFR);
        updateWheelTransform (wheelColliderRL, wheelTransformRL);
        updateWheelTransform (wheelColliderRR, wheelTransformRR);
    }
}
