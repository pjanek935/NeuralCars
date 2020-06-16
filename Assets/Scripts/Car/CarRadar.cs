﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRadar : MonoBehaviour
{
    [SerializeField] GameObject sensorPrefab;

    List<CarSensor> sensors = new List<CarSensor> ();

    private void Start ()
    {
        //Init (6, 15f, 15f);
    }

    public List<double> GetValues ()
    {
        List<double> result = new List<double> ();
        sensors.ForEach (s => result.Add (s.Value));

        return result;
    }

    public void Init (int sensorsCount, float angleBetweenSensors, float length)
    {
        float angle = -((sensorsCount * angleBetweenSensors) / 2f); //init start angle
        angle += angleBetweenSensors / 2f;

        for (int i = 0; i < sensorsCount; i++)
        {
            if (i >= sensors.Count)
            {
                createNewSensor ();
            }
            
            sensors [i].Init (length, angle);
            angle += angleBetweenSensors;
        }

        int diff = sensors.Count - sensorsCount;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                deleteLastSensor ();
            }
        }
    }

    public void SetSensorsVisible (bool visible)
    {
        foreach (CarSensor sensor in sensors)
        {
            sensor.SetVisible (visible);
        }
    }

    public void SetSensorsLength (float length)
    {
        foreach (CarSensor sensor in sensors)
        {
            sensor.SetLength (length);
        }
    }

    public void SetAngleBetweenSensors (float angleBetweenSensors)
    {
        float angle = -((sensors.Count * angleBetweenSensors) / 2f); //init start angle
        angle += angleBetweenSensors / 2f;

        for (int i = 0; i < sensors.Count; i++)
        {
            sensors [i].SetAngle (angle);
            angle += angleBetweenSensors;
        }
    }

    void createNewSensor ()
    {
        GameObject newObject = Instantiate (sensorPrefab);
        newObject.SetActive (true);
        newObject.transform.SetParent (this.transform, false);
        sensors.Add (newObject.GetComponent <CarSensor> ());
    }

    void deleteLastSensor ()
    {
        if (sensors.Count > 0)
        {
            GameObject tmp = sensors [sensors.Count - 1].gameObject;
            sensors.RemoveAt (sensors.Count - 1);
            Destroy (tmp);
        }
    }

    private void FixedUpdate ()
    {
        for (int i = 0; i < sensors.Count; i ++)
        {
            sensors [i].ShootRaycast ();
        }
    }
}
