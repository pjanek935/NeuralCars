using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class is controlls the car based on neural network output.
/// </summary>
public class CarNeuralCore : CarNeuralCoreBase
{
    static System.Random rand = new System.Random ();

    public event CarNeuralCoreEventHandler OnCarDisabled;
    public event CarNeuralCoreEventHandler OnGatePassed;

    int lastPassedGateIndex = 0;
    float lastGatePassedTime = 0;
    int parity = 0; //not all cars refresh their state in the same frame (performance reasons). Car refreshes state if Time.frameCount % 2 == partity;

    public bool DisableOnWallHit
    {
        get;
        set;
    }

    private void OnEnable ()
    {
        parity = rand.NextDouble () > 0.5 ? 1 : 0;
    }

    public void Reset ()
    {
        lastPassedGateIndex = 0;
        lastGatePassedTime = Time.time;
        IsActive = false;
        carFitness.Reset ();
    }

    public override void Init (NetworkTopologySimpleData networkTopology)
    {
        base.Init (networkTopology);
        IsActive = false;
    }

    public double [] GetRandomNeuralNetworkWeights ()
    {
        return neuralNetwork.GetRandomWeights ();
    }

    void steerCarBasedOnNeuralNetworkOutput ()
    {
        carRadar.ShootRayCasts ();
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
            if (Time.frameCount % 2 == parity)
            {
                steerCarBasedOnNeuralNetworkOutput ();
            }
            
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

            if (timeDiff > GlobalConst.TIME_BETWEEN_GATES_TO_DISABLE)
            {
                disableCar ();
            }
        }
    }

    protected override void onWallHit ()
    {
        if (DisableOnWallHit)
        {
            disableCar ();
        }
    }

    protected override void onGatePassed (int gateIndex)
    {
        //OnGatePassed?.Invoke (this);

        //if (carFitness.GatesPassed > GlobalConst.MIN_GATES_PASSED_WHEN_DISABLED_BASED_ON_AVG_VELOCITY
        //    && carFitness.AvgVelocity < GlobalConst.MIN_CAR_AVG_VELOCITY)
        //{
        //    disableCar ();
        //}
    }

    void disableCar ()
    {
        IsActive = false;
        carController.SetTorque (0);
        carController.SetSteerAngle (0);
        carController.SetBrake (false);
        carFitness.PosWhenDisabled = this.transform.position;
        carFitness.RotationWhenDisabled = this.transform.rotation;
        carRadar.Disable ();

        OnCarDisabled?.Invoke (this);
    }
}
