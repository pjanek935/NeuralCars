using UnityEngine;
using UnityEngine.Events;

public class CameraFollow : MonoBehaviour
{
    public UnityAction OnDrag;

    [SerializeField] Transform target;
    [SerializeField] float smoothness = 10f;
    [SerializeField] Vector3 offset;
    [SerializeField] float scrollSpeed = 5f;

    Vector2 pointerPos;

    private void FixedUpdate ()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + offset;
            Vector3 newPos = Vector3.Lerp (transform.position, targetPos, smoothness * Time.deltaTime);
            transform.position = newPos;

            Quaternion targetRotation = Quaternion.LookRotation (target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, smoothness * Time.deltaTime);
        }
    }

    private void Update ()
    {
        updateZooming ();

        if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1))
        {
            if (Vector2.Distance (pointerPos, Input.mousePosition) * Time.deltaTime > 0.1f)
            {
                OnDrag?.Invoke ();
            }
        }

        pointerPos = Input.mousePosition;
    }

    void updateZooming ()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (Mathf.Abs (scrollDelta) > 0.1f)
        {
            float deltaY = -scrollDelta * scrollSpeed;
            offset.y += deltaY;
            offset.y = Mathf.Clamp (offset.y, 5f, 100f);
        }
    }

    public void SetTarget (Transform target)
    {
        this.target = target;
    }
}

