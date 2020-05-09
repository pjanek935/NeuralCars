using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageUtilities
{
    public static Vector3 PerpendicularClockwise (Vector3 lineSegmentStart, Vector3 lineSegmentEnd)
    {
        Vector3 direction = lineSegmentEnd - lineSegmentStart;
        return new Vector3 (direction.z, 0, -direction.x);
    }

    public static Vector3 PerpendicularCounterClockwise (Vector3 lineSegmentStart, Vector3 lineSegmentEnd)
    {
        Vector3 direction = lineSegmentEnd - lineSegmentStart;
        return new Vector3 (-direction.z, 0, direction.x);
    }

    // Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
    // intersect the intersection point may be stored in the floats i_x and i_y.
    public static bool GetLineSegmentIntersectionPoint (Vector3 p0, Vector3 p1,
        Vector3 p2, Vector3 p3, out Vector3 intersectionPoint)
    {
        bool result = false;
        intersectionPoint = new Vector3 ();

        Vector3 s1, s2;
        s1.x = p1.x - p0.x;
        s1.z = p1.z - p0.z;
        s2.x = p3.x - p2.x;
        s2.z = p3.z - p2.z;

        float s, t;

        s = (-s1.z * (p0.x - p2.x) + s1.x * (p0.z - p2.z)) / (-s2.x * s1.z + s1.x * s2.z);
        t = (s2.x * (p0.z - p2.z) - s2.z * (p0.x - p2.x)) / (-s2.x * s1.z + s1.x * s2.z);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        { // Collision detected
            intersectionPoint.x = p0.x + (t * s1.x);
            intersectionPoint.z = p0.z + (t * s1.z);

            result = true;
        }

        return result;
    }
}
