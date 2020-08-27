using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

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

    [SerializeField] float frictionChangeSmoothness = 10f;

    float prevTorque = 0;
    public bool IsHandbrakeOn
    {
        get;
        private set;
    }

    public float TorqueChange
    {
        get;
        private set;
    }

    public float Torque
    {
        get;
        private set;
    }

    public float SteerAngle
    {
        get;
        private set;
    }

    public bool ConstaintRotation
    {
        get;
        set;
    }

    void Start ()
    {
        GetComponent<Rigidbody> ().centerOfMass.Set (0, GlobalConst.CAR_CENTER_OF_MASS_Y, 0);
    }

    void Update ()
    {
        if (playerControlled)
        {
            prevTorque = Torque;
            Torque = Input.GetAxis ("Vertical"); ;
            SteerAngle = Input.GetAxis ("Horizontal");
            IsHandbrakeOn = Input.GetKey (KeyCode.Space);
            TorqueChange = Mathf.Abs ((prevTorque - Torque) * Time.deltaTime);

            //Debug.Log ("Torque: " + torque);
            //Debug.Log ("Steer angle: " + steerAngle);
            //Debug.Log ("Torque change: " + TorqueChange);
        }

        updateWheelTransforms ();
    }

    public void SetTorque (float torque)
    {
        this.Torque = torque;
    }

    public void SetSteerAngle (float steerAngle)
    {
        this.SteerAngle = steerAngle;
    }

    public void SetBrake (bool handBrake)
    {
        this.IsHandbrakeOn = handBrake;
    }

    private void FixedUpdate ()
    {
        if (! IsHandbrakeOn)
        {
            wheelColliderRR.motorTorque = maxTorque * Torque;
            wheelColliderRL.motorTorque = maxTorque * Torque;
        }
        else
        {
            float friction = 1f - handBrakeFriction;
            wheelColliderRR.motorTorque = wheelColliderRR.motorTorque * friction;
            wheelColliderRL.motorTorque = wheelColliderRL.motorTorque * friction;
        }

        wheelColliderFL.steerAngle = maxSteerAngle * SteerAngle;
        wheelColliderFR.steerAngle = maxSteerAngle * SteerAngle;

        if (ConstaintRotation)
        {
            transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y, 0f);
        }
        
        changeFriction ();
    }

    /// <summary>
    /// Return to default wheel friction after using handbrake.
    /// </summary>
    void changeFriction ()
    {
        lerpFrontWheelsForwardFriction ();
        lerpFrontWheelsSidewaysFriction ();
        lerpRearWheelsForwardFriction ();
        lerpRearWheelsSidewaysFriction ();
    }

    void lerpFrontWheelsForwardFriction ()
    {
        WheelFrictionCurve fronWheelForwardFrictionCurve = wheelColliderFR.forwardFriction;

        if (IsHandbrakeOn)
        {
            fronWheelForwardFrictionCurve.stiffness = driftFrontWheelsForwardFriction;
        }
        else
        {
            fronWheelForwardFrictionCurve.stiffness = Mathf.Lerp (fronWheelForwardFrictionCurve.stiffness,
                defaultFrontWheelsForwardFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderFR.forwardFriction = fronWheelForwardFrictionCurve;
        wheelColliderFL.forwardFriction = fronWheelForwardFrictionCurve;
    }

    void lerpFrontWheelsSidewaysFriction ()
    {
        WheelFrictionCurve fronWheelSidewaysFrictionCurve = wheelColliderFR.sidewaysFriction;

        if (IsHandbrakeOn)
        {
            fronWheelSidewaysFrictionCurve.stiffness = driftFrontWheelsSidewaysFriction;
        }
        else
        {
            fronWheelSidewaysFrictionCurve.stiffness = Mathf.Lerp (fronWheelSidewaysFrictionCurve.stiffness,
                defaultFrontWheelsSidewaysFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderFR.sidewaysFriction = fronWheelSidewaysFrictionCurve;
        wheelColliderFL.sidewaysFriction = fronWheelSidewaysFrictionCurve;
    }

    void lerpRearWheelsSidewaysFriction ()
    {
        WheelFrictionCurve rearWheelSidewaysFrictionCurve = wheelColliderRR.sidewaysFriction;

        if (IsHandbrakeOn)
        {
            rearWheelSidewaysFrictionCurve.stiffness = driftRearWheelSidewaysdFriction;
        }
        else
        {
            rearWheelSidewaysFrictionCurve.stiffness = Mathf.Lerp (rearWheelSidewaysFrictionCurve.stiffness,
                defaultRearWheelSidewaysFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderRR.sidewaysFriction = rearWheelSidewaysFrictionCurve;
        wheelColliderRL.sidewaysFriction = rearWheelSidewaysFrictionCurve;
    }

    void lerpRearWheelsForwardFriction ()
    {
        WheelFrictionCurve rearWheelForwardFrictionCurve = wheelColliderRR.forwardFriction;

        if (IsHandbrakeOn)
        {
            rearWheelForwardFrictionCurve.stiffness = driftRearWheelForwardFriction;
        }
        else
        {
            rearWheelForwardFrictionCurve.stiffness = Mathf.Lerp (rearWheelForwardFrictionCurve.stiffness,
                defaultRearWheelForwardFriction, frictionChangeSmoothness * Time.deltaTime);
        }

        wheelColliderRR.forwardFriction = rearWheelForwardFrictionCurve;
        wheelColliderRL.forwardFriction = rearWheelForwardFrictionCurve;
    }

    void updateWheelTransform (WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelCollider != null && wheelTransform != null)
        {
            Vector3 pos;
            Quaternion quat;
            wheelCollider.GetWorldPose (out pos, out quat);
            wheelTransform.position = pos;
            wheelTransform.rotation = quat;
            wheelTransform.Rotate (new Vector3 (0, 0, 90));
        }
    }

    /// <summary>
    /// Update wheel mesh to match its collider.
    /// </summary>
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
