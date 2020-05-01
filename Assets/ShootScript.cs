using UnityEngine;
using System.Collections;

public class ShootScript : MonoBehaviour {

    public GameObject bullet;

    private float shootTimer = 0;
    private float shootLimiter = 2;

    private static System.Random rand = new System.Random();

	// Use this for initialization
	void Start () {
        shootLimiter = rand.Next(3, 7);
	}
	
	// Update is called once per frame
	void Update () {
        shootTimer += Time.deltaTime;
        if (shootTimer > shootLimiter)
        {
            shootTimer = 0;
            Shoot();
        }
	}

    void Shoot()
    {
        //Quaternion rotation = Quaternion.Euler(transform.rotation.ToEuler().x,
        //    (float)rand.NextDouble() * 60 - 30, transform.rotation.ToEuler().z);
        Instantiate(bullet, transform.position + transform.forward * 3, transform.rotation);
        shootLimiter = rand.Next(3, 7);
    }
}
