using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackPropNeuralCore : CarNeuralCoreBase
{
    public UnityAction OnLastGatePassed;

    [SerializeField] Stage stage;

    List<double []> trainingData = new List<double []> ();

    public List <double []> TrainingData
    {
        get { return trainingData; }
    }

    public void Reset ()
    {
        trainingData.Clear ();
        GetComponent<CarFitness> ().Reset ();
        IsActive = false;
    }

    public void Train (List <double []> trainDataList)
    {
        if (trainDataList != null)
        {
            double[][] trainData = new double [trainDataList.Count] [];

            for (int i = 0; i < trainDataList.Count; i ++)
            {
                trainData [i] = trainDataList [i];
            }

            neuralNetwork.SetWeights (neuralNetwork.GetRandomWeights ());
            double [] weights = neuralNetwork.Train (trainData, 1, 0.2f, 0f);
        }
    }

    private void FixedUpdate ()
    {
        if (IsActive)
        {
            carRadar.ShootRayCasts ();
            double [] output = getCurrentOutput ();
            bool isAnyOutputActive = false;

            for (int i = 0; i < output.Length; i ++)
            {
                if (Mathf.Abs ((float) output [i]) >= 0.01f)
                {
                    isAnyOutputActive = true;

                    break;
                }
            }

            if (isAnyOutputActive)
            {
                double [] input = getInputForNeuralNetwork ();
                double [] data = new double [input.Length + output.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    data [i] = input [i];
                }

                for (int i = 0; i < output.Length; i++)
                {
                    data [input.Length + i] = output [i];
                }

                trainingData.Add (data);
            }
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

    protected override void onGatePassed (int gateIndex)
    {
        if (!IsActive)
        {
            IsActive = true;
        }
        else if (IsActive && gateIndex == stage.GetLastGateIndex ())
        {
            IsActive = false;

            OnLastGatePassed?.Invoke ();
        }
    }
}
