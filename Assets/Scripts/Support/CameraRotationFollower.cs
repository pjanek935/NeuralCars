using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CameraRotationFollower : MonoBehaviour
{
    [SerializeField] Camera cameraToFollow;
    [SerializeField] Vector3 startPosition;
    [SerializeField] float startRotation = 0f;

	void Update ()
    {
		if (cameraToFollow != null)
        {
            float cameraYRotation = cameraToFollow.transform.rotation.eulerAngles.y + startRotation;
            float newX = startPosition.x * Mathf.Cos (Mathf.Deg2Rad * cameraYRotation) + startPosition.z * Mathf.Sin (Mathf.Deg2Rad * cameraYRotation);
            float newZ = -1 * startPosition.x * Mathf.Sin (Mathf.Deg2Rad * cameraYRotation) + startPosition.z * Mathf.Cos (Mathf.Deg2Rad * cameraYRotation);
            this.transform.position = new Vector3 (-newX, this.transform.position.y, newZ);
            this.transform.eulerAngles = new Vector3 (this.transform.eulerAngles.x, -cameraYRotation, this.transform.eulerAngles.z);
        }
	}
}
