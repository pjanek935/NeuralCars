using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarFitness : MonoBehaviour
{
    public UnityAction OnWallHit;

    [SerializeField] CarTelemetry carTelemetry;

    public int GatesPassed;
    float avgVelocity = 0;

    public void Reset ()
    {
        GatesPassed = 0;
    }

    public int GetFitness ()
    {
        return GatesPassed * (int) avgVelocity;
    }

    private void OnTriggerEnter (Collider other)
    {
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
        if (string.Equals (collision.transform.tag, "Wall"))
        {
            OnWallHit?.Invoke ();
        }
    }
}
