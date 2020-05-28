using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageConsts
{
    public const float MaxNodeWidth = 18f;
    public const float MinNodeWidth = 6f;
    public const float NodeD = 2f;

    public const float FlagHeight = 4f; //flag object size in world space, used to calculate fag editor height based on camera position
    public const float ShowAndHideTime = 0.2f;
    public const float HiddenComponentYPos = 30f;

    public const float Epsilon = 0.01f;
}
