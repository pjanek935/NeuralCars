using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Curver : MonoBehaviour
{
    //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static List <Vector3> MakeSmoothCurve (List <Vector3> pointsToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = pointsToCurve.Count;

        curvedLength = (pointsLength * Mathf.RoundToInt (smoothness)) - 1;
        curvedPoints = new List<Vector3> (curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp (0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3> (pointsToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points [i] = (1 - t) * points [i] + t * points [i + 1];
                }
            }

            curvedPoints.Add (points [0]);
        }

        return curvedPoints;
    }

    public static Vector3 cubeBezier3 (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return (((-p0 + 3 * (p1 - p2) + p3) * t + (3 * (p0 + p2) - 6 * p1)) * t + 3 * (p1 - p0)) * t + p0;
    }
}