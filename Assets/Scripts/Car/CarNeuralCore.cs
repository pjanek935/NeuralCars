using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNeuralCore : MonoBehaviour
{
    public delegate void CarNeuralCoreEventHandler (CarNeuralCore carNeuralCore);
    public event CarNeuralCoreEventHandler OnCarDisabled;

    [SerializeField] CarRadar carRadar;
    [SerializeField] CarController carController;
    [SerializeField] CarFitness carFitness;
    [SerializeField] CarTelemetry carTelemetry;

    [SerializeField] bool disableOnWallHit;
    [SerializeField] bool isActive = false;
    [SerializeField] int neuronsInHiddenLayer = 1;
    [SerializeField] int sensorsCount = 7;
    [SerializeField] float angleBetweenSensors = 15f;
    [SerializeField] float sensorsLength = 15f;

    NeuralNetwork neuralNetwork;
    int lastPassedGateIndex = 0;
    float lastGatePassedTime = 0;

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    public int NeuronsInHiddenLayer
    {
        get { return neuronsInHiddenLayer; }
        set { neuronsInHiddenLayer = value; }
    }

    public int SensorsCount
    {
        get { return sensorsCount; }
        set { sensorsCount = value; }
    }

    public float AngleBetweenSensors
    {
        get { return angleBetweenSensors; }
        set { angleBetweenSensors = value; }
    }

    public float SensorsLength
    {
        get { return sensorsLength; }
        set { sensorsLength = value; }
    }

    public CarSimpleData GetCarSimpleData ()
    {
        return new CarSimpleData (GetWeights (), GetComponent< CarFitness> ().GetFitness (), SensorsLength, AngleBetweenSensors);
    }

    private void Awake ()
    {
        carFitness.OnWallHit += disableCar;
    }

    public void Init ()
    {
        initNeuralNetwork (sensorsCount);
        carRadar.Init (sensorsCount, angleBetweenSensors, sensorsLength);
        IsActive = true;
    }

    public void Reset ()
    {
        lastPassedGateIndex = 0;
        lastGatePassedTime = Time.time;
        carFitness.Reset ();
        isActive = true;
    }

    public double [] GetWeights ()
    {
        return neuralNetwork.GetWeights ();
    }

    public void SetWeights (double [] weights)
    {
        neuralNetwork.SetWeights (weights);
    }

    public void SetSensorsVisible (bool visible)
    {
        carRadar.SetSensorsVisible (visible);
    }

    public void SetSensorsLength (float length)
    {
        this.sensorsLength = length;
        carRadar.SetSensorsLength (length);
    }

    public void SetAngleBetweenSensors (float angle)
    {
        this.angleBetweenSensors = angle;
        carRadar.SetAngleBetweenSensors (angle);
    }

    void initNeuralNetwork (int sensorsCount)
    {
        neuralNetwork = new NeuralNetwork (sensorsCount + 1, neuronsInHiddenLayer, 3);
    }

    void steerCar ()
    {
        List<double> inputList = carRadar.GetValues ();
        inputList.Add (carTelemetry.GetAngleBetweenForwardAndMovementDirection ());
        double [] output = neuralNetwork.GetOutput (inputList.ToArray ());

        carController.SetTorque ((float) output [0]);
        carController.SetSteerAngle ((float) output [1]);
        carController.SetBrake ((float) output [2]);
    }

    private void FixedUpdate ()
    {
        if (IsActive)
        {
            steerCar ();
            checkIfShouldBeDisabled ();
        }
    }

    void checkIfShouldBeDisabled ()
    {
        if (carFitness.GatesPassed > lastPassedGateIndex)
        {
            lastPassedGateIndex = carFitness.GatesPassed;
            lastGatePassedTime = Time.time;
        }
        else
        {
            float timeDiff = Time.time - lastGatePassedTime;

            if (timeDiff > 2f)
            {
                disableCar ();
            }
        }
    }

    void disableCar ()
    {
        IsActive = false;
        OnCarDisabled?.Invoke (this);
    }
}
