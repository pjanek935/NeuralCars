using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedTopologyData
{
    public NetworkTopologySimpleData TopologyData;
    public string TopologyName;
    public List<CarSimpleData> CarSimpleData = new List<CarSimpleData> ();

    public SavedTopologyData (NetworkTopologySimpleData networkTopologySimpleData, List <CarSimpleData> cars, string topologyName)
    {
        if (networkTopologySimpleData != null)
        {
            this.TopologyData = networkTopologySimpleData.GetCopy ();
        }
        else
        {
            Debug.LogError ("Null parameter");
        }

        if (cars != null)
        {
            CarSimpleData.Clear ();

            foreach (CarSimpleData car in cars)
            {
                CarSimpleData.Add (car.GetCopy ());
            }
        }
        else
        {
            Debug.LogError ("Null parameter");
        }

        if (! string.IsNullOrEmpty (topologyName))
        {
            this.TopologyName = topologyName;
        }
    }
}
