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

    [SerializeField] int gatesPassed;
    [SerializeField] float avgVelocity;
    [SerializeField] float distanceTravelled;
    [SerializeField] float driftScore;

    const float DISTANCE_FACTOR = 10f;

    public int GatesPassed
    {
        get { return gatesPassed; }
        set { gatesPassed = value; }
    }

    public float AvgVelocity
    {
        get { return avgVelocity; }
        set { avgVelocity = value; }
    }

    public float DistanceTravelled
    {
        get { return distanceTravelled; }
        set { distanceTravelled = value; }
    }

    public float DriftScore
    {
        get { return driftScore; }
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
            return CalculateFitness (gatesPassed, distanceTravelled, avgVelocity, driftScore);
        }
    }

    public static int CalculateFitness (int gatesPassed, float distanceTravelled, float avgSpeed, float driftScore)
    {
        int result = 0;

        if (gatesPassed > 5)
        {
            result = (int) (distanceTravelled * DISTANCE_FACTOR * avgSpeed);
        }

        return result;
    }

    public void Reset ()
    {
        GatesPassed = 0;
        avgVelocity = 0;
        distanceTravelled = 0;
        driftScore = 0;
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
                    avgVelocity += carTelemetry.VelocityAverage.magnitude;
                    avgVelocity /= 2f;
                }
            }
        } 
    }

    private void OnCollisionEnter (Collision collision)
    {
        if (!carNeuralCore.IsActive)
        {
            return;
        }

        if (string.Equals (collision.transform.tag, "Wall"))
        {
            OnWallHit?.Invoke ();
        }
    }

    //private void FixedUpdate ()
    //{
    //    driftScore += Mathf.Abs (carTelemetry.GetAngleBetweenForwardAndMovementDirection (true));
    //}
}
