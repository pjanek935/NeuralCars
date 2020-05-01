using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

    public float speed = 20;

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collison)
    {
        if (collison.gameObject.tag == "Wall")
        {
            Destroy(this.gameObject);
        }
    }
}
