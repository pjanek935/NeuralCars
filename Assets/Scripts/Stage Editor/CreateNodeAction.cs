using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNodeAction : StageAction
{
    public Vector3 Position
    {
        get;
        private set;
    }

    public float Width
    {
        get;
        private set;
    }

    public CreateNodeAction (int indexInList, Vector3 position, float width) : base (indexInList)
    {
        this.Position = position;
        this.Width = width;
    }
}
