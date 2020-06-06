using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothness = 10f;
    [SerializeField] Vector3 offset;

    private void FixedUpdate ()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + offset;
            Vector3 newPos = Vector3.Lerp (transform.position, targetPos, smoothness * Time.deltaTime);
            transform.position = newPos;
            transform.LookAt (target);
        }
    }
}

