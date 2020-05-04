using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectibleScript : MonoBehaviour {

    public static int idCounter = 0;
    public int id;

	// Use this for initialization
	void Start () {
        id = idCounter;
        idCounter++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Agent")
        {
            other.gameObject.GetComponent<Fitness>().IncreaseFitness(1);
            //Destroy();
            Destroy(this.gameObject);
        }
    }

    public void Destroy()
    {
        GameObject observer = GameObject.Find("Observer");
        observer.GetComponent<CollectibleObserver>().DestroyCollectible(id);
    }
}
