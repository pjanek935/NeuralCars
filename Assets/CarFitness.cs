using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarFitness : Fitness {

    private float timer = 0;
    private float resetLimiter = 5;
    private List<int> gatesIds;

    private int hitCounter = 0;
    public int maxHits = 1;
    private float maxImpactForce = 1;
    private Core core;

    // Use this for initialization
    void Start () {
        Init();
        gatesIds = new List<int>();
        core = GetComponent<Core>();
    }
	
	// Update is called once per frame
	void Update () {
        if (IsActive()) timer += Time.deltaTime;
        if (timer >= resetLimiter && core.type == Core.Type.GENETICS)
        {
            TurnOff();
            hitCounter = 0;
            timer = 0;
            Debug.Log("!!");
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (!stopOnHit) return;

        if (other.gameObject.tag == "Wall")
        {
            //Vector3 collisionNormal = other.contacts[0].normal;
            //Vector3 velocity = GetComponent<Rigidbody>().velocity;
            //float impact = Vector3.Dot(collisionNormal, velocity);

            //if (Mathf.Abs(impact) > maxImpactForce)
            //{
            //    hitCounter++;
            //    if (hitCounter >= maxHits)
            //    {
            //        hitCounter = 0;
            //        TurnOff();
            //    }   
            //}

            //hitCounter++;
            //if (hitCounter >= maxHits)
            //{
            //    hitCounter = 0;

            //}

            TurnOff();
        }
           
    }

    public void IncreaseFitness(float amount, int gateId)
    {
        foreach (int id in gatesIds)
        {
            if (id == gateId)
            {
                return;
            }
        }

        IncreaseFitness(amount);
        timer = 0;
        gatesIds.Add(gateId);

        //timer = 0;
        //IncreaseFitness(amount);
    }

    public override void Reset()
    {
        base.Reset();
        gatesIds.Clear();
        gatesIds = new List<int>();
        timer = 0;
        
    }
}
