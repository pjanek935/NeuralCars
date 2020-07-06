using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeneticsManager : MonoBehaviour
{
    public UnityAction OnNewGenCreated;
    public event CarNeuralCore.CarNeuralCoreEventHandler OnCarClicked;

    [SerializeField] Transform startPosition;
    [SerializeField] GameObject carPrefab;
    [SerializeField] Stage stage;

    [SerializeField] int carsCount = 10;
    [SerializeField] int newRandomCarsCount = 5;
    [SerializeField] int neuronsInHiddenLayer = 5;
    [SerializeField] int sensorsCount = 7;
    [SerializeField] float angleBetweenSensors = 15f;
    [SerializeField] float sensorsLength = 15f;
    [SerializeField] bool crossBreedSensors = false;
    [SerializeField] float mutationProbability = 0.2f;
    [SerializeField] float mutationProbabilityD = 0.2f;
    [SerializeField] bool adaptiveMutationProbability = true;
    [SerializeField] bool disableOnWallHit = true;

    List<CarNeuralCore> cars = new List<CarNeuralCore> ();
    Genetics genetics = new Genetics ();
    CarSimpleData prevBestCar = new CarSimpleData ();
    bool isActivatingCars = false;
    Coroutine activatingCarsCoroutine;

    const float MAX_SENSOR_LENGTH = 50f; //if crossover sensors options enabled may exceed this value; used to calculate length for brand new cars
    const float MAX_ANGLE_BETWEEN_SENSORS = 15f; //if crossover sensors options enabled may exceed this value; used to calculate angle for brand new cars
    const float SENSOR_LENGTH_D = 4f; //max value that sensor length might change if crossover sensor option enabled
    const float ANGLE_BETWEEN_SENSORS_D = 2f; //max value that angle between sensors might change if crossover sensor option enabled
    const float DEFAULT_MUTATION_PROBABILITY = 0.1f;

    public int Generation
    {
        get;
        private set;
    }

    public bool IsPaused
    {
        get;
        private set;
    }

    public bool DisableOnWallHit
    {
        get { return disableOnWallHit; }

        set 
        { 
            disableOnWallHit = value;

            foreach (CarNeuralCore car in cars)
            {
                car.DisableOnWallHit = value;
            }
        }
    }

    private void OnValidate ()
    {
        foreach (CarNeuralCore car in cars)
        {
            car.DisableOnWallHit = disableOnWallHit;
        }
    }

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

    public int NewRandomCarsCount
    {
        get { return newRandomCarsCount; }
        set { newRandomCarsCount = value; }
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

    public float MutationProbability
    {
        get { return mutationProbability; }
        set { mutationProbability = value; }
    }

    public float AngleBetweenSensors
    {
        get { return angleBetweenSensors; }

        set
        { 
            angleBetweenSensors = value;

            foreach (CarNeuralCore car in cars)
            {
                car.SetAngleBetweenSensors (value);
            }
        }
    }

    public float SensorsLength
    {
        get { return sensorsLength; }

        set
        { 
            sensorsLength = value;

            foreach (CarNeuralCore car in cars)
            {
                car.SetSensorsLength (value);
            }
        }
    }


    private void Awake ()
    {
        DisableOnWallHit = disableOnWallHit;    
    }

    void OnEnable ()
    {
        //Init ();
    }

    public void Init ()
    {
        createCars ();
    }

    public void Pause ()
    {
        IsPaused = true;
    }

    public void Resume ()
    {
        IsPaused = false;
    }

    public void ResetCars ()
    {
        stopActivatingCarsCoroutineIfNeeded ();

        foreach (CarNeuralCore car in cars)
        {
            car.gameObject.transform.position = startPosition.position;
            car.gameObject.transform.forward = startPosition.forward;

            car.Reset ();
        }
    }

    public void ResetSimulation  ()
    {
        ResetCars ();
        createCars ();
    }

    public void SetSensorsVisible (bool visible)
    {
        foreach (CarNeuralCore car in cars)
        {
            car.SetSensorsVisible (visible);
        }
    }

    void createCars ()
    {
        if (carsCount < 0)
        {
            return;
        }

        System.Random rand = new System.Random ();

        for (int i = 0; i < carsCount; i ++)
        {
            if (i >= cars.Count)
            {
                createNewCarObject ();
            }

            cars [i].gameObject.transform.position = startPosition.position;
            cars [i].gameObject.transform.forward = startPosition.forward;
            cars [i].NeuronsInHiddenLayer = neuronsInHiddenLayer;
            cars [i].SensorsCount = sensorsCount;

            if (crossBreedSensors)
            {
                cars [i].AngleBetweenSensors = (float) rand.NextDouble () *MAX_ANGLE_BETWEEN_SENSORS;
                cars [i].SensorsLength = (float) rand.NextDouble () * MAX_SENSOR_LENGTH;
            }
            else
            {
                cars [i].AngleBetweenSensors = angleBetweenSensors;
                cars [i].SensorsLength = sensorsLength;
            }
            
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

        ActivateCars ();
    }

    public void ActivateCars ()
    {
        stopActivatingCarsCoroutineIfNeeded ();
        isActivatingCars = true;
        activatingCarsCoroutine = StartCoroutine (proceedActivateCars ());
    }

    void stopActivatingCarsCoroutineIfNeeded ()
    {
        if (activatingCarsCoroutine != null)
        {
            StopCoroutine (activatingCarsCoroutine);
            activatingCarsCoroutine = null;
        }

        isActivatingCars = false;
    }

    IEnumerator proceedActivateCars ()
    {
        for (int i = 0; i < cars.Count; i ++)
        {
            yield return new WaitForSeconds (0.1f);

            cars [i].IsActive = true;
        }

        isActivatingCars = false;
    }

    void createNewCarObject ()
    {
        GameObject newGameObject = Instantiate (carPrefab);
        newGameObject.SetActive (true);
        newGameObject.transform.SetParent (this.transform, false);
        CarNeuralCore carNeuralCore = newGameObject.GetComponent<CarNeuralCore> ();
        carNeuralCore.OnCarDisabled += onCarDisabled;
        carNeuralCore.OnCarClicked += onCarClicked;
        cars.Add (carNeuralCore);
    }

    void onCarClicked (CarNeuralCore carNeuralCore)
    {
        OnCarClicked?.Invoke (carNeuralCore);
    }

    void onCarDisabled (CarNeuralCore carNeuralCore)
    {
        CarFitness carFitness = carNeuralCore.GetComponent<CarFitness> ();
        carFitness.DistanceTravelled = stage.GetDistanceFromBeginning (carFitness.PosWhenDisabled);

        bool allDisabled = true;

        for (int i = 0; i < cars.Count; i ++)
        {
            if (cars [i].IsActive)
            {
                allDisabled = false;

                break;
            }
        }

        if (allDisabled && ! isActivatingCars)
        {
            onAllCarsDisabled ();
        }
    }

    void onAllCarsDisabled ()
    {
        Generation++;
        System.Random rnd = new System.Random ((int) DateTime.Now.Ticks);

        List<CarNeuralCore> sortedCars = getSortedByFitnessCarsList ();
        double [] fitnesses = new double [sortedCars.Count];
        List<CarSimpleData> newGenCars = new List<CarSimpleData> ();
        newGenCars.Add (sortedCars [0].GetCarSimpleData ());
        
        for (int i = 0; i < sortedCars.Count; i ++)
        {
            fitnesses [i] = sortedCars [i].GetComponent<CarFitness> ().Fitness;
        }

        float prevBestFitness = (float) prevBestCar.Fitness;
        float currentBestFitness = (float) fitnesses [0];

        if (fitnesses [0] > prevBestCar.Fitness)
        {
            prevBestCar = sortedCars [0].GetCarSimpleData ();
        }
        else if (prevBestCar.Weights != null)
        {
            newGenCars.Add (prevBestCar.GetCopy ());
        }

        if (AdaptiveMutationProbability)
        {
            if (prevBestFitness > currentBestFitness)
            {
                float d = currentBestFitness / prevBestFitness;
                d = 1f - d;
                mutationProbability += mutationProbabilityD * d;
            }
            else
            {
                mutationProbability = DEFAULT_MUTATION_PROBABILITY;
            }

            //Debug.Log ("prev best: " + prevBestFitness);
            //Debug.Log ("current best: " + currentBestFitness);
           
            mutationProbability = Mathf.Clamp (mutationProbability, 0.05f, 0.5f);
        }

        while (newGenCars.Count < carsCount - newRandomCarsCount)
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
            if (rnd.NextDouble () < mutationProbability)
            {
                genetics.Mutation (newGenCars [i].Weights, mutationProbability);
            }
        }

        if (CrossbreedSensors)
        {
            for (int i = 0; i < newGenCars.Count; i++)
            {
                if (rnd.NextDouble () < mutationProbability)
                {
                    float d = ((float) rnd.NextDouble () * SENSOR_LENGTH_D) - (SENSOR_LENGTH_D / 2f);
                    float newLenght = newGenCars [i].SensorsLength + d;

                    if (newLenght < 0)
                    {
                        newLenght = 0;
                    }

                    newGenCars [i].SensorsLength = newLenght;
                }
                else if (rnd.NextDouble () < mutationProbability)
                {
                    float d = ((float) rnd.NextDouble () * ANGLE_BETWEEN_SENSORS_D) - (ANGLE_BETWEEN_SENSORS_D / 2f);
                    float newAngle = newGenCars [i].AngleBetweenSensors + d;

                    if (newAngle < 0)
                    {
                        newAngle = 0;
                    }

                    newGenCars [i].AngleBetweenSensors = newAngle;
                }
            }
        }

        while (newGenCars.Count < carsCount)
        {
            CarSimpleData carSimpleData = new CarSimpleData ();
            carSimpleData.Weights = sortedCars [0].GetRandomNeuralNetworkWeights ();
            carSimpleData.AngleBetweenSensors = (float) rnd.NextDouble () * MAX_ANGLE_BETWEEN_SENSORS;
            carSimpleData.SensorsLength = (float) rnd.NextDouble () * MAX_SENSOR_LENGTH;

            newGenCars.Add (carSimpleData);
        }

        for (int i = 0; i < cars.Count; i ++)
        {
            cars [i].SetWeights (newGenCars [i].Weights);

            if (CrossbreedSensors)
            {
                cars [i].SetSensorsLength (newGenCars [i].SensorsLength);
                cars [i].SetAngleBetweenSensors (newGenCars [i].AngleBetweenSensors);
            }
        }

        ResetCars ();
        OnNewGenCreated?.Invoke ();
        ActivateCars ();
    }

    List <CarNeuralCore> getSortedByFitnessCarsList ()
    {
        return cars.OrderByDescending (c => c.GetComponent<CarFitness> ().Fitness).ToList ();
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
}
