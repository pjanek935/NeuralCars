using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FitnessType
{
    ONLY_DISTANCE, DIST_MUL_SPEED, DIST_ADD_SPEED,
}

public class CarFitness : MonoBehaviour
{
    public delegate void OnGatePassedEventHandler (int gateIndex);

    public UnityAction OnWallHit;
    public event OnGatePassedEventHandler OnGatePassed;
    public event OnGatePassedEventHandler OnFinalGatePassed;

    [SerializeField] CarTelemetry carTelemetry;
    [SerializeField] CarNeuralCoreBase carNeuralCore;

    public FitnessType FitnessType
    {
        get;
        set;
    }

    public int GatesPassed
    {
        get;
        private set;
    }

    public float AvgVelocity
    {
        get;
        private set;
    }

    public float DistanceTravelled
    {
        get;
        set;
    }
    
    public Vector3 PosWhenDisabled
    {
        get;
        set;
    }

    public Quaternion RotationWhenDisabled
    {
        get;
        set;
    }

    public int Fitness
    {
        get 
        {
            return CalculateFitness (GatesPassed, DistanceTravelled, AvgVelocity, FitnessType);
        }
    }

    private void Awake ()
    {
        FitnessType = FitnessType.DIST_MUL_SPEED;
    }

    public static int CalculateFitness (int gatesPassed, float distanceTravelled, float avgSpeed, FitnessType fitnessType)
    {
        int result = 0;

        if (gatesPassed > GlobalConst.MIN_GATES_PASSED_WHEN_DISABLED_BASED_ON_AVG_VELOCITY)
        {//even when car did not pass any gate distanceTravelled will contain value grater than zero, hence this condition
            switch (fitnessType)
            {
                case FitnessType.DIST_ADD_SPEED:

                    result = (int) (distanceTravelled + avgSpeed);

                    break;

                case FitnessType.DIST_MUL_SPEED:

                    result = (int) (distanceTravelled * avgSpeed);

                    break;

                case FitnessType.ONLY_DISTANCE:

                    result = (int) distanceTravelled;

                    break;
            }
        }

        return result;
    }

    public void Reset ()
    {
        GatesPassed = 0;
        AvgVelocity = 0;
        DistanceTravelled = 0;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.transform.parent != null)
        {
            Gate gate = other.transform.GetComponent<Gate> ();

            if (gate != null)
            {
                if (! carNeuralCore.IsActive)
                {
                    return;
                }

                if (gate.Index > GatesPassed)
                {
                    GatesPassed = gate.Index;
                }

                AvgVelocity += carTelemetry.VelocityAverage.magnitude;
                AvgVelocity *= 0.5f;
                OnGatePassed?.Invoke (gate.Index);

                if (gate.IsFinalGate)
                {
                    OnFinalGatePassed?.Invoke (gate.Index);
                }
            }
        } 
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (! carNeuralCore.IsActive)
        {
            return;
        }

        if (string.Equals (collision.transform.tag, GlobalConst.WALL_TAG))
        {
            OnWallHit?.Invoke ();
        }
    }
}
