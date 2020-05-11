using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWidthAction : StageAction
{
    public float From
    {
        get;
        private set;
    }

    public float To
    {
        get;
        private set;
    }

    public ChangeWidthAction (int indexInList, float from, float to) : base (indexInList)
    {
        this.From = from;
        this.To = to;
    }
}
