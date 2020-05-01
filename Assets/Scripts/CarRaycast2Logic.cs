using UnityEngine;
using System.Collections;

public class CarRaycast2Logic : MonoBehaviour {


    private double stimulation;
    public GameObject parentCar;
    public GameObject point;
    public GameObject start;
    public GameObject end;

    private int layerMask;

    // Use this for initialization
    void Start () {
        layerMask = LayerMask.GetMask("Wall");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {

            //Debug.Log("!!!");

            Vector3 direction = end.transform.position - start.transform.position;
            float length = Vector3.Distance(start.transform.position, end.transform.position);
            //Debug.Log(length);
            RaycastHit hit;
            if (Physics.Raycast(start.transform.position, direction, out hit, length, layerMask))
            {
                //Debug.Log(hit.collider.gameObject.name);
                point.transform.position = hit.point;
                stimulation = 1 - Vector3.Distance(start.transform.position, hit.point) / length;
                //Debug.Log(stimulation);
                //Debug.DrawRay(start.transform.position, direction, Color.green);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {

            //Debug.Log("!!!");

            Vector3 direction = end.transform.position - start.transform.position;
            float length = Vector3.Distance(start.transform.position, end.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(start.transform.position, direction, out hit, length, layerMask))
            {

                point.transform.position = hit.point;
                stimulation = 1 - Vector3.Distance(start.transform.position, hit.point) / length;
                //Debug.Log(stimulation);
                //Debug.DrawRay(start.transform.position, direction, Color.green);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            stimulation = 0;
        }
    }
   

    public double GetStimulation() { return stimulation; }
}
