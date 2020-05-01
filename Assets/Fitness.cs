using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fitness : MonoBehaviour {

    public float fitness;
    public bool stopOnHit = true;

    private bool active = true;
    protected Core core;

    protected List<int> collectedIds;

    // Use this for initialization
    void Start () {
        
    }

    protected void Init()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(0.1f, 0.1f, 1f);
        core = GetComponent<Core>();
        collectedIds = new List<int>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetActive(bool active) { this.active = active; }
    public bool IsActive() { return active; }
    public virtual void IncreaseFitness(float amount) { fitness += amount; }

    public virtual void Reset()
    {
        active = true;
        fitness = 0;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(0.1f, 0.1f, 1f);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        core.neuralControlled = true;
        
    }

    public void TurnOff()
    {
        if (!stopOnHit) return;
        core.neuralControlled = false;
        active = false;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(1f, 0.2f, 0.1f);
    }

    public float GetFitness() { return fitness; }

    
}
