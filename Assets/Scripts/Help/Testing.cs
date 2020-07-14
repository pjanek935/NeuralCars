using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] Stage stage;
    [SerializeField] Transform car;

    private void Update ()
    {
        if (car != null && stage != null)
        {
            Debug.Log ("Dist: " + stage.GetDistanceFromBeginning (car.position));
        }
    }
}
