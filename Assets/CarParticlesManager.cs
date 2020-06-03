using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticlesManager : MonoBehaviour
{
    [SerializeField] CarTelemetry carTelemetry;
    [SerializeField] CarController carController;

    [SerializeField] ParticleSystem rrSmoke;
    [SerializeField] ParticleSystem rlSmoke;
    [SerializeField] ParticleSystem frSmoke;
    [SerializeField] ParticleSystem flSmoke;

    private void Update ()
    {
        updateParticles ();
    }

    void updateParticles ()
    {
        float alpha = carTelemetry.GetAngleBetweenForwardAndMovementDirection ();
        float torqueChange = carController.TorqueChange;

        if (torqueChange > 0.5)
        {
            StartSmoke ();
        }
        else if (Mathf.Abs (alpha) > 30f && Mathf.Abs (alpha) < 170)
        {
            StartSmoke ();
        }
        else
        {
            StopSmoke ();
        }
    }

    public void StartSmoke ()
    {
        ParticleSystem.EmissionModule emmision = rrSmoke.emission;
        emmision.enabled = true;
        emmision = rlSmoke.emission;
        emmision.enabled = true;
        emmision = frSmoke.emission;
        emmision.enabled = true;
        emmision = flSmoke.emission;
        emmision.enabled = true;
    }

    public void StopSmoke ()
    {
        ParticleSystem.EmissionModule emmision = rrSmoke.emission;
        emmision.enabled = false;
        emmision = rlSmoke.emission;
        emmision.enabled = false;
        emmision = frSmoke.emission;
        emmision.enabled = false;
        emmision = flSmoke.emission;
        emmision.enabled = false;
    }
}
