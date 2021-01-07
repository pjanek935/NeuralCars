
using UnityEngine;

public static class TimeScaleController 
{
    public const string TimeScaleKey = "ts";

    public static void SetTimeScale (float val)
    {
        Time.timeScale = val;
        PlayerPrefs.SetFloat (TimeScaleKey, val);
        Debug.Log ("Time scale set: " + val);
    }

    public static void SetDefaultTimeScale ()
    {
        Time.timeScale = 1f;
    }

    public static float SetSavedTimeScale ()
    {
        float val = PlayerPrefs.GetFloat (TimeScaleKey, 1f);
        Time.timeScale = val;

        return val;
    }
}
