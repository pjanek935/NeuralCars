using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] bool playerControlled = false;

    [SerializeField] CarParticlesManager carParticlesManager;

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
    [SerializeField] bool changeFriction = false;

    float torqueChange = 0;
    float prevTorque = 0;
    float torque = 0;
    float steerAngle = 0;
    bool handBrake = false;
    Vector3 prevPosition = Vector3.zero;
    Queue<Vector3> movementDirectionBuffer = new Queue<Vector3> ();

    public Vector3 MovementDirectionAverage
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
            prevTorque = torque;
            torque = Input.GetAxis ("Vertical"); ;
            steerAngle = Input.GetAxis ("Horizontal");
            handBrake = Input.GetKey (KeyCode.Space);
            torqueChange = Mathf.Abs ((prevTorque - torque) * Time.deltaTime);
        }

        updateWheelTransforms ();
        updateMovementParameters ();
        updateParticles ();
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

            if (changeFriction)
            {
                WheelFrictionCurve frictionCurve = wheelColliderRR.sidewaysFriction;
                frictionCurve.stiffness = defaultRearWheelFriction;
                wheelColliderRR.sidewaysFriction = frictionCurve;
                wheelColliderRL.sidewaysFriction = frictionCurve;

                frictionCurve = wheelColliderFL.sidewaysFriction;
                frictionCurve.stiffness = defaultFrontWheelFriction;
                wheelColliderFL.sidewaysFriction = frictionCurve;
                wheelColliderFR.sidewaysFriction = frictionCurve;
            }
            
        }
        else
        {
            float friction = 1f - handBrakeFriction;
            wheelColliderRR.motorTorque = wheelColliderRR.motorTorque * friction;
            wheelColliderRL.motorTorque = wheelColliderRL.motorTorque * friction;

            if (changeFriction)
            {
                WheelFrictionCurve frictionCurve = wheelColliderRR.sidewaysFriction;
                frictionCurve.stiffness = handBrakeRearWheelFriction;
                wheelColliderRR.sidewaysFriction = frictionCurve;
                wheelColliderRL.sidewaysFriction = frictionCurve;

                frictionCurve = wheelColliderFL.sidewaysFriction;
                frictionCurve.stiffness = handBrakeFrontWheelFriction;
                wheelColliderFL.sidewaysFriction = frictionCurve;
                wheelColliderFR.sidewaysFriction = frictionCurve;
            }
        }

        wheelColliderFL.steerAngle = maxSteerAngle * steerAngle;
        wheelColliderFR.steerAngle = maxSteerAngle * steerAngle;

        transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y, 0f);
    }

    void updateParticles ()
    {  
        float alpha = GetAngleBetweenForwardAndMovementDirection (true);

        if (torqueChange > 0)
        {
            carParticlesManager.StartSmoke ();
        }
        else if (Mathf.Abs (alpha) > 0.03f)
        {
            carParticlesManager.StartSmoke ();
        }
        else
        {
            carParticlesManager.StopSmoke ();
        }
    }

    void updateMovementParameters ()
    {
        MovementDirectionAverage = transform.position - prevPosition;

        if (MovementDirectionAverage.magnitude < 0.1f)
        {
            MovementDirectionAverage = Vector3.zero;
        }

        Vector3 currentVelocity = MovementDirectionAverage / Time.deltaTime;

        MovementDirectionAverage.Normalize ();
        movementDirectionBuffer.Enqueue (MovementDirectionAverage);

        if (movementDirectionBuffer.Count > 10)
        {
            movementDirectionBuffer.Dequeue ();
        }

        MovementDirectionAverage = calculateAverage (movementDirectionBuffer).normalized;
        prevPosition = transform.position;


        Debug.DrawLine (this.transform.position, this.transform.position + MovementDirectionAverage * 10, Color.yellow);
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

    public Vector3 calculateAverage (Queue <Vector3> queue)
    {
        Vector3 result = Vector3.zero;
        
        foreach (Vector3 v in queue)
        {
            result += v;
        }

        result /= queue.Count;

        return result;
    }

    public float GetAngleBetweenForwardAndMovementDirection (bool normalized = false)
    {
        float result = Vector3.SignedAngle (transform.forward, MovementDirectionAverage, Vector3.up);

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
