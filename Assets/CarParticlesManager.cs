using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticlesManager : MonoBehaviour
{
    [SerializeField] ParticleSystem rrSmoke;
    [SerializeField] ParticleSystem rlSmoke;
    [SerializeField] ParticleSystem frSmoke;
    [SerializeField] ParticleSystem flSmoke;

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
