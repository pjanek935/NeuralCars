using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CollectibleObserver : ObserverScript
{
    public float generationDuration = 30;
    public GameObject collectible;
    public int collectibleAmount = 100;

    private List<GameObject> collectibles;
    public float timer = 0;

    private int simulation = 0;

	// Use this for initialization
	void Start ()
    {
        collectibles = new List<GameObject>();
        Init();
        SpawnCollectibles();

        logName = "log";
    }

    private void SpawnCollectibles()
    {
        for (int i=0; i<collectibleAmount; i++)
        {
            AddCollectible();
        }
    }

    private void AddCollectible()
    {
        GameObject col;
        col = Instantiate(collectible, spawnPoint.position + GetRandVec(), spawnPoint.rotation) as GameObject;
        collectibles.Add(col);
    }

    // Update is called once per frame
    void Update () {

        timer += Time.deltaTime;
        if (timer >= generationDuration)
        {
            timer = 0;
            foreach (GameObject agent in agents)
            {
                agent.GetComponent<Fitness>().SetActive(false);
            }
        }

        //if (collectibles.Count < collectibleAmount * 0.99)
        //{
        //    AddCollectible();
        //}

        ObseverUpdate();
    }

    protected override void Reset()
    {
        Debug.Log("Collectibles count: " + collectibles.Count);
        CleanUpCollectibles();
        base.Reset();

        if (generation > 1000)
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

    private void CleanUpCollectibles()
    {
        foreach (GameObject obj in collectibles)
            Destroy(obj);
        collectibles.Clear();
        SpawnCollectibles();
    }
    
    public void DestroyCollectible(int id)
    {
        for (int i=0; i<collectibles.Count; i++)
        {
            if (collectibles[i].GetComponent<CollectibleScript>().id == id)
            {
                Destroy(collectibles[i]);
                collectibles.RemoveAt(i);
                break;
            }
        }
        //Debug.Log("Collectibles Count: " + collectibles.Count);
    }
}
