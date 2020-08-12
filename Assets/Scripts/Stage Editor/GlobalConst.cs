using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConst
{
    //Stage Editor
    public const float MAX_NODE_WIDTH = 18f;
    public const float MIN_NODE_WIDTH = 6f;
    public const float NODE_D = 2f; //minimal incremental step when choosing node width
    public const float STAGE_RADIUS = 199f;

    //Flags
    public const float FLAG_HEIGHT = 4f; //flag object size in world space, used to calculate flag editor height based on camera position
    public const float HiddenComponentYPos = 30f;

    //Car
    public const float CAR_CENTER_OF_MASS_Y = -0.9f;
    public const float MIN_CAR_AVG_VELOCITY = 2f; //when car has avg velocity below this value it deactivates - it's to punish slow cars
    public const int MIN_GATES_PASSED_WHEN_DISABLED_BASED_ON_AVG_VELOCITY = 5; //in pair with MIN_CAR_AVG_VELOCITY; it's to let the car gain velocity
    public const float TIME_BETWEEN_GATES_TO_DISABLE = 2f; //when time between two consecutive gates exceeds this value the car is disabled

    //Tags
    public const string WALL_TAG = "Wall";
    public const string FLOOR_TAG = "Floor";
    public const string ROAD_LAYER = "Road";

    //Other
    public const float SHOW_AND_HIDE_TIME = 0.2f; //used in ui compoments; duration in seconds when animating fade in/out etc.
    public const float EPSILON = 0.01f;
    public const int INVALID_ID = -1;
    public const float MIN_AVG_FITNESS = 10f;
}
