using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public Transform target;

    private float rotationSpeed = 2;
    private Vector3 offset;
    private float startHeight;

	// Use this for initialization
	void Start () {
        offset = transform.position - target.position;
        startHeight = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = new Vector3(target.position.x,
            transform.position.y, target.position.z);
        newPos.y = startHeight;
        transform.position = newPos;

        Vector3 targetRotation = target.rotation.eulerAngles;
        targetRotation.x = transform.rotation.eulerAngles.x;
        targetRotation.z = transform.rotation.eulerAngles.z;

        Quaternion rot = Quaternion.Euler(targetRotation);

        transform.rotation = rot;

    }
}
