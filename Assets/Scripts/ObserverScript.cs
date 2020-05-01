using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using System;
using System.IO;
using UnityEditor;

public class ObserverScript : MonoBehaviour {

    public Transform spawnPoint;
    public float randPositionRadius = 0;
    public GameObject agent;
    public int amount = 16;
    public Genetics.CrossType crossType = Genetics.CrossType.ARYTM;
    public Genetics.MutationType mutationType = Genetics.MutationType.REGULAR;
    public float mutationProbability = 0.2f;
    public bool addNewEachGen = true;
    public string populationFileName = "population10-10/population47.txt";

    protected List<GameObject> agents;
    private Genetics genetics;
    protected double[] bestCar;
    protected float bestFitness = 0;
    protected int generation = 0;
    protected string logName = "log";
    private bool continueLearning = true;

    private List<string> log;

    private int savePopulationCounter = 0;

    private static System.Random rand = new System.Random();

    private bool loaded = false;
    private bool pause = true;
    // Use this for initialization
    void Start () {
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        
        Init();
	}

    protected void Init()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 30;
        Application.runInBackground = true;
        genetics = GetComponent<Genetics>();
        agents = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            GameObject spawnedCar = Instantiate(agent, spawnPoint.transform.position + GetRandVec(),
                spawnPoint.rotation, spawnPoint)
                as GameObject;
            agents.Add(spawnedCar);
        }
        log = new List<string>();

        //LoadPopulation("startPopulation.txt", false);
    }
	
	// Update is called once per frame
	void Update () {
        ObseverUpdate();
        //Debug.Log("Update");
	}


    protected void ObseverUpdate()
    {
        //Debug.Log("started update");
        if (!loaded)
        {
            loaded = true;
            LoadPopulation("startPopulation.txt", false);
            return;
        }
        bool allCrashed = true;
        foreach (GameObject c in agents)
        {
            if (c.GetComponent<Fitness>().IsActive())
            {
                allCrashed = false;
                break;
            }
        }
        if (allCrashed)
        {
            if (pause)
            {
                EditorApplication.isPaused = true;
                pause = false;
                return;
            }
            
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Load population");
            LoadPopulation(populationFileName, true);
        }
        //Debug.Log("Ended update");
    }

    protected virtual void Reset()
    {
        pause = true;
        Debug.Log("Entered resetting");
        //Posoruj agentów pod względem funkcji przystosowania
        List<GameObject> SortedList = agents.OrderByDescending(c => c.GetComponent<Fitness>().GetFitness()).ToList();
        agents = SortedList;
        Debug.Log("Sorted cars");

        int chromosomeSize = agents[0].GetComponent<Core>().GetWeights().Length;

        //Preserve car with best fitness
        if (agents[0].GetComponent<Fitness>().GetFitness() > bestFitness)
        {
            bestFitness = agents[0].GetComponent<Fitness>().GetFitness();
            bestCar = new double[chromosomeSize];
            agents[0].GetComponent<Core>().GetWeights().CopyTo(bestCar, 0);
            Debug.Log("New best fitness: " + bestFitness);
        }

        if (agents[0].GetComponent<Fitness>().GetFitness() >= 306)
        {
            //SavePopulation();
        }

        //save log
        Debug.Log("Calculating avg fitness");
        float avgFitness = 0;
        for (int i = 0; i < agents.Count; i++)
            avgFitness += agents[i].GetComponent<Fitness>().GetFitness();
        avgFitness /= agents.Count;
        string line = generation + " " + agents[0].GetComponent<Fitness>().GetFitness() + " " +
            bestFitness + " " + avgFitness;
        log.Add(line);
        Debug.Log("Saving log");
        SaveLog();

        //Pobierz wartosci funkcji przystosowania
        //od agentow
        double[] fitness = new double[agents.Count];
        for (int i=0; i<agents.Count; i++)
        {
            fitness[i] = agents[i].GetComponent<Fitness>().GetFitness();
        }

        Debug.Log("Creating new children");
        double[][] childrenWeights = new double[amount][];
        for (int i=0; i<amount; i++)
        {
            childrenWeights[i] = new double[chromosomeSize];
        }

        //Assign best car and best car from current generation
        //childrenWeights[0] = cars[0].GetComponent<NeuralNetwork2>().GetWeights();
        agents[0].GetComponent<Core>().GetWeights().CopyTo(childrenWeights[0], 0);
        if (bestCar != null)
        {
            bestCar.CopyTo(childrenWeights[1], 0);
        }
        else
        { 
            agents[1].GetComponent<Core>().GetWeights().CopyTo(childrenWeights[1], 0);
        }

        Debug.Log("Creating new generation");
        int index = 2;
        int limit = amount;
        if (addNewEachGen) limit = amount - 2;
        while (index < limit)
        {
            //Wybierz indeksy rodzicow
            int parent1index = genetics.rouletteSelect(fitness);
            int parent2index = genetics.rouletteSelect(fitness);

            //Upewnij sie ze rodzice nie sa tacy sami
            while (parent1index == parent2index)
            {
                parent2index = genetics.rouletteSelect(fitness);
            }
            
            //Pobierz wagi rodzicow
            double[] parent1 = agents[parent1index].GetComponent<Core>().GetWeights();
            double[] parent2 = agents[parent2index].GetComponent<Core>().GetWeights();

            //Dokonaj krzyzowania
            double[] child1;
            double[] child2;
            genetics.Crossover(parent1, parent2, out child1, out child2, crossType);

            child1.CopyTo(childrenWeights[index], 0);
            index++;
            child2.CopyTo(childrenWeights[index], 0);
            index++;
            //Debug.Log("Parent1: " + parent1index + ", parent2: " + parent2index);
        }

        while (index < amount)
        {
            Debug.Log("nowe typy");
            double[] randWeights = agents[0].GetComponent<Core>().GetRandomWeights();
            randWeights.CopyTo(childrenWeights[index], 0);
            index++;
        }

        Debug.Log("Starting mutation");
        //Mutate
        for (int i = 0; i < childrenWeights.Length; i++)
        {
            System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
            if (rnd.NextDouble() < mutationProbability)
            {
                genetics.Mutation(childrenWeights[i], mutationProbability);
            }
        }
        if (mutationType == Genetics.MutationType.IRREGULAR)
        {
            mutationProbability -= 0.0033f;
            Debug.Log("Mutation probability: " + mutationProbability);
            if (mutationProbability < 0.01)
            {
                mutationProbability = 0.01f;
            }
        }


        for (int i=0; i<agents.Count; i++)
        {
            agents[i].GetComponent<Fitness>().Reset();
            agents[i].GetComponent<Core>().SetWeights(childrenWeights[i]);
            agents[i].transform.position = spawnPoint.position + GetRandVec();
            agents[i].transform.rotation = spawnPoint.rotation;
            //cars[i].GetComponent<FitnessScript>().StartCar();
        }

        //agents[0].GetComponent<Renderer>().material.color = new Color(0, 1, 0);

        generation++;
        Debug.Log("Current generation: " + generation + ". Best fitness so far: " + bestFitness);

      
    }

    private void SaveLog()
    {
        using (StreamWriter outputFile = new StreamWriter(logName + ".txt"))
        {
            //String line = "Generacja    Najlepsze w gen.    Najlepsze dotychczas";
            //outputFile.WriteLine(line);
            foreach (string l in log)
                outputFile.WriteLine(l);

        }
    }

    public void SavePopulation()
    {
        Debug.Log("Saving population");
        using (StreamWriter outputFile = new StreamWriter("population/population" + savePopulationCounter + ".txt"))
        {
            foreach (GameObject agent in agents)
            {
                double[] weights = agent.GetComponent<Core>().GetWeights();
                String line = "";
                foreach(double w in weights)
                {
                    line += w + " ";
                }
                outputFile.WriteLine(line);
            }
        }
        savePopulationCounter++;
    }

    public void LoadPopulation(string name, bool onlyBest)
    {
        string line;
        System.IO.StreamReader file =
            new System.IO.StreamReader(name);
        double[][] weights = new double[amount][];
        int counter = 0;
        while ((line = file.ReadLine()) != null)
        {
            string[] characters = line.Split(' ');
            weights[counter] = new double[characters.Length - 1];
            for (int i=0; i<characters.Length-1; i++)
            {
                weights[counter][i] = Convert.ToDouble(characters[i]);
                //Debug.Log(weights[counter][i]);
            }
            counter++;
        }
        file.Close();

        for (int i =0; i<agents.Count; i++)
        {
            agents[i].GetComponent<Fitness>().Reset();
            if (onlyBest)
            {
                agents[i].GetComponent<Core>().SetWeights(weights[0]);
            }
            else
            {
                agents[i].GetComponent<Core>().SetWeights(weights[i]);
            }
            
            agents[i].transform.position = spawnPoint.position + GetRandVec();
            agents[i].transform.rotation = spawnPoint.rotation;
        }

    }

    public Vector3 GetRandVec()
    {
        float randX = (float)rand.NextDouble() * randPositionRadius - randPositionRadius / 2;
        float randZ = (float)rand.NextDouble() * randPositionRadius - randPositionRadius / 2;
        Vector3 randVec = new Vector3(randX, 0, randZ);
        return randVec;
    }
}
