using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode
{
    public Vector3 Position
    {
        get;
        set;
    }

    public float Width
    {
        get;
        set;
    }

    public StageNode (Vector3 position, float width)
    {
        this.Position = position;
        this.Width = width;
    }
}
