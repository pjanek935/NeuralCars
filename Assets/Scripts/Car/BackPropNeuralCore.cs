using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPropNeuralCore : MonoBehaviour
{
    [SerializeField] CarTelemetry carTelemetry;
    [SerializeField] CarRadar carRadar;
    [SerializeField] CarController carController;

    NeuralNetwork neuralNetwork;
    int lastPassedGateIndex = 0;
    float lastGatePassedTime = 0;
    NetworkTopologySimpleData networkTopology = new NetworkTopologySimpleData ();

    List<double []> trainingData = new List<double []> ();

    bool collectData = false;

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
        CarFitness carFitness = GetComponent<CarFitness> ();
        return new CarSimpleData (GetWeights (), carFitness.Fitness, carFitness.DistanceTravelled, SensorsLength, AngleBetweenSensors);
    }

    public double [] GetWeights ()
    {
        return neuralNetwork.GetWeights ();
    }

    public void Init (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            initNeuralNetwork (networkTopology);
            carRadar.Init (networkTopology.SensorsCount, AngleBetweenSensors, SensorsLength);
        }
    }

    public void SetSensorsVisible (bool visible)
    {
        carRadar.SetSensorsVisible (visible);
    }

    void initNeuralNetwork (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            this.networkTopology = networkTopology.GetCopy ();
            int outputCount = 1 + (this.networkTopology.TorqueOutput ? 1 : 0) + (this.networkTopology.HandbrakeOutput ? 1 : 0); //there is always one output - steer angle
            int additionalInputCount = (this.networkTopology.TorqueInput ? 1 : 0) + (this.networkTopology.VelocityInput ? 1 : 0) +
                (this.networkTopology.SteerAngleInput ? 1 : 0) + (this.networkTopology.MovementAngleInput ? 1 : 0);
            neuralNetwork = new NeuralNetwork (networkTopology.SensorsCount + additionalInputCount, this.networkTopology.HiddenLayerNeuronsCount, outputCount);
        }
    }

    private void FixedUpdate ()
    {
        if (collectData)
        {
            double [] input = getInputForNeuralNetwork ();
            double [] output = getCurrentOutput ();
            double [] data = new double [input.Length + output.Length];

            for (int i = 0; i < input.Length; i ++)
            {
                data [i] = input [i];
            }

            for (int i = 0; i < output.Length; i ++)
            {
                data [input.Length + i] = output [i];
            }

            trainingData.Add (data);
        }
    }

    private void Update ()
    {
         if (Input.GetKeyDown (KeyCode.KeypadEnter))
        {
            collectData = !collectData;
        }
    }

    double [] getInputForNeuralNetwork ()
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

        return inputList.ToArray ();
    }

    double [] getCurrentOutput ()
    {
        List<double> outputList = new List<double> ();
        outputList.Add (carController.SteerAngle);

        if (networkTopology.TorqueOutput)
        {
            outputList.Add (carController.Torque);
        }

        if (networkTopology.HandbrakeOutput)
        {
            outputList.Add (carController.IsHandbrakeOn ? 1f : 0f);
        }

        return outputList.ToArray ();
    }
}
