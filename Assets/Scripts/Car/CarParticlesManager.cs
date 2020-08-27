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

    [SerializeField] float minTorqueChange = 0.5f; //Torque change which will relase smoke from under wheels of the car.
    [SerializeField] float minAngleBetweenForwardVectorAndMovementDirection = 30f; //The bigger the value, the grater angle car has to slide to release smoke

    private void Update ()
    {
        updateParticles ();
    }

    private void OnValidate ()
    {
        minAngleBetweenForwardVectorAndMovementDirection = Mathf.Clamp (minAngleBetweenForwardVectorAndMovementDirection, 0f, 180f);
    }

    void updateParticles ()
    {
        float alpha = carTelemetry.GetAngleBetweenForwardAndMovementDirection ();
        float torqueChange = carController.TorqueChange;

        if (torqueChange > minTorqueChange)
        {
            StartSmoke ();
        }
        else if (Mathf.Abs (alpha) > minAngleBetweenForwardVectorAndMovementDirection && Mathf.Abs (alpha) < 180f)
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
