using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAction
{
    public int IndexInList
    {
        get;
        protected set;
    }

    public StageAction (int indexInList)
    {
        this.IndexInList = indexInList;
    }
}
