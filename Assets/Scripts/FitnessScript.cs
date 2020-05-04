using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FitnessScript : MonoBehaviour {

    private float fitness = 0;
    private float selectionProbability = 0;
    private bool crashed = false;
    private bool reachedFinalGate = false;
    private Core nn;
    private CarControlScipt cc;
    private float timer = 0;
    private float resetLimiter = 5;
    private List<int> gatesIds;

	// Use this for initialization
	void Start () {
        nn = GetComponent<Core>();
        cc = GetComponent<CarControlScipt>();
        gatesIds = new List<int>();

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(0.1f, 0.1f, 1f);
    }
	
	// Update is called once per frame
	void Update () {

        if(!crashed) timer += Time.deltaTime;
        if (timer >= resetLimiter)
        {
            nn.neuralControlled = false;
            crashed = true;
            //Debug.Log("Crashed due to time out");
            timer = 0;
        }
	}

    public void IncreaseFitness(float amount, int gateId)
    {
        //bool foundGate = false;
        //foreach (int id in gatesIds)
        //{
        //    if (id == gateId)
        //    {
        //        foundGate = true;
        //        break;
        //    }
        //}
        //if (!foundGate)
        //{
        //    fitness += amount;
        //    timer = 0;
        //    gatesIds.Add(gateId);
        //}

        foreach (int id in gatesIds) if (id == gateId) return;

        //Debug.Log("started incrasing fitness");
        fitness += amount;
        timer = 0;
        gatesIds.Add(gateId);
        //Debug.Log("stopped incrasing fitness");
        //fitness += amount;
        //timer = 0;

    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            nn.neuralControlled = false;
            crashed = true;
            //Debug.Log("Crashed due to wall hit");

            Renderer renderer = GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.2f, 0.1f);
        }

        if (other.gameObject.tag == "FinalGate")
        {
            nn.neuralControlled = false;
            crashed = true;
            reachedFinalGate = true;
        }

    }

    public void Reset()
    {
        crashed = false;
        fitness = 0;
        //nn.neuralControled = true;
        //nn.ResetWeights();
        cc.Move(0);
        cc.Steer(0);
        gatesIds.Clear();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(0.1f, 0.1f, 1f);
    }

    public void StartCar()
    {
        nn.neuralControlled = true;
        cc.Move(0);
        cc.Steer(0);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }

    public bool IsCrashed() { return crashed; }

    public float GetFitness() { return fitness; }

    public bool ReachedFinalGate() { return reachedFinalGate; }



}
