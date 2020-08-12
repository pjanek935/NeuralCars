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

    private void FixedUpdate ()
    {
        if (IsActive)
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
            IsActive = ! IsActive;
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
