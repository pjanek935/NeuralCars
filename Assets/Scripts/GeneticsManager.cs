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
    [SerializeField] bool crossBreedSensors = false;
    [SerializeField] float mutationProbability = 0.2f;
    [SerializeField] bool adaptiveMutationProbability = true;

    List<CarNeuralCore> cars = new List<CarNeuralCore> ();
    Genetics genetics = new Genetics ();
    CarSimpleData prevBestCar = new CarSimpleData ();

    const float SENSOR_LENGTH_D = 5f;
    const float ANGLE_BETWEEN_SENSORS_D = 1f;

    public bool CrossbreedSensors
    {
        get { return crossBreedSensors; }
        set { crossBreedSensors = value; }
    }

    public bool AdaptiveMutationProbability
    {
        get { return adaptiveMutationProbability; }
        set { adaptiveMutationProbability = value; }
    }

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
        double [] fitnesses = new double [sortedCars.Count];
        List<CarSimpleData> newGenCars = new List<CarSimpleData> ();
        newGenCars.Add (sortedCars [0].GetCarSimpleData ());

        for (int i = 0; i < sortedCars.Count; i ++)
        {
            fitnesses [i] = sortedCars [i].GetComponent<CarFitness> ().GetFitness ();
        }

        if (fitnesses [0] > prevBestCar.Fitness)
        {
            prevBestCar = sortedCars [0].GetCarSimpleData ();
        }
        else
        {

            newGenCars.Add (prevBestCar.GetCopy ());
        }

        if (AdaptiveMutationProbability)
        {
            if (prevBestCar.Fitness > fitnesses [0])
            {
                mutationProbability += 0.01f;
            }
            else
            {
                mutationProbability -= 0.01f;
            }

            mutationProbability = Mathf.Clamp (mutationProbability, 0.05f, 0.6f);
        }

        while (newGenCars.Count < carsCount)
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

            CarSimpleData c1;
            CarSimpleData c2;
            genetics.CrossoverCars (sortedCars [parent1index].GetCarSimpleData (), sortedCars [parent2index].GetCarSimpleData (), out c1, out c2, Genetics.CrossType.ARYTM);
            newGenCars.Add (c1);
            newGenCars.Add (c2);
        }

        for (int i = 0; i < newGenCars.Count; i++)
        {
            System.Random rnd = new System.Random ((int) DateTime.Now.Ticks);

            if (rnd.NextDouble () < mutationProbability)
            {
                genetics.Mutation (newGenCars [i].Weights, mutationProbability);
            }
        }

        //if (CrossbreedSensors)
        //{
        //    System.Random rnd = new System.Random ((int) DateTime.Now.Ticks);

        //    for (int i = 0; i < newGenWeights.Count; i++)
        //    {
        //        if (rnd.NextDouble () < mutationProbability)
        //        {
        //            float d = (float) rnd.NextDouble () * SENSOR_LENGTH_D - SENSOR_LENGTH_D / 2f;
        //            newGenSensorLength [i] += d;
        //        }

        //        if (rnd.NextDouble () < mutationProbability)
        //        {
        //            float d = (float) rnd.NextDouble () * ANGLE_BETWEEN_SENSORS_D - ANGLE_BETWEEN_SENSORS_D / 2f;
        //            newGenAngleBetweenSensors [i] += d;
        //        }
        //    }
        //}

        for (int i = 0; i < cars.Count; i ++)
        {
            cars [i].SetWeights (newGenCars [i].Weights);

            //if (CrossbreedSensors)
            //{
            //    cars [i].SetSensorsLength (newGenSensorLength [i]);
            //    cars [i].SetAngleBetweenSensors (newGenAngleBetweenSensors [i]);
            //}
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
