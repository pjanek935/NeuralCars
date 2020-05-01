using UnityEngine;
using System.Collections;

public class DodgeObserver : ObserverScript {

    private int simulation = 0;



    // Use this for initialization
    void Start () {
        Init();

        logName = "log";
    }
	
	// Update is called once per frame
	void Update () {
        ObseverUpdate();
	}

    protected override void Reset()
    {
        CleanUpBullets();
        base.Reset();

        if (generation > 150)
        {
            HardReset();
        }
    }

    private void HardReset()
    {
        foreach (GameObject a in agents)
        {
            Destroy(a);
        }
        bestFitness = 0;
        bestCar = null;
        Init();
        simulation++;
        generation = 0;
        logName = "log" + simulation;

    }

    void CleanUpBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject obj in bullets)
        {
            if (obj.name != "Wall") Destroy(obj);
        }
    }
}
