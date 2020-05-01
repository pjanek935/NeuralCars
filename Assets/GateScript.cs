using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

    private static int idCounter = 0;
    private int id;

	// Use this for initialization
	void Start () {
        id = idCounter;
        idCounter++;

        GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Car")
        {
            GameObject car = collider.gameObject;
            if (car.GetComponent<CarFitness>().IsActive())
            {
                car.GetComponent<CarFitness>().IncreaseFitness(1, id);
                //Debug.Log(car.GetComponent<FitnessScript>().GetFitness());
            }

        }
    }
}
