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

    [SerializeField] float defaultRearWheelFriction = 7.2f;
    [SerializeField] float handBrakeRearWheelFriction = 5f;
    [SerializeField] float defaultFrontWheelFriction = 11f;
    [SerializeField] float handBrakeFrontWheelFriction = 5f;
    [Range (0, 1f)] [SerializeField] float handBrakeFriction = 0.1f;

    float torque = 0;
    float steerAngle = 0;
    bool handBrake = false;
    Vector3 prevPosition = Vector3.zero;
    Queue<Vector3> movementDirectionBuffer = new Queue<Vector3> ();

    public Vector3 MovementDirection
    {
        get;
        private set;
    }

    void Start ()
    {
        prevPosition = transform.position;
        GetComponent<Rigidbody> ().centerOfMass.Set (0, -0.9f, 0);
    }

    void Update ()
    {
        if (playerControlled)
        {
            torque = Input.GetAxis ("Vertical"); ;
            steerAngle = Input.GetAxis ("Horizontal");
            handBrake = Input.GetKey (KeyCode.Space);
        }

        updateWheelTransforms ();
        updateMovementParameters ();
    }

    public void SetTorque (float torque)
    {
        this.torque = torque;
    }

    public void SetSteerAngle (float steerAngle)
    {
        this.steerAngle = steerAngle;
    }

    private void FixedUpdate ()
    {
        if (! handBrake)
        {
            wheelColliderRR.motorTorque = maxTorque * torque;
            wheelColliderRL.motorTorque = maxTorque * torque;

            WheelFrictionCurve frictionCurve = wheelColliderRR.sidewaysFriction;
            frictionCurve.stiffness = defaultRearWheelFriction;
            wheelColliderRR.sidewaysFriction = frictionCurve;
            wheelColliderRL.sidewaysFriction = frictionCurve;

            frictionCurve = wheelColliderFL.sidewaysFriction;
            frictionCurve.stiffness = defaultFrontWheelFriction;
            wheelColliderFL.sidewaysFriction = frictionCurve;
            wheelColliderFR.sidewaysFriction = frictionCurve;
        }
        else
        {
            float friction = 1f - handBrakeFriction;
            wheelColliderRR.motorTorque = wheelColliderRR.motorTorque * friction;
            wheelColliderRL.motorTorque = wheelColliderRL.motorTorque * friction;

            WheelFrictionCurve frictionCurve = wheelColliderRR.sidewaysFriction;
            frictionCurve.stiffness = handBrakeRearWheelFriction;
            wheelColliderRR.sidewaysFriction = frictionCurve;
            wheelColliderRL.sidewaysFriction = frictionCurve;

            frictionCurve = wheelColliderFL.sidewaysFriction;
            frictionCurve.stiffness = handBrakeFrontWheelFriction;
            wheelColliderFL.sidewaysFriction = frictionCurve;
            wheelColliderFR.sidewaysFriction = frictionCurve;
        }

        wheelColliderFL.steerAngle = maxSteerAngle * steerAngle;
        wheelColliderFR.steerAngle = maxSteerAngle * steerAngle;
    }

    void updateMovementParameters ()
    {
        MovementDirection = transform.position - prevPosition;

        if (MovementDirection.magnitude < 0.1f)
        {
            MovementDirection = Vector3.zero;
        }

        MovementDirection.Normalize ();
        prevPosition = transform.position;
        movementDirectionBuffer.Enqueue (MovementDirection);

        if (movementDirectionBuffer.Count > 10)
        {
            movementDirectionBuffer.Dequeue ();
        }

        MovementDirection = calculateAverageMovementDirection ();

        Debug.DrawLine (this.transform.position, this.transform.position + MovementDirection * 10, Color.yellow);
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

    public Vector3 calculateAverageMovementDirection ()
    {
        Vector3 result = Vector3.zero;
        
        foreach (Vector3 v in movementDirectionBuffer)
        {
            result += v;
        }

        result /= movementDirectionBuffer.Count;
        result.Normalize ();

        return result;
    }

    public float GetAngleBetweenForwardAndMovementDirection (bool normalized = false)
    {
        float result = Vector3.SignedAngle (transform.forward, MovementDirection, Vector3.up);

        if (normalized)
        {
            if (result > 180)
            {
                result = 180;
            }
            else if (result < -180)
            {
                result = -180;
            }

            result /= 180;
        }

        return result;
    }

    public float GetCurrentTorqueNormalized ()
    {
        return wheelColliderRL.motorTorque / maxTorque;
    }
}
