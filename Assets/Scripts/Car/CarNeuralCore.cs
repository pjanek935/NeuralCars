using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarNeuralCore : MonoBehaviour, IPointerClickHandler
{
    public delegate void CarNeuralCoreEventHandler (CarNeuralCore carNeuralCore);
    public event CarNeuralCoreEventHandler OnCarDisabled;
    public event CarNeuralCoreEventHandler OnGatePassed;
    public event CarNeuralCoreEventHandler OnCarClicked;

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

    public bool DisableOnWallHit
    {
        get { return disableOnWallHit; }
        set { disableOnWallHit = value; }
    }

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
        return new CarSimpleData (GetWeights (), GetComponent< CarFitness> ().Fitness, SensorsLength, AngleBetweenSensors);
    }

    private void Awake ()
    {
        carFitness.OnWallHit += onWallHit;
        carFitness.OnGatePassed += onGatePassed;
    }

    public void Init ()
    {
        initNeuralNetwork (sensorsCount);
        carRadar.Init (sensorsCount, angleBetweenSensors, sensorsLength);
        IsActive = false;
    }

    public void Reset ()
    {
        lastPassedGateIndex = 0;
        lastGatePassedTime = Time.time;
        carFitness.Reset ();
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

    public double [] GetRandomNeuralNetworkWeights ()
    {
        return neuralNetwork.GetRandomWeights ();
    }

    void initNeuralNetwork (int sensorsCount)
    {
        neuralNetwork = new NeuralNetwork (sensorsCount + 0, neuronsInHiddenLayer, 2);
    }

    void steerCar ()
    {
        List<double> inputList = carRadar.GetValues ();

        //inputList.Add (carTelemetry.GetAngleBetweenForwardAndMovementDirection (true));
        //inputList.Add (carTelemetry.VelocityAverage.magnitude);
        //inputList.Add (carController.Torque);
        //inputList.Add (carController.SteerAngle);

        double [] output = neuralNetwork.GetOutput (inputList.ToArray ());

        carController.SetTorque (1f);
        carController.SetSteerAngle ((float) output [0]);
        carController.SetBrake ((float) output [1] > 0f);
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

    void onWallHit ()
    {
        if (disableOnWallHit)
        {
            disableCar ();
        }
    }

    void onGatePassed ()
    {
        OnGatePassed?.Invoke (this);
    }

    void disableCar ()
    {
        IsActive = false;
        carController.SetTorque (0);
        carController.SetSteerAngle (0);
        carController.SetBrake (false);
        carFitness.PosWhenDisabled = this.transform.position;

        OnCarDisabled?.Invoke (this);
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        OnCarClicked?.Invoke (this);
    }
}
