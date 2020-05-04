using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectibleFitness : Fitness {

    

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public override void IncreaseFitness(float amount)
    {
        base.IncreaseFitness(amount);
        Color color = GetComponent<Renderer>().material.color;
        color.g += 0.05f;
        color.b -= 0.05f;
        GetComponent<Renderer>().material.color = color;
    }


}
