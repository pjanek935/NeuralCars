using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not much here, just stores consecutive gate index. The further on track the bigger the index.
/// </summary>
public class Gate : MonoBehaviour
{
    [SerializeField] int index;

    int maxPossibleIndex;

    public int Index
    {
        set
        {
            index = value;
            setIsFinalGate ();
        }

        get
        {
            return index;
        }
    }
    public int MaxPossibleIndex
    {
        set
        {
            maxPossibleIndex = value;
            setIsFinalGate ();
        }

        get
        {
            return maxPossibleIndex;
        }
    }

    public bool IsFinalGate
    {
        get;
        private set;
    }

    public void setIsFinalGate ()
    {
        IsFinalGate = (Index == MaxPossibleIndex);
    }
}
