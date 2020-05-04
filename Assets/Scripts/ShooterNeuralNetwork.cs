using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets;

public class ShooterNeuralNetwork : MonoBehaviour {

    public GameObject rayCast;
    public int amount = 16;
    public int neuronsInHiddenLayer = 5;
    public int outputs = 2;
    public float angle = 80;
    public bool neuralControled = false;

    private List<GameObject> rayCasts;
    private NeuralNetwork nn;
    private CarControlScipt cc;

    private double[] prevOutput;

    private double[] xValues;

    private double[] output;

    // Use this for initialization
    void Start()
    {
        cc = GetComponent<CarControlScipt>();
        nn = new NeuralNetwork(amount*2 + outputs, neuronsInHiddenLayer, outputs);
        rayCasts = new List<GameObject>();
        prevOutput = new double[outputs];

        for (int i = 0; i < outputs; i++)
        {
            prevOutput[i] = 0;
        }


        for (int i = 0; i < amount; i++)
        {
            GameObject newRay = Instantiate(rayCast) as GameObject;
            newRay.transform.SetParent(this.transform);
            newRay.transform.Rotate(0, angle / amount * i - angle / 2 + transform.rotation.y + 90, 0);
            newRay.transform.position = transform.position + transform.forward * 3f;

            newRay.GetComponentInChildren<ShooterRayCast>().parentCar = this.gameObject;
            rayCasts.Add(newRay);
        }
        double[] w = nn.GetWeights();
        for (int i = 0; i < w.Length; i++)
        {
            //Debug.Log(w[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (neuralControled)
        {
            GatherXValues();
            output = nn.GetOutput(xValues);
            prevOutput = output;
            //cc.Move((float)output[0]);
            //cc.Move(1);
            //cc.Steer((float)output[1]);
            //Debug.Log(output[0] + ", ");
        }
        else
        {
            GatherXValues();
        }
    }

    private void GatherXValues()
    {
        xValues = new double[amount*2 + outputs];
        for (int i = 0; i < amount; i++)
        {
            xValues[i] = rayCasts[i].GetComponentInChildren<ShooterRayCast>().GetStimulation()[0];
        }
        for (int i = 0; i < 2; i++)
        {
            xValues[amount + i] = prevOutput[i];
        }
    }

    public double[] GetXValues()
    {
        return xValues;
    }

    public void SetPrevOutput(double[] prevOutput)
    {
        this.prevOutput = prevOutput;
    }

    public void ResetWeights()
    {
        nn.InitWeights();
    }

    public double[] GetWeights()
    {
        return nn.GetWeights();
    }

    public void SetWeights(double[] weights)
    {
        nn.SetWeights(weights);
    }

    public double[] GetRandomWeights()
    {
        return nn.GetRandomWeights();
    }

    public double[] Train(double[][] trainData, int maxEpochs,
      double learnRate, double momentum)
    {
        return nn.Train(trainData, maxEpochs, learnRate, momentum);
    }

    public double[] GetOutput()
    {
        return output;
    }
}
