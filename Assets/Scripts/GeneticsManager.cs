using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GeneticsManager : MonoBehaviour
{
    [SerializeField] Transform startPosition;
    [SerializeField] GameObject carPrefab;

    [SerializeField] int carsCount = 10;
    [SerializeField] int neuronsInHiddenLayer = 5;
    [SerializeField] int sensorsCount = 7;
    [SerializeField] float angleBetweenSensors = 15f;
    [SerializeField] float sensorsLength = 15f;

    List<CarNeuralCore> cars = new List<CarNeuralCore> ();
    Genetics genetics = new Genetics ();
    double [] prevBest;
    double prevBestFitness;

    float mutationProbability = 0.2f;

    public int CarsCount
    {
        get { return carsCount; }
        set { carsCount = value; }
    }

    public int NeuronsInHiddenLayer
    {
        get { return neuronsInHiddenLayer; }
        set { neuronsInHiddenLayer = value; }
    }

    public int SensorsCount
    {
        get { return sensorsCount; }
        set { sensorsCount = value; }
    }

    public float AngleBetweenSensors
    {
        get { return angleBetweenSensors; }
        set { angleBetweenSensors = value; }
    }

    public float SensorsLength
    {
        get { return sensorsLength; }
        set { sensorsLength = value; }
    }

    void OnEnable ()
    {
        Init ();
    }

    public void Init ()
    {
        createCars ();
    }

    public void Pause ()
    {
        Time.timeScale = 0f;
    }

    public void Resume ()
    {
        Time.timeScale = 1f;
    }

    public void SetSensorsVisible (bool visible)
    {
        foreach (CarNeuralCore car in cars)
        {
            car.SetSensorsVisible (visible);
        }
    }

    public void SetSensorsLength (float length)
    {
        foreach (CarNeuralCore car in cars)
        {
            car.SetSensorsLength (length);
        }
    }

    public void SetAngleBetweenSensors (float angle)
    {
        foreach (CarNeuralCore car in cars)
        {
            car.SetAngleBetweenSensors (angle);
        }
    }

    void createCars ()
    {
        if (carsCount < 0)
        {
            return;
        }

        for (int i = 0; i < carsCount; i ++)
        {
            if (i >= cars.Count)
            {
                createNewCarObject ();
            }

            cars [i].gameObject.transform.position = startPosition.position;
            cars [i].gameObject.transform.forward = startPosition.forward;
            cars [i].AngleBetweenSensors = angleBetweenSensors;
            cars [i].NeuronsInHiddenLayer = neuronsInHiddenLayer;
            cars [i].SensorsCount = sensorsCount;
            cars [i].SensorsLength = sensorsLength;
            cars [i].Init ();
        }

        int diff = cars.Count - carsCount;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                deleteLastCarObject ();
            }
        }
    }

    void createNewCarObject ()
    {
        GameObject newGameObject = Instantiate (carPrefab);
        newGameObject.SetActive (true);
        newGameObject.transform.SetParent (this.transform, false);
        CarNeuralCore carNeuralCore = newGameObject.GetComponent<CarNeuralCore> ();
        carNeuralCore.OnCarDisabled += onCarDisabled;
        cars.Add (carNeuralCore);
    }

    void onCarDisabled (CarNeuralCore carNeuralCore)
    {
        bool allDisabled = true;

        for (int i = 0; i < cars.Count; i ++)
        {
            if (cars [i].IsActive)
            {
                allDisabled = false;

                break;
            }
        }

        if (allDisabled)
        {
            onAllCarsDisabled ();
        }
    }

    void onAllCarsDisabled ()
    {
        List<CarNeuralCore> sortedCars = getSortedByFitnessCarsList ();
        List<double []> newGenWeights = new List<double []> ();
        double [] fitnesses = new double [sortedCars.Count];

        for (int i = 0; i < sortedCars.Count; i ++)
        {
            fitnesses [i] = sortedCars [i].GetComponent<CarFitness> ().GetFitness ();
        }

        //Preserve best car in this generation (car with higher fitness)
        newGenWeights.Add (sortedCars [0].GetWeights ());

        if (fitnesses [0] > prevBestFitness)
        {
            prevBestFitness = fitnesses [0];
            prevBest = sortedCars [0].GetWeights ();
        }
        else
        {
            newGenWeights.Add (prevBest);
        }

        if (prevBestFitness > fitnesses [0])
        {
            mutationProbability += 0.01f;
        }
        else
        {
            mutationProbability -= 0.01f;
        }

        mutationProbability = Mathf.Clamp (mutationProbability, 0.05f, 0.6f);
        
        while (newGenWeights.Count < carsCount)
        {
            int parent1index = genetics.RouletteSelect (fitnesses);
            int parent2index = genetics.RouletteSelect (fitnesses);
            int helpIndex = 0;

            while (parent1index == parent2index)
            {
                parent2index = genetics.RouletteSelect (fitnesses);
                helpIndex++;

                if (helpIndex > carsCount)
                {
                    break;
                }
            }

            double [] parent1Weights = sortedCars [parent1index].GetWeights ();
            double [] parent2Weights = sortedCars [parent2index].GetWeights ();

            double [] child1;
            double [] child2;
            genetics.Crossover (parent1Weights, parent2Weights, out child1, out child2, Genetics.CrossType.ARYTM);

            newGenWeights.Add (child1);
            newGenWeights.Add (child2);
        }

        for (int i = 0; i < newGenWeights.Count; i++)
        {
            System.Random rnd = new System.Random ((int) DateTime.Now.Ticks);

            if (rnd.NextDouble () < mutationProbability)
            {
                genetics.Mutation (newGenWeights [i], mutationProbability);
            }
        }

        for (int i = 0; i < cars.Count; i ++)
        {
            cars [i].SetWeights (newGenWeights [i]);
        }

        resetCars ();
    }

    List <CarNeuralCore> getSortedByFitnessCarsList ()
    {
        return cars.OrderByDescending (c => c.GetComponent<CarFitness> ().GetFitness ()).ToList ();
    }

    void deleteLastCarObject ()
    {
        if (cars.Count > 0)
        {
            GameObject tmp = cars [cars.Count - 1].gameObject;
            cars.RemoveAt (cars.Count - 1);
            Destroy (tmp);
        }
    }

    void resetCars ()
    {
        foreach (CarNeuralCore car in cars)
        {
            car.gameObject.transform.position = startPosition.position;
            car.gameObject.transform.forward = startPosition.forward;

            car.Reset ();
        }
    }
}
