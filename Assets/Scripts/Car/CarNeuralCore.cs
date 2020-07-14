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

    NeuralNetwork neuralNetwork;
    int lastPassedGateIndex = 0;
    float lastGatePassedTime = 0;
    NetworkTopologySimpleData networkTopology = new NetworkTopologySimpleData ();

    const float TIME_TO_DISABLE = 2f;
    const float MIN_AVG_VELOCITY = 2f;

    public bool DisableOnWallHit
    {
        get;
        set;
    }

    public bool IsActive
    {
        get;
        set;
    }

    public float AngleBetweenSensors
    {
        get;
        set;
    }

    public float SensorsLength
    {
        get;
        set;
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

    public void Init (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            initNeuralNetwork (networkTopology);
            carRadar.Init (networkTopology.SensorsCount, AngleBetweenSensors, SensorsLength);
            IsActive = false;
        }
    }

    public void Reset ()
    {
        lastPassedGateIndex = 0;
        lastGatePassedTime = Time.time;
        IsActive = false;
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
        this.SensorsLength = length;
        carRadar.SetSensorsLength (length);
    }

    public void SetAngleBetweenSensors (float angle)
    {
        this.AngleBetweenSensors = angle;
        carRadar.SetAngleBetweenSensors (angle);
    }

    public double [] GetRandomNeuralNetworkWeights ()
    {
        return neuralNetwork.GetRandomWeights ();
    }

    void initNeuralNetwork (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            this.networkTopology = networkTopology.GetCopy ();
            int outputCount = 1 + (this.networkTopology.TorqueOutput ? 1 : 0) + (this.networkTopology.HandbrakeOutput ? 1 : 0);
            int additionalInputCount = (this.networkTopology.TorqueInput ? 1 : 0) + (this.networkTopology.VelocityInput ? 1 : 0) +
                (this.networkTopology.SteerAngleInput ? 1 : 0) + (this.networkTopology.MovementAngleInput ? 1 : 0);
            neuralNetwork = new NeuralNetwork (networkTopology.SensorsCount + additionalInputCount, this.networkTopology.HiddenLayerNeuronsCount, outputCount);
        }
    }

    void steerCarBasedOnNeuralNetworkOutput ()
    {
        List<double> inputList = carRadar.GetValues ();

        if (networkTopology.MovementAngleInput)
        {
            inputList.Add (carTelemetry.GetAngleBetweenForwardAndMovementDirection (true));
        }

        if (networkTopology.VelocityInput)
        {
            inputList.Add (carTelemetry.GetAngleBetweenForwardAndMovementDirection (true));
        }

        if (networkTopology.TorqueInput)
        {
            inputList.Add (carController.Torque);
        }

        if (networkTopology.SteerAngleInput)
        {
            inputList.Add (carController.SteerAngle);
        }

        double [] output = neuralNetwork.GetOutput (inputList.ToArray ());
        int currentOutputIndex = 0;

        carController.SetSteerAngle ((float) output [currentOutputIndex]);
        currentOutputIndex ++;

        if (networkTopology.TorqueOutput)
        {
            carController.SetTorque ((float) output [currentOutputIndex]);
            currentOutputIndex++;
        }
        else
        {
            carController.SetTorque (1f);
        }

        if (networkTopology.HandbrakeOutput)
        {
            carController.SetBrake ((float) output [currentOutputIndex] > 0f);
        }
    }

    private void FixedUpdate ()
    {
        if (IsActive)
        {
            steerCarBasedOnNeuralNetworkOutput ();
            checkIfCarShouldBeDisabled ();
        }
    }

    void checkIfCarShouldBeDisabled ()
    {
        if (carFitness.GatesPassed > lastPassedGateIndex)
        {
            lastPassedGateIndex = carFitness.GatesPassed;
            lastGatePassedTime = Time.time;
        }
        else
        {
            float timeDiff = Time.time - lastGatePassedTime;

            if (timeDiff > TIME_TO_DISABLE)
            {
                disableCar ();
            }
        }
    }

    void onWallHit ()
    {
        if (DisableOnWallHit)
        {
            disableCar ();
        }
    }

    void onGatePassed ()
    {
        OnGatePassed?.Invoke (this);

        if (carFitness.GatesPassed > 5 && carFitness.AvgVelocity < MIN_AVG_VELOCITY)
        {
            disableCar ();
        }
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
