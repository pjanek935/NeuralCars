using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNodeAction : StageAction
{
    public Vector3 From
    {
        get;
        private set;
    }

    public Vector3 To
    {
        get;
        private set;
    }

    public MoveNodeAction (int indexInList, Vector3 from, Vector3 to) : base (indexInList)
    {
        this.From = from;
        this.To = to;
    }
}
