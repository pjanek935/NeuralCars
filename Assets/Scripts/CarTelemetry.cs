using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTelemetry : MonoBehaviour
{
    /// <summary>
    /// Length of a buffer that contains consequent movement directions. Vector3 MovementDirectionAverage is calucalted
    /// based on that buffer
    /// </summary>
    [SerializeField] int bufferSize = 10;

    Queue<Vector3> movementDirectionBuffer = new Queue<Vector3> ();
    Vector3 prevPosition = Vector3.zero;

    const float MIN_SQRD_MAGNITUDE = 0.1f;

    public Vector3 MovementDirectionAverage
    {
        get;
        private set;
    }

    private void Start ()
    {
        prevPosition = transform.position;
    }

    private void Update ()
    {
        updateMovementParameters ();
    }

    void updateMovementParameters ()
    {
        MovementDirectionAverage = transform.position - prevPosition;

        if (MovementDirectionAverage.sqrMagnitude < MIN_SQRD_MAGNITUDE)
        {
            MovementDirectionAverage = Vector3.zero;
        }

        MovementDirectionAverage.Normalize ();
        movementDirectionBuffer.Enqueue (MovementDirectionAverage);

        if (movementDirectionBuffer.Count > bufferSize)
        {
            movementDirectionBuffer.Dequeue ();
        }

        MovementDirectionAverage = calculateAverage (movementDirectionBuffer).normalized;
        prevPosition = transform.position;

        Debug.DrawLine (this.transform.position, this.transform.position + MovementDirectionAverage * 10, Color.yellow);
    }

    public float GetAngleBetweenForwardAndMovementDirection (bool normalized = false)
    {
        float result = Vector3.SignedAngle (transform.forward, MovementDirectionAverage, Vector3.up);

        if (normalized)
        {
            if (result > 180)
            {
                result = 180;
            }
            else if (result < -180)
            {
                result = -180;
            }

            result /= 180;
        }

        return result;
    }

    Vector3 calculateAverage (Queue<Vector3> queue)
    {
        Vector3 result = Vector3.zero;

        foreach (Vector3 v in queue)
        {
            result += v;
        }

        result /= queue.Count;

        return result;
    }
}
