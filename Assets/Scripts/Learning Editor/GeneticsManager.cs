using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class GeneticsManager : MonoBehaviour
{
    public UnityAction OnNewGenCreated;
    public event CarNeuralCoreBase.CarNeuralCoreEventHandler OnCarClicked;

    [SerializeField] Transform startPosition;
    [SerializeField] GameObject carPrefab;
    [SerializeField] Stage stage;

    [SerializeField] GameObject lastBestGameObject;
    [SerializeField] GameObject allTimeBest;

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
    bool isActivatingCars = false;
    FitnessType fitnessType = FitnessType.DIST_MUL_SPEED;
    Coroutine activatingCarsCoroutine;
    NetworkTopologySimpleData currentTopology = new NetworkTopologySimpleData ();
    CarSimpleData prevCarWithHighestFitness = new CarSimpleData ();
    CarSimpleData prevCarWithFurthestDistTravelled = new CarSimpleData (); //distance is usuall multiplied by speed in fitness function,
    //so it's not identical to prevCarWithHighestFitness


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

    public FitnessType FitnessType
    {
        get
        { 
            return fitnessType;
        }

        set
        {
            fitnessType = value;
            refreshCarsFitness ();
        }
    }

    public NetworkTopologySimpleData CurrentTopology
    {
        get { return currentTopology; }
    }

    public List <CarSimpleData> GetCarSimpleData ()
    {
        List<CarSimpleData> result = new List<CarSimpleData> ();

        for (int i = 0; i < cars.Count; i ++)
        {
            result.Add (cars [i].GetCarSimpleData ());
        }

        return result;
    }

    public void SetNewCars (List<CarSimpleData> carSimpleData)
    {
        if (carSimpleData != null)
        {
            this.carsCount = carSimpleData.Count;
            createCars ();

            for (int i = 0; i < cars.Count; i++)
            {
                cars [i].SetWeights (carSimpleData [i].Weights);

                if (CrossbreedSensors)
                {
                    cars [i].SetSensorsLength (carSimpleData [i].SensorsLength);
                    cars [i].SetAngleBetweenSensors (carSimpleData [i].AngleBetweenSensors);
                }
            }
        }
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

    public void SetNetworkTopology (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            currentTopology = networkTopology.GetCopy ();
        }
    }

    private void Awake ()
    {
        DisableOnWallHit = disableOnWallHit;
    }

    public void SetDefaultNetworkTopology ()
    {
        currentTopology.HiddenLayerNeuronsCount = NeuronsInHiddenLayer;
        currentTopology.SensorsCount = SensorsCount;
        currentTopology.MovementAngleInput = false;
        currentTopology.SteerAngleInput = false;
        currentTopology.TorqueInput = false;
        currentTopology.VelocityInput = false;
        currentTopology.HandbrakeOutput = false;
        currentTopology.TorqueOutput = false;
    }

    public void Init ()
    {
        SetDefaultNetworkTopology ();
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
        Generation = 1;
        prevCarWithHighestFitness = new CarSimpleData ();
        prevCarWithFurthestDistTravelled = new CarSimpleData ();
        lastBestGameObject.SetActive (false);
        allTimeBest.SetActive (false);
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

    void refreshCarsFitness ()
    {
        cars.ForEach ((c) => c.GetComponent<CarFitness> ().FitnessType = this.FitnessType);
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
            
            cars [i].Init (currentTopology);
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

    CarNeuralCore createNewCarObject ()
    {
        GameObject newGameObject = Instantiate (carPrefab);
        newGameObject.SetActive (true);
        newGameObject.transform.SetParent (this.transform, false);
        newGameObject.GetComponent<CarFitness> ().FitnessType = fitnessType;
        CarNeuralCore carNeuralCore = newGameObject.GetComponent<CarNeuralCore> ();
        carNeuralCore.OnCarDisabled += onCarDisabled;
        carNeuralCore.OnCarClicked += onCarClicked;
        cars.Add (carNeuralCore);

        return carNeuralCore;
    }

    void onCarClicked (CarNeuralCoreBase carNeuralCoreBase)
    {
        OnCarClicked?.Invoke (carNeuralCoreBase);
    }

    void onCarDisabled (CarNeuralCoreBase carNeuralCoreBase)
    {
        CarFitness carFitness = carNeuralCoreBase.GetComponent<CarFitness> ();

        if (carFitness != null)
        {
            carFitness.DistanceTravelled = stage.GetDistanceFromBeginning (carFitness.PosWhenDisabled);
            carFitness.RotationWhenDisabled = carNeuralCoreBase.transform.rotation;

            bool allDisabled = true;

            for (int i = 0; i < cars.Count; i++)
            {
                if (cars [i].IsActive)
                {
                    allDisabled = false;

                    break;
                }
            }

            if (allDisabled && !isActivatingCars)
            {
                onAllCarsDisabled ();
            }
        }
    }

    void refreshGhostCarsPositions (List<CarNeuralCore> sortedCars)
    {
        if (sortedCars != null && sortedCars.Count > 0)
        {
            if (prevCarWithFurthestDistTravelled != null)
            {
                CarFitness carFitness = sortedCars [0].GetComponent<CarFitness> ();

                if (carFitness.DistanceTravelled > prevCarWithFurthestDistTravelled.DistTravelled)
                {
                    lastBestGameObject.SetActive (false);
                    allTimeBest.SetActive (true);
                    allTimeBest.transform.position = carFitness.PosWhenDisabled;
                    allTimeBest.transform.rotation = carFitness.RotationWhenDisabled;
                }
                else
                {
                    lastBestGameObject.SetActive (true);
                    lastBestGameObject.transform.position = carFitness.PosWhenDisabled;
                    lastBestGameObject.transform.rotation = carFitness.RotationWhenDisabled;
                }
            }
        }
        else
        {
            allTimeBest.SetActive (false);
            lastBestGameObject.SetActive (false);
        }
    }

    void onAllCarsDisabled ()
    {
        Generation++;
        System.Random rnd = new System.Random ((int) DateTime.Now.Ticks);

        List<CarNeuralCore> sortedCarsByFitness = getCarsSortedByFitness ();
        List<CarNeuralCore> sortedCarsByDistace = getCarsSortedByDistTravelled ();

        refreshGhostCarsPositions (sortedCarsByDistace);
        double [] fitnesses = new double [sortedCarsByFitness.Count];
        List<CarSimpleData> newGenCars = new List<CarSimpleData> ();
        newGenCars.Add (sortedCarsByFitness [0].GetCarSimpleData ());
        
        for (int i = 0; i < sortedCarsByFitness.Count; i ++)
        {
            fitnesses [i] = sortedCarsByFitness [i].GetComponent<CarFitness> ().Fitness;
        }

        float prevBestDistance = prevCarWithFurthestDistTravelled.DistTravelled;
        float prevBestFitness = (float) prevCarWithHighestFitness.Fitness;
        float currentBestFitness = (float) fitnesses [0];
        float currentBestDistance = sortedCarsByDistace [0].GetComponent<CarFitness> ().DistanceTravelled;

        if (currentBestFitness > prevCarWithHighestFitness.Fitness)
        {
            prevCarWithHighestFitness = sortedCarsByFitness [0].GetCarSimpleData ();
        }
        else if (prevCarWithHighestFitness.Weights != null)
        {
            newGenCars.Add (prevCarWithHighestFitness.GetCopy ());
        }

        if (currentBestDistance > prevCarWithFurthestDistTravelled.DistTravelled)
        {
            prevCarWithFurthestDistTravelled = sortedCarsByDistace [0].GetCarSimpleData ();
        }
        else if (sortedCarsByDistace [0] != sortedCarsByFitness [0] && prevCarWithFurthestDistTravelled.Weights != null)
        {
            newGenCars.Add (prevCarWithFurthestDistTravelled.GetCopy ());
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
            genetics.CrossoverCars (sortedCarsByFitness [parent1index].GetCarSimpleData (), sortedCarsByFitness [parent2index].GetCarSimpleData (), out c1, out c2, Genetics.CrossType.ARYTM);

            newGenCars.Add (c1);
            newGenCars.Add (c2);
        }

        for (int i = 0; i < newGenCars.Count; i++)
        {
            //different approach
            //if (rnd.NextDouble () < mutationProbability)
            //{
            //    genetics.Mutation (newGenCars [i].Weights, mutationProbability);
            //}

            genetics.Mutation (newGenCars [i].Weights, mutationProbability);
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
            carSimpleData.Weights = sortedCarsByFitness [0].GetRandomNeuralNetworkWeights ();
            carSimpleData.AngleBetweenSensors = (float) rnd.NextDouble () * MAX_ANGLE_BETWEEN_SENSORS;
            carSimpleData.SensorsLength = (float) rnd.NextDouble () * MAX_SENSOR_LENGTH;

            newGenCars.Add (carSimpleData);
        }

        int diff = newGenCars.Count - cars.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                CarNeuralCore carNeuralCore = createNewCarObject ();
                carNeuralCore.Init (currentTopology);
            }
        }

        for (int i = 0; i < newGenCars.Count; i ++)
        {
            cars [i].SetWeights (newGenCars [i].Weights);

            if (CrossbreedSensors)
            {
                cars [i].SetSensorsLength (newGenCars [i].SensorsLength);
                cars [i].SetAngleBetweenSensors (newGenCars [i].AngleBetweenSensors);
            }
        }

        diff = cars.Count - newGenCars.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                deleteLastCarObject ();
            }
        }

        ResetCars ();
        OnNewGenCreated?.Invoke ();
        ActivateCars ();
    }

    List <CarNeuralCore> getCarsSortedByFitness ()
    {
        return cars.OrderByDescending (c => c.GetComponent<CarFitness> ().Fitness).ToList ();
    }

    List<CarNeuralCore> getCarsSortedByDistTravelled ()
    {
        return cars.OrderByDescending (c => c.GetComponent<CarFitness> ().DistanceTravelled).ToList ();
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
