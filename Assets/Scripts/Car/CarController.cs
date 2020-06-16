using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] bool playerControlled = false;

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

    [SerializeField] float defaultFrontWheelsForwardFriction = 7.2f;
    [SerializeField] float defaultFrontWheelsSidewaysFriction = 7.2f;

    [SerializeField] float driftFrontWheelsForwardFriction = 7.2f;
    [SerializeField] float driftFrontWheelsSidewaysFriction = 7.2f;

    [SerializeField] float defaultRearWheelForwardFriction = 7.2f;
    [SerializeField] float defaultRearWheelSidewaysFriction = 7.2f;

    [SerializeField] float driftRearWheelForwardFriction = 7.2f;
    [SerializeField] float driftRearWheelSidewaysdFriction = 7.2f;

    [Range (0, 1f)] [SerializeField] float handBrakeFriction = 0.1f;

    [SerializeField] bool changeFriction = false;
    [SerializeField] float frictionChangeSmoothness = 10f;

    float prevTorque = 0;
    float torque = 0;
    float steerAngle = 0;
    bool handBrake = false;

    public float TorqueChange
    {
        get;
        private set;
    }

    void Start ()
    {
        GetComponent<Rigidbody> ().centerOfMass.Set (0, -0.9f, 0);
    }

    void Update ()
    {
        if (playerControlled)
        {
            prevTorque = torque;
            torque = Input.GetAxis ("Vertical"); ;
            steerAngle = Input.GetAxis ("Horizontal");
            handBrake = Input.GetKey (KeyCode.Space);
            TorqueChange = Mathf.Abs ((prevTorque - torque) * Time.deltaTime);
        }

        updateWheelTransforms ();
    }

    public void SetTorque (float torque)
    {
        this.torque = torque;
    }

    public void SetSteerAngle (float steerAngle)
    {
        this.steerAngle = steerAngle;
    }

    public void SetBrake (float brakeForce)
    {
        handBrake = brakeForce > 0.3f;
    }

    private void FixedUpdate ()
    {
        if (! handBrake)
        {
            wheelColliderRR.motorTorque = maxTorque * torque;
            wheelColliderRL.motorTorque = maxTorque * torque;
        }
        else
        {
            float friction = 1f - handBrakeFriction;
            wheelColliderRR.motorTorque = wheelColliderRR.motorTorque * friction;
            wheelColliderRL.motorTorque = wheelColliderRL.motorTorque * friction;
        }

        wheelColliderFL.steerAngle = maxSteerAngle * steerAngle;
        wheelColliderFR.steerAngle = maxSteerAngle * steerAngle;

        transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y, 0f);
        changeFrictionIfNeeded ();
    }

    void changeFrictionIfNeeded ()
    {
        if (changeFriction)
        {
            lerpFrontWheelsForwardFriction ();
            lerpFrontWheelsSidewaysFriction ();
            lerpRearWheelsForwardFriction ();
            lerpRearWheelsSidewaysFriction ();
        }
    }

    void lerpFrontWheelsForwardFriction ()
    {
        WheelFrictionCurve fronWheelForwardFrictionCurve = wheelColliderFR.forwardFriction;

        if (handBrake)
        {
            fronWheelForwardFrictionCurve.stiffness = driftFrontWheelsForwardFriction;
        }
        else
        {
            fronWheelForwardFrictionCurve.stiffness = Mathf.Lerp (fronWheelForwardFrictionCurve.stiffness, defaultFrontWheelsForwardFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderFR.forwardFriction = fronWheelForwardFrictionCurve;
        wheelColliderFL.forwardFriction = fronWheelForwardFrictionCurve;
    }

    void lerpFrontWheelsSidewaysFriction ()
    {
        WheelFrictionCurve fronWheelSidewaysFrictionCurve = wheelColliderFR.sidewaysFriction;

        if (handBrake)
        {
            fronWheelSidewaysFrictionCurve.stiffness = driftFrontWheelsSidewaysFriction;
        }
        else
        {
            fronWheelSidewaysFrictionCurve.stiffness = Mathf.Lerp (fronWheelSidewaysFrictionCurve.stiffness, defaultFrontWheelsSidewaysFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderFR.sidewaysFriction = fronWheelSidewaysFrictionCurve;
        wheelColliderFL.sidewaysFriction = fronWheelSidewaysFrictionCurve;
    }

    void lerpRearWheelsSidewaysFriction ()
    {
        WheelFrictionCurve rearWheelSidewaysFrictionCurve = wheelColliderRR.sidewaysFriction;

        if (handBrake)
        {
            rearWheelSidewaysFrictionCurve.stiffness = driftRearWheelSidewaysdFriction;
        }
        else
        {
            rearWheelSidewaysFrictionCurve.stiffness = Mathf.Lerp (rearWheelSidewaysFrictionCurve.stiffness, defaultRearWheelSidewaysFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderRR.sidewaysFriction = rearWheelSidewaysFrictionCurve;
        wheelColliderRL.sidewaysFriction = rearWheelSidewaysFrictionCurve;
    }

    void lerpRearWheelsForwardFriction ()
    {
        WheelFrictionCurve rearWheelForwardFrictionCurve = wheelColliderRR.forwardFriction;

        if (handBrake)
        {
            rearWheelForwardFrictionCurve.stiffness = driftRearWheelForwardFriction;
        }
        else
        {
            rearWheelForwardFrictionCurve.stiffness = Mathf.Lerp (rearWheelForwardFrictionCurve.stiffness, defaultRearWheelForwardFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderRR.forwardFriction = rearWheelForwardFrictionCurve;
        wheelColliderRL.forwardFriction = rearWheelForwardFrictionCurve;
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

    public float GetSteerAngleNormalized ()
    {
        return (wheelColliderFL.steerAngle / maxSteerAngle);
    }

    public float GetCurrentTorqueNormalized ()
    {
        return wheelColliderRL.motorTorque / maxTorque;
    }
}
