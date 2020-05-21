using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNeuralCore : MonoBehaviour
{
    [SerializeField] CarRadar carRadar;
    [SerializeField] CarController carController;

    const int neuronsInHiddenLayer = 5;

    NeuralNetwork neuralNetwork;

    public bool IsActive
    {
        get;
        private set;
    }

    void initNeuralNetwork (int sensorsCount)
    {
        neuralNetwork = new NeuralNetwork (sensorsCount, neuronsInHiddenLayer, 2);
    }

    private void Start ()
    {
        initNeuralNetwork (7);
        carRadar.Init (7, 10f, 15f);
        IsActive = true;
    }

    void steerCar ()
    {
        double [] input = carRadar.GetValues ().ToArray ();
        double [] output = neuralNetwork.GetOutput (input);

        carController.SetTorque ((float) output [0]);
        carController.SetSteerAngle ((float) output [1]);
    }

    private void FixedUpdate ()
    {
        //tmp ();
    }
}
