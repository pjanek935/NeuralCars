using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageNode
{
    public Vector3 Position;
    public float Width;

    public StageNode (Vector3 position, float width)
    {
        this.Position = position;
        this.Width = width;
    }
}
