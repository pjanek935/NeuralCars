using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnExitCleaner : MonoBehaviour
{
    private void OnDisable ()
    {
        TimeScaleController.SaveTimeScale ();
    }
}
