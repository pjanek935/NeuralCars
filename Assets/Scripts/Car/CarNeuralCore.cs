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
    [SerializeField] float angleBetweenSensors = 15f;
    [SerializeField] float sensorsLength = 15f;

    NeuralNetwork neuralNetwork;
    int lastPassedGateIndex = 0;
    float lastGatePassedTime = 0;
    NetworkTopologySimpleData nt = new NetworkTopologySimpleData ();

    const float TIME_TO_DISABLE = 2f;
    const float MIN_AVG_VELOCITY = 2f;

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

    public void Init (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            initNeuralNetwork (networkTopology);
            carRadar.Init (networkTopology.SensorsCount, angleBetweenSensors, sensorsLength);
            IsActive = false;
        }
    }

    public void Reset ()
    {
        lastPassedGateIndex = 0;
        lastGatePassedTime = Time.time;
        isActive = false;
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

    void initNeuralNetwork (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            nt = networkTopology.GetCopy ();
            int outputCount = 1 + (nt.TorqueOutput ? 1 : 0) + (nt.HandbrakeOutput ? 1 : 0);
            int additionalInputCount = (nt.TorqueInput ? 1 : 0) + (nt.VelocityInput ? 1 : 0) +
                (nt.SteerAngleInput ? 1 : 0) + (nt.MovementAngleInput ? 1 : 0);
            neuralNetwork = new NeuralNetwork (networkTopology.SensorsCount + additionalInputCount, nt.HiddenLayerNeuronsCount, outputCount);
        }
    }

    void steerCar ()
    {
        List<double> inputList = carRadar.GetValues ();

        if (nt.MovementAngleInput)
        {
            inputList.Add (carTelemetry.GetAngleBetweenForwardAndMovementDirection (true));
        }

        if (nt.VelocityInput)
        {
            inputList.Add (carTelemetry.GetAngleBetweenForwardAndMovementDirection (true));
        }

        if (nt.TorqueInput)
        {
            inputList.Add (carController.Torque);
        }

        if (nt.SteerAngleInput)
        {
            inputList.Add (carController.SteerAngle);
        }

        double [] output = neuralNetwork.GetOutput (inputList.ToArray ());
        int currentOutputIndex = 0;

        carController.SetSteerAngle ((float) output [currentOutputIndex]);
        currentOutputIndex ++;

        if (nt.TorqueOutput)
        {
            carController.SetTorque ((float) output [currentOutputIndex]);
            currentOutputIndex++;
        }
        else
        {
            carController.SetTorque (1f);
        }

        if (nt.HandbrakeOutput)
        {
            carController.SetBrake ((float) output [currentOutputIndex] > 0f);
        }
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

            if (timeDiff > TIME_TO_DISABLE)
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
