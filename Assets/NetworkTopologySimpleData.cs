using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTopologySimpleData
{
    public bool MovementAngleInput;
    public bool VelocityInput;
    public bool TorqueInput;
    public bool SteerAngleInput;

    public bool TorqueOutput;
    public bool SteerAngleOutput;
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
        else if (SteerAngleOutput != other.SteerAngleOutput)
        {
            result = true;
        }
        else if (HandbrakeOutput != other.HandbrakeOutput)
        {
            result = true;
        }

        return result;
    }
}
