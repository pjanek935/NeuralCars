using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticsManager : MonoBehaviour
{
    [SerializeField] Transform startPosition;
    [SerializeField] GameObject carPrefab;

    List<CarNeuralCore> cars = new List<CarNeuralCore> ();
}
