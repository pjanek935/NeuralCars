using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedTopologyData
{
    public CarSimpleData BestCar;
    public NetworkTopologySimpleData TopologyData;
    public string TopologyName;

    public SavedTopologyData (CarSimpleData bestCar, NetworkTopologySimpleData networkTopologySimpleData, string topologyName)
    {
        if (bestCar != null)
        {
            this.BestCar = bestCar.GetCopy ();
        }
        else
        {
            Debug.LogError ("Null parameter");
        }

        if (networkTopologySimpleData != null)
        {
            this.TopologyData = networkTopologySimpleData.GetCopy ();
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
