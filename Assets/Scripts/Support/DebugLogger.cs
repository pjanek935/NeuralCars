using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionStatus
{
    NullNeighbourList,
    NullNodesList,
    IndexOutOfRange,
    NullParam,
    NodeAlreadyAdded,
    NodeDoesNotExist
}

public class DebugLogger : MonoBehaviour
{
    static DebugLogger instance;

    public static DebugLogger Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject newDebugLoggerObject = new GameObject ();
                instance = newDebugLoggerObject.AddComponent<DebugLogger> ();
            }

            return instance;
        }
    }

    private void Awake ()
    {
        if (instance != this)
        {
            Destroy (this);
        }
    }

    public static void Log (string message)
    {
        Debug.Log (message);
    }

    public static void Log (ActionStatus actionStatus)
    {
        string message = ActionStatusToMessage (actionStatus);

        if (string.IsNullOrEmpty (message))
        {
            message = actionStatus.ToString ();
        }

        Log (message);
    }

    public static string ActionStatusToMessage (ActionStatus actionStatus)
    {
        string result = string.Empty;

        switch (actionStatus)
        {
            case ActionStatus.IndexOutOfRange:

                result = "Index do którego się odwołujesz jest poza zakresem!";

                break;

            case ActionStatus.NullNeighbourList:

                result = "Lista sąsiedztwa węzła jest nullem!";

                break;

            case ActionStatus.NullNodesList:

                result = "Lista węzłów jest nullem!";

                break;

            case ActionStatus.NullParam:

                result = "Podano null jako parametr!";

                break;

            case ActionStatus.NodeAlreadyAdded:

                result = "Ten węzeł został już dodany!";

                break;
        }

        return result;
    }
}
