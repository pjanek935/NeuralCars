using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTimeline
{
    List<StageAction> actions = new List<StageAction> ();
    int lastActionIndex = GlobalConst.INVALID_ID;

    public void AddAction (StageAction stageAction)
    {
        if (lastActionIndex == actions.Count - 1)
        {
            actions.Add (stageAction);
            lastActionIndex = actions.Count - 1;
        }
        else
        {
            if (lastActionIndex < 0)
            {
                actions.Clear ();
            }
            else
            {
                actions.RemoveRange (lastActionIndex, actions.Count - lastActionIndex);
            }
            
            actions.Add (stageAction);
            lastActionIndex = actions.Count - 1;
        }
    }
        

    public StageAction MakeOneStepBack ()
    {
        StageAction result = null;

        if (lastActionIndex >= 0)
        {
            result = actions [lastActionIndex];
            lastActionIndex--;
        }
        
        return result;
    }

    public StageAction MakeOneStepForward ()
    {
        StageAction result = null;

        if (lastActionIndex < actions.Count - 1)
        {
            lastActionIndex++;
            result = actions [lastActionIndex];
        }

        return result;
    }

    public bool CanMakeOneStepBack ()
    {
        return lastActionIndex >= 0;
    }

    public bool CanMakeOneStepForward ()
    {
        return lastActionIndex < actions.Count - 1;
    }

}
