using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class Control : MonoBehaviour {

    protected List<double[]> learningVectors;
    protected bool gathering = false;
    protected Core core;

    public int learingVectorAmount = 5000;
    public string learningFileName = "learning10.txt";

    private static System.Random rand = new System.Random();

    // Use this for initialization
    void Start () {
	
	}

    protected void Init()
    {
        learningVectors = new List<double[]>();
        core = GetComponent<Core>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        GatherUpdate();
    }

    protected void GatherUpdate()
    {
        if (gathering)
        {
            AddLearningVector();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gathering = !gathering;
            Debug.Log("Gathering: " + gathering);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Saving vectors...");
            gathering = false;
            SaveVectors();

            //core.Train(learningVectors);
            //core.neuralControlled = true;
            //core.type = Core.Type.Q;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            gathering = false;
            Debug.Log("Loading vectors...");
            LoadAndLearn(learingVectorAmount);
        }
    }

    private void LoadAndLearn(int v)
    {
        learningVectors.Clear();
        int counter = 0;
        string line;
        System.IO.StreamReader file =
           new System.IO.StreamReader(learningFileName);
        while ((line = file.ReadLine()) != null)
        {
            
            string[] vecStr = line.Split(' ');
            double[] vec = new double[vecStr.Length - 1];
            for (int i=0; i<vec.Length; i++)
            {
                vec[i] = Double.Parse(vecStr[i]);
            }
            learningVectors.Add(vec);
            //for (int i = 0; i < vec.Length; i++)
            //{
            //    Debug.Log(vec[i]);
            //}
            counter++;
       }
        Debug.Log("Done loading vectors");

        Debug.Log("Learning count: " + learningVectors.Count);
        while (learningVectors.Count > v)
        {
            int r = rand.Next(0, learningVectors.Count);
            learningVectors.RemoveAt(r);
        }
        Debug.Log("Learning count: " + learningVectors.Count);

        core.Train(learningVectors);
        core.neuralControlled = true;
        core.type = Core.Type.Q;

        file.Close();
    }

    private void SaveVectors()
    {
        using (StreamWriter outputFile = new StreamWriter("test.txt"))
        {
            foreach (double[] vec in learningVectors)
            {
                string line = "";
                for (int i = 0; i < vec.Length; i++)
                {
                    line += vec[i] + " ";
                }
                outputFile.WriteLine(line);
            }

        }
    }

    public double[] GetSteering()
    {
        double[] output = core.GetOutput();
        int winner = 0;
        double maxVar = 0;
        for (int i = 0; i < 8; i++)
        {
            if (output[i] > maxVar)
            {
                maxVar = output[i];
                winner = i;
            }
        }

        float rotation = 0;
        float move = 0;

        switch (winner)
        {
            case 0:
                move = 1;
                rotation = 0;
                break;

            case 1:
                move = -1;
                rotation = 0;
                break;

            case 2:
                move = 0;
                rotation = 1;
                break;

            case 3:
                move = 0;
                rotation = -1;
                break;

            case 4:
                move = 1;
                rotation = 1;
                break;

            case 5:
                move = 1;
                rotation = -1;
                break;

            case 6:
                move = -1;
                rotation = 1;
                break;

            case 7:
                move = -1;
                rotation = -1;
                break;
        }

        double[] steering = new double[2];
        steering[0] = move;
        steering[1] = rotation;
        return steering;
    }

    private void AddLearningVector()
    {
        
        double[] xValues = core.GetXValues();
        float rotation = Input.GetAxis("Horizontal");
        float move = Input.GetAxis("Vertical");
        double[] output = new double[8];

        for (int i = 0; i < 8; i++)
            output[i] = 0;

        if (move > 0 && rotation == 0)
        {
            output[0] = 1;
        }
        else if (move < 0 && rotation == 0)
        {
            output[1] = 1;
        }
        else if (move == 0 && rotation > 0)
        {
            output[2] = 1;
        }
        else if (move == 0 && rotation < 0)
        {
            output[3] = 1;
        }
        else if (move > 0 && rotation > 0)
        {
            output[4] = 1;
        }
        else if (move > 0 && rotation < 0)
        {
            output[5] = 1;
        }
        else if (move < 0 && rotation > 0)
        {
            output[6] = 1;
        }
        else if (move < 0 && rotation < 0)
        {
            output[7] = 1;
        }

        double[] vector = new double[xValues.Length + 8];
        for (int i = 0; i < xValues.Length; i++)
            vector[i] = xValues[i];
        for (int i = 0; i < 8; i++)
            vector[xValues.Length + i] = output[i];

        learningVectors.Add(vector);

    }
}
