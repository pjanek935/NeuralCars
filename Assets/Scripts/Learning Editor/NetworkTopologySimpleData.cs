using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NetworkTopologySimpleData
{
    public bool MovementAngleInput;
    public bool VelocityInput;
    public bool TorqueInput;
    public bool SteerAngleInput;

    public bool TorqueOutput;
    public bool HandbrakeOutput;

    public int SensorsCount;
    public int HiddenLayerNeuronsCount;

    public bool IsDifferent (NetworkTopologySimpleData other)
    {
        if (other == null)
        {
            return true;
        }

        bool result = false;

        if (MovementAngleInput != other.MovementAngleInput)
        {
            result = true;
        }
        else if (VelocityInput != other.VelocityInput)
        {
            result = true;
        }
        else if (TorqueInput != other.TorqueInput)
        {
            result = true;
        }
        else if (SteerAngleInput != other.SteerAngleInput)
        {
            result = true;
        }
        else if (SensorsCount != other.SensorsCount)
        {
            result = true;
        }
        else if (HiddenLayerNeuronsCount != other.HiddenLayerNeuronsCount)
        {
            result = true;
        }
        else if (TorqueOutput != other.TorqueOutput)
        {
            result = true;
        }
        else if (HandbrakeOutput != other.HandbrakeOutput)
        {
            result = true;
        }

        return result;
    }
    
    public NetworkTopologySimpleData GetCopy ()
    {
        NetworkTopologySimpleData result = new NetworkTopologySimpleData ();

        result.MovementAngleInput = MovementAngleInput;
        result.SteerAngleInput = SteerAngleInput;
        result.TorqueInput = TorqueInput;
        result.VelocityInput = VelocityInput;

        result.HiddenLayerNeuronsCount = HiddenLayerNeuronsCount;
        result.SensorsCount = SensorsCount;

        result.HandbrakeOutput = HandbrakeOutput;
        result.TorqueOutput = TorqueOutput;

        return result;
    }
}
