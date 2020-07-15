using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarFitness : MonoBehaviour
{
    public UnityAction OnWallHit;
    public UnityAction OnGatePassed;

    [SerializeField] CarTelemetry carTelemetry;
    [SerializeField] CarNeuralCore carNeuralCore;

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

    public int Fitness
    {
        get 
        {
            return CalculateFitness (GatesPassed, DistanceTravelled, AvgVelocity);
        }
    }

    public static int CalculateFitness (int gatesPassed, float distanceTravelled, float avgSpeed)
    {
        int result = 0;

        if (gatesPassed > 1)
        {//even when car did not pass any gate distanceTravelled will contain value grater than zero, hence this condition
            result = (int) (distanceTravelled * avgSpeed);
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
        if (! carNeuralCore.IsActive)
        {
            return;
        }

        if (other.transform.parent != null)
        {
            Gate gate = other.transform.GetComponent<Gate> ();

            if (gate != null)
            {
                if (gate.Index > GatesPassed)
                {
                    GatesPassed = gate.Index;
                }

                AvgVelocity += carTelemetry.VelocityAverage.magnitude;
                AvgVelocity *= 0.5f;
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
