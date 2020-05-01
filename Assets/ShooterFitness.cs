using UnityEngine;
using System.Collections;

public class ShooterFitness : MonoBehaviour {

    private float fitness = 0;
    private bool crashed = false;
    private NeuralNetwork2 nn;

    private int collisionCount = 0;
    public int maxHits = 3;

    // Use this for initialization
    void Start () {
        nn = GetComponent<NeuralNetwork2>();

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(0.5f, 1f, 0.1f);
    }
	
	// Update is called once per frame
	void Update () {
        if(!crashed)
            fitness += Time.deltaTime;	
	}

    public float GetFitness()
    {
        return (int)fitness;
    }

    public void Reset()
    {
        fitness = 0;
        collisionCount = 0;
        crashed = false;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(0.5f, 1f, 0.1f);

    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            collisionCount++;
            if (other.gameObject.name == "Wall") collisionCount = 3;
            if (collisionCount >= maxHits)
            {
                nn.neuralControled = false;
                crashed = true;
                Debug.Log("Crashed due to wall hit");
                Renderer renderer = GetComponent<Renderer>();
                renderer.material.color = new Color(1f, 0.5f, 0.1f);
            }
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Wall")
    //    {
    //        //nn.neuralControled = false;
    //        //crashed = true;
    //        //Debug.Log("Crashed due to wall hit");
    //        //Renderer renderer = GetComponent<Renderer>();
    //        //renderer.material.color = new Color(1f, 0.5f, 0.1f);
    //        collisionCount++;
    //        if (other.gameObject.name == "Wall") collisionCount = maxHits;
    //        if (collisionCount >= maxHits)
    //        {
    //            nn.neuralControled = false;
    //            crashed = true;
    //            // Debug.Log("Crashed due to wall hit");
    //            Renderer renderer = GetComponent<Renderer>();
    //            renderer.material.color = new Color(1f, 0.5f, 0.1f);
    //        }
    //    }
    //}

    public void StartCar()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        GetComponent<NeuralNetwork2>().neuralControled = true;
    }

    public bool IsCrashed() { return crashed; }
}
