using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFitness : MonoBehaviour
{
    [SerializeField] CarNeuralCore carNeuralCore;

    //public int GatesPassed
    //{
    //    get;
    //    private set;
    //}

    public int GatesPassed;

    public void Reset ()
    {
        GatesPassed = 0;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.transform.parent != null)
        {
            Gate gate = other.transform.GetComponent<Gate> ();

            if (gate != null)
            {
                if (gate.Index > GatesPassed)
                {
                    GatesPassed = gate.Index;
                }
            }
        } 
    }
}
