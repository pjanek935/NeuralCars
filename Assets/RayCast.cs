using UnityEngine;
using System.Collections;

public class RayCast : MonoBehaviour {

    private double stimulation;
    public GameObject parentObject;
    public GameObject point;
    public GameObject start;
    public GameObject end;
    public string colliderName = "Wall";

    private int layerMask;
    private float length;

    void Start()
    {
        length = Vector3.Distance(start.transform.position, end.transform.position);
        layerMask = LayerMask.GetMask(colliderName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void ShootRayCast()
    {
        //int layerMask = LayerMask.GetMask(colliderName);
        Vector3 direction = end.transform.position - start.transform.position;
        //float length = Vector3.Distance(start.transform.position, end.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(start.transform.position, direction, out hit, length, layerMask))
        {
            point.transform.position = hit.point;
            stimulation = 1 - Vector3.Distance(start.transform.position, hit.point) / length;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == colliderName)
            ShootRayCast();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == colliderName)
            ShootRayCast();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == colliderName)
            stimulation = 0;
    }


    public double GetStimulation() { return stimulation; }
}
