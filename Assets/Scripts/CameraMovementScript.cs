using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour {

    public float moveSpeed = 25.0f;

    private float horMove;
    private float verMove;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        horMove = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        verMove = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        //Debug.Log(horMove + " " + verMove);
        transform.Translate(horMove, 0, verMove);
    }

    void FixesUpdate()
    {
        
    }
}
