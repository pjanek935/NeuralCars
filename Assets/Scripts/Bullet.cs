using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float velocity = 10;
    private Rigidbody rigidbody;
    public float lifeTime = 5;
    private float timer = 0;

    private static System.Random rand = new System.Random();

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        float rotation = (float)rand.NextDouble() * 60 - 30;
        transform.Rotate(new Vector3(0, rotation, 0));
        //rigidbody.AddForce(transform.forward, ForceMode.Impulse);
        rigidbody.velocity = transform.forward * velocity;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Agent")
        {
            GameObject agent = other.gameObject;
            agent.GetComponent<Fitness>().TurnOff();
        }
    }
}
