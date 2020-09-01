using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionSmoke;
    [SerializeField] ParticleSystem explosionDebris;
    [SerializeField] ParticleSystem explosionFlash;

    [SerializeField] new Rigidbody rigidbody;
    [SerializeField] CarController carController;

    [SerializeField] float forceOffset = 2f;
    [SerializeField] float maxTorque = 10f;
    [SerializeField] float maxOffset = 9000f;

    [SerializeField] GameObject wheelModels;

    public void Explode ()
    {
        explosionDebris.Play ();
        explosionSmoke.Play ();
        explosionFlash.Play ();

        wheelModels.SetActive (false);

        System.Random rand = new System.Random (Time.frameCount);
        Vector3 pos = this.transform.position;
        Vector3 posDiff = new Vector3 ((float) rand.NextDouble () * forceOffset - forceOffset / 2f,
            (float) rand.NextDouble () * forceOffset - forceOffset / 2f,
            (float) rand.NextDouble () * forceOffset - forceOffset / 2f);
        pos += posDiff;
        carController.ConstaintRotation = false;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.AddForceAtPosition (new Vector3 (0, (float) rand.NextDouble () * maxOffset, 0),
            pos,
            ForceMode.Impulse);
        Vector3 torque = new Vector3 ((float) rand.NextDouble () * maxTorque - maxTorque / 2f, (float) rand.NextDouble () * maxTorque - maxTorque / 2f,
            (float) rand.NextDouble () * maxTorque - maxTorque / 2f); ;
        rigidbody.AddTorque (torque, ForceMode.Impulse);
    }

    public void Reset ()
    {
        wheelModels.SetActive (true);
    }
}
