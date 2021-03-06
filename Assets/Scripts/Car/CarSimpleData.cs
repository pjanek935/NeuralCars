﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarSimpleData
{
    public double [] Weights;
    public double Fitness = 0f;
    public float DistTravelled = 0f;
    public float SensorsLength = 0f;
    public float AngleBetweenSensors = 0f;

    public CarSimpleData (double [] weights, double fitness, float distTravelled, float sensorsLength, float angleBetweenSensors)
    {
        Fitness = fitness;
        SensorsLength = sensorsLength;
        AngleBetweenSensors = angleBetweenSensors;
        DistTravelled = distTravelled;

        if (weights != null)
        {
            Weights = new double [weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                Weights [i] = weights [i];
            }
        }
    }

    public CarSimpleData ()
    {

    }

    public CarSimpleData GetCopy ()
    {
        return new CarSimpleData (Weights, Fitness, DistTravelled, SensorsLength, AngleBetweenSensors);
    }
}
