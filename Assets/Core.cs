using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;
using System;

public class Core : MonoBehaviour {

    public GameObject rayCast;
    public int rayCastAmount = 8;
    public int neuronsInHiddenLayer = 5;
    public bool neuralControlled;
    public int outputAmount = 2;
    public float angle = 80;
    public float offset = 3;
    public Type type = Type.GENETICS;
    public int epochs = 200;

    public bool withMemory = false;
    public int memoryLength = 4;
    public int memoryGap = 5;
    private double[][] memory;
    private int tickCounter = 0;

    private List<GameObject> rayCasts;
    private NeuralNetwork nn;
    private NeuralNetworkBackProp nnbp;
    private double[] xValues;
    private double[] outputReg;
    private double[] outputClas;

    private Vector3 prevPos;
    private float speed = 0;
    private int classAmount = 8;

    public enum Type {GENETICS, Q, PLAYER, GATHER, NONE};

    // Use this for initialization
    void Start () {
        if (withMemory)
        {
            nn = new NeuralNetwork(rayCastAmount + rayCastAmount * memoryLength,
                neuronsInHiddenLayer, outputAmount);
            nnbp = new NeuralNetworkBackProp(rayCastAmount + rayCastAmount * memoryLength,
                neuronsInHiddenLayer, classAmount);
        }
        else
        {
            nn = new NeuralNetwork(rayCastAmount,
                neuronsInHiddenLayer, outputAmount);
            nnbp = new NeuralNetworkBackProp(rayCastAmount,
                neuronsInHiddenLayer, classAmount);
        }

        memory = new double[memoryLength*memoryGap][];
        for(int i=0; i<memoryLength*memoryGap; i++)
        {
            memory[i] = new double[rayCastAmount];
            for (int j=0; j<rayCastAmount; j++)
            {
                memory[i][j] = 0;
            }
        }
            
        rayCasts = new List<GameObject>();
        outputReg = new double[outputAmount];
        outputClas = new double[classAmount];
        for (int i = 0; i < rayCastAmount; i++)
        {
            GameObject newRay = Instantiate(rayCast) as GameObject;
            newRay.transform.SetParent(this.transform);
            newRay.transform.Rotate(0, angle / rayCastAmount * i - angle / 2 + transform.rotation.y + 90, 0);
            newRay.transform.position = transform.position + transform.forward * offset;
            newRay.GetComponentInChildren<RayCast>().parentObject = this.gameObject;
            rayCasts.Add(newRay);
        }
        prevPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //speed = (prevPos - transform.position).magnitude;
        //prevPos = transform.position;
        
        if (neuralControlled && (type == Type.GENETICS || type == Type.Q))
        {
            GatherXValues();

            switch (type)
            {
                case Type.GENETICS:
                    outputReg = nn.GetOutput(xValues);
                    break;

                case Type.Q:
                    outputClas = nnbp.GetOutput(xValues);
                    break;

                default:
                    break;
            }
        }
        else
        {
            GatherXValues();
        }
    }

    private void GatherXValues()
    {
        if (withMemory)
        {
            xValues = new double[rayCastAmount + rayCastAmount*memoryLength];
            for (int i = 0; i < rayCastAmount; i++)
            {
                xValues[i] = rayCasts[i].GetComponentInChildren<RayCast>().GetStimulation();
            }

            for (int i=0; i < memoryLength; i++)
            {
                for (int j = 0; j < rayCastAmount; j++)
                {
                    xValues[(i + 1) * rayCastAmount + j] = memory[i * memoryGap][j];
                }
            }

            //for (int i = 0; i < memoryLength*memoryGap; i=i+memoryGap)
            //{
            //    for (int j=0; j<rayCastAmount; j++)
            //    {
            //        int index = (i + 1) * rayCastAmount + j;
            //        Debug.Log(index);
            //        xValues[(i + 1) * rayCastAmount + j] = memory[i][j];
            //    }
            //}
            PushMemory();
        }
        else
        {
            xValues = new double[rayCastAmount];
            for (int i = 0; i < rayCastAmount; i++)
            {
                xValues[i] = rayCasts[i].GetComponentInChildren<RayCast>().GetStimulation();
            }
        }
        
    }

    private void PushMemory()
    {
        double[][] newMemory = new double[memoryLength * memoryGap][];
        for (int i = 0; i < memoryLength * memoryGap; i++)
            newMemory[i] = new double[rayCastAmount];

        for (int i=0; i<rayCastAmount; i++)
        {
            newMemory[0][i] = xValues[i];
        }

        for (int i=1; i<memoryLength * memoryGap; i++)
        {
            newMemory[i] = memory[i - 1];
        }

        memory = newMemory;
    }

    public void Train(List<double[]> learningVectors)
    {
        Transform spawnPoint = GameObject.Find("SpawnPoint").transform;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        GetComponent<Fitness>().Reset();
        
        double[][] trainData = new double[learningVectors.Count][];
        for (int i = 0; i < learningVectors.Count; i++)
        {
            trainData[i] = new double[learningVectors[i].Length];
            for (int j=0; j< learningVectors[i].Length; j++)
            {
                trainData[i][j] = learningVectors[i][j];
            }
            //trainData[i] = learningVectors[i];
            //Debug.Log(trainData[i].Length);
        }


        Debug.Log("Started training...");
        
         nnbp.BackProp(trainData, epochs, 0.05, 0.01);
       // nnbp.SetWeights(weights);

    }

    public double[] GetXValues() { return xValues; }
    public void ResetWeights(){ nn.InitWeights(); }
    public double[] GetWeights(){ return nn.GetWeights(); }
    public void SetWeights(double[] weights){ nn.SetWeights(weights); }
    public double[] GetRandomWeights(){ return nn.GetRandomWeights(); }
    public double[] Train(double[][] trainData, int maxEpochs,
      double learnRate, double momentum)
    {
        return nn.Train(trainData, maxEpochs, learnRate, momentum);
    }
    public double[] GetOutput()
    {
        if (type == Type.GENETICS)
        {
            return outputReg;
        }
        else
        {
            return outputClas;
        }
        
    }
}
