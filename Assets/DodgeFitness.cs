using UnityEngine;
using System.Collections;

public class DodgeFitness : Fitness {

    private int hitCounter = 0;
    private int maxHits = 2;

    // Use this for initialization
    void Start ()
    {
        Init();
	}
	
	// Update is called once per frame
	void Update () {
        if (IsActive())
            IncreaseFitness(Time.deltaTime);
	}

    public void OnCollisionEnter(Collision other)
    {
        if (!stopOnHit) return;
        if (other.gameObject.tag == "Wall")
        {
            TurnOff();
            hitCounter = 0;
        }
    }
}
