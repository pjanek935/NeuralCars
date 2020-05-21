using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int Index = 0;

    [SerializeField] Collider collider;

    List<Gate> isTouching = new List<Gate> ();

    public Collider Collider
    {
        get { return collider; }
    }

    public bool IsTouching (Gate gate)
    {
        return isTouching.Contains (gate);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.transform.parent != null)
        {
            Gate otherGate = other.transform.gameObject.GetComponent<Gate> ();

            if (otherGate != null)
            {
                isTouching.Add (otherGate);
            }
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.transform.parent != null)
        {
            Gate otherGate = other.transform.gameObject.GetComponent<Gate> ();

            if (otherGate != null)
            {
                isTouching.Remove (otherGate);
            }
        }
    }
}
