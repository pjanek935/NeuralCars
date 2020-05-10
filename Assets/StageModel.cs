using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageModel
{
    public List <StageNode> Nodes
    {
        get;
        private set;
    }

    public List<Vector3> PointsRight
    {
        get;
        private set;
    }

    public List<Vector3> PointsLeft
    {
        get;
        private set;
    }

    const float epsilon = 0.01f;

    public StageModel ()
    {
        Nodes = new List<StageNode> ();
        PointsLeft = new List<Vector3> ();
        PointsRight = new List<Vector3> ();
    }

    public void SetNodes (List <StageNode> nodes, float bezierCurveFactor)
    {
        if (nodes != null)
        {
            this.Nodes = nodes;
            refreshPointsRightAndLeft (bezierCurveFactor);
        }
    }

    void refreshPointsRightAndLeft (float bezierCurveFactor)
    {
        PointsRight.Clear ();
        PointsLeft.Clear ();
        List<Vector3> pointsRightTmp = new List<Vector3> ();
        List<Vector3> pointsLeftTmp = new List<Vector3> ();
        Vector3 direction;
        Vector3 perpenRightDirection;
        Vector3 perpenLeftDirection;

        for (int i = 0; i < Nodes.Count - 1; i += 1)
        {
            direction = Nodes [i].Position - Nodes [i + 1].Position;
            direction.Normalize ();

            perpenRightDirection = StageUtilities.PerpendicularCounterClockwise (Nodes [i].Position, Nodes [i + 1].Position);
            perpenLeftDirection = StageUtilities.PerpendicularClockwise (Nodes [i].Position, Nodes [i + 1].Position);
            perpenRightDirection.Normalize ();
            perpenLeftDirection.Normalize ();

            pointsRightTmp.Add (Nodes [i].Position + perpenRightDirection * Nodes [i].Width);
            pointsLeftTmp.Add (Nodes [i].Position + perpenLeftDirection * Nodes [i].Width);

            pointsRightTmp.Add (Nodes [i + 1].Position + perpenRightDirection * Nodes [i + 1].Width);
            pointsLeftTmp.Add (Nodes [i + 1].Position + perpenLeftDirection * Nodes [i + 1].Width);
        }

        pointsRightTmp = shortenIntersectingLineSegments (pointsRightTmp);
        pointsLeftTmp = shortenIntersectingLineSegments (pointsLeftTmp);

        pointsRightTmp = interpolate (pointsRightTmp, bezierCurveFactor);
        pointsLeftTmp = interpolate (pointsLeftTmp, bezierCurveFactor);

        PointsRight = pointsRightTmp;
        PointsLeft = pointsLeftTmp;
    }

    List<Vector3> shortenIntersectingLineSegments (List <Vector3> points)
    {
        List<Vector3> result = new List<Vector3> ();

        if (points.Count >= 4)
        {
            for (int i = 0; i < points.Count - 3; i += 2)
            {
                Vector3 p0 = points [i];
                Vector3 p1 = points [i + 1];
                Vector3 p2 = points [i + 2];
                Vector3 p3 = points [i + 3];

                Vector3 intersectionPoint = Vector3.zero;

                if (StageUtilities.GetLineSegmentIntersectionPoint (p0, p1, p2, p3, out intersectionPoint))
                {
                    result.Add (p0);
                    result.Add (intersectionPoint);

                    points [i + 1] = intersectionPoint;
                    points [i + 2] = intersectionPoint;
                }
                else
                {
                    result.Add (p0);
                    result.Add (p1);
                }
            }

            result.Add (points [points.Count - 2]);
            result.Add (points [points.Count - 1]);
        }
        else
        {
            points.ForEach (p => result.Add (p));
        }

        return result;
    }

    List<Vector3> interpolate (List<Vector3> points, float bezierDistanceFactor)
    {
        List<Vector3> result = new List<Vector3> ();

        if (points != null && points.Count >= 4)
        {
            Vector3 prevDirection = points [1] - points [0]; ;
            prevDirection.Normalize ();
            result.Add (points [0]);

            for (int i = 1; i < points.Count - 2; i++)
            {
                Vector3 p0 = points [i - 1];
                Vector3 p1 = points [i];
                Vector3 p2 = points [i + 1];
                Vector3 p3 = points [i + 2];

                Vector3 direction = p2 - p1;
                direction.Normalize ();
                Vector3 nextDirection = p3 - p2;
                nextDirection.Normalize ();

                float d1 = Vector3.Dot (prevDirection, direction);
                float d2 = Vector3.Dot (prevDirection, nextDirection);
                float dist = Vector3.Distance (p1, p2);

                if (Mathf.Abs (d1 - 1f) >= epsilon && Mathf.Abs (d2 - 1f) >= epsilon)
                {
                    result.Add (points [i]);

                    if (dist >= 0.2 && dist <= 10f)
                    {
                        Vector3 b_p0 = p1;
                        Vector3 b_p1 = p1 + prevDirection * bezierDistanceFactor * (1f - d2) * dist * 0.2f;
                        Vector3 b_p2 = p2 - nextDirection * bezierDistanceFactor * (1f - d2) * dist * 0.2f;
                        Vector3 b_p3 = p2;

                        for (float j = 0.05f; j < 1f; j += 0.05f)
                        {
                            Vector3 newPoint = Curver.cubeBezier3 (b_p0, b_p1, b_p2, b_p3, j);
                            result.Add (newPoint);
                        }
                    }


                    result.Add (points [i + 1]);
                    i++;
                    prevDirection = nextDirection;
                }
                else
                {
                    result.Add (points [i]);
                }
            }

            for (int i = points.Count - 2; i < points.Count; i++)
            {
                result.Add (points [i]);
            }

        }
        else if (points != null)
        {
            points.ForEach (p => result.Add (p));
        }

        return result;
    }
}
