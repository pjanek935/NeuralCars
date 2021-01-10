
using UnityEngine;

public static class TimeScaleController 
{
    public const string TimeScaleKey = "ts";
    public static float CurrentTimeScale => Time.timeScale;

    public static float TargetTimeScale
    {
        get
        {
            if (! targetTimeScaleLoaded)
            {
                targetTimeScale = PlayerPrefs.GetFloat (TimeScaleKey, 1f);
                targetTimeScaleLoaded = true;
            }

            return targetTimeScale;
        }

        set
        {
            targetTimeScale = value;
            targetTimeScaleLoaded = true;
        }
    }

    static float targetTimeScale = 1f;
    static bool targetTimeScaleLoaded = false;

    public static void SetTimeScale (float val)
    {
        TargetTimeScale = val;
        Time.timeScale = val;
    }

    public static void SaveTimeScale ()
    {
        PlayerPrefs.SetFloat (TimeScaleKey, TargetTimeScale);
    }

    public static void SetDefaultTimeScale ()
    {
        Time.timeScale = 1f;
    }

    public static float SetTargetTimeScale ()
    {
        Time.timeScale = TargetTimeScale;

        return TargetTimeScale;
    }
}
