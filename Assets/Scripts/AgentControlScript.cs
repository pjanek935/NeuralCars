using UnityEngine;
using System.Collections;

public class AgentControlScript : MonoBehaviour {

    public bool playerControl = true;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 20f;
    public GameObject projectile;
    public Transform spawnPoint;
    public float shootDelay = 2;

    private float horMove;
    private float verMove;
    private float rotation;
    private float shootTimer = 0;
    private bool canShoot = true;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


        if (playerControl)
        {
            if (Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.K))
            {
                rotation = 0;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                rotation = -0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                rotation = 0.1f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
        

        
    }

    void FixedUpdate()
    {

        if (playerControl)
        {
            horMove = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            verMove = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            Rotate(rotation);
            MoveForward(verMove);
            MoveRight(horMove);
        }

        if (!canShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootDelay)
            {
                canShoot = true;
                shootTimer = 0;
            }
        }
        
        //transform.position += verMove * transform.forward * Time.deltaTime * moveSpeed;
        //transform.position += horMove * transform.right * Time.deltaTime * moveSpeed;
        //transform.Rotate(0, rotationSpeed * rotation, 0);

        //Debug.Log(rotation + " " + verMove);
        //transform.Translate(horMove, 0, verMove);
    }

    public void Shoot()
    {
        if (canShoot)
        {
            Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            canShoot = false;
        }
        
    }

    public void Rotate(float rotation)
    {
        transform.Rotate(0, rotationSpeed * rotation, 0);
    }

    public void MoveForward(float forMove)
    {
        transform.position += forMove * transform.forward * Time.deltaTime * moveSpeed;
    }

    public void MoveRight(float rightMove)
    {
        transform.position += rightMove * transform.right * Time.deltaTime * moveSpeed;
    }
}
