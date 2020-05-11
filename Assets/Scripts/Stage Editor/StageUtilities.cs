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

    public static float DistToVector (Vector3 vectorPointA, Vector3 vectorPointB, Vector3 p)
    {
        Vector3 d = (vectorPointB - vectorPointA) / Vector3.Distance (vectorPointB, vectorPointA);
        Vector3 v = p - vectorPointA;
        float t = Vector3.Dot (v, d);
        Vector3 P = vectorPointA + t * d;
        return Vector3.Distance (P, p);
    }

    public static Vector3 FindClosestPointOnLineSegment (Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 line = lineEnd - lineStart;
        Vector3 dir = point - lineStart;
        float d = Vector3.Dot (line, dir) / line.sqrMagnitude;
        d = Mathf.Clamp01 (d);
        return Vector3.Lerp (lineStart, lineEnd, d);
    }

    public static float DistToLineSegment (Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 closestPoint = FindClosestPointOnLineSegment (lineStart, lineEnd, point);
        return Vector3.Distance (point, closestPoint);
    }
}
