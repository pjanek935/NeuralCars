using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeneticsUIController : MonoBehaviour
{
    [SerializeField] GeneticsManager geneticsManager;
    [SerializeField] Text generationText;
    [SerializeField] CarInspectorController carInspector;

    [SerializeField] Button resetCarsPositions;
    [SerializeField] Button resetAllCars;

    [SerializeField] Toggle showSensorsToggle;
    [SerializeField] Toggle crossbreedSensorsToggle;
    [SerializeField] Toggle disableCarsOnWallHit;
    [SerializeField] Toggle adaptiveMutationProbabilityToggle;
    [SerializeField] Toggle multiplyDistanceByVelocity;
    [SerializeField] Toggle explosionsToggle;

    [SerializeField] ValueController sensorsLengthController;
    [SerializeField] ValueController angleBetweenSensorsController;
    [SerializeField] ValueController mutationProbabilityController;
    [SerializeField] ValueController totalCarsCountController;
    [SerializeField] ValueController newRandomCarsEveryGenController;

    [SerializeField] ScrollbarValueController timeScaleController;

    [SerializeField] CameraController cameraController;
    [SerializeField] CameraFollow cameraFollow;

    const float MAX_SENSOR_LENGTH = 50f;
    const float MIN_SENSORS_LENGTH = 5f;
    const float SENSOR_LENGTH_D = 2.5f;
    const float MAX_ANGLE_BETWEEN_SENSORS = 60f;
    const float MIN_ANGLE_BEETWEEN_SENSORS = 1f;
    const float ANGLE_D = 1f;
    const float MAX_MUTATION_PROBABILITY = 0.5f;
    const float MIN_MUTATION_PROBABILITY = 0.025f;
    const float MUTATION_PROBABILITY_D = 0.025f;

    private void OnEnable ()
    {
        geneticsManager.OnNewGenCreated += onNewGenCreated;
        geneticsManager.OnCarClicked += onCarClicked;
        cameraFollow.OnDrag += onCameraFollowDrag;
    }

    private void OnDisable ()
    {
        geneticsManager.OnNewGenCreated -= onNewGenCreated;
        geneticsManager.OnCarClicked -= onCarClicked;
        cameraFollow.OnDrag -= onCameraFollowDrag;
    }

    private void Awake ()
    {
        resetCarsPositions.onClick.AddListener (() => onResetCarsPositionsButtonClicked ());
        resetAllCars.onClick.AddListener (() => onResetAllCarsClicked ());

        showSensorsToggle.onValueChanged.AddListener ((value) => onShowSensorValueChanged (value));
        crossbreedSensorsToggle.onValueChanged.AddListener ((value) => onCrossbreedSensorsToggleValueChanged (value));
        disableCarsOnWallHit.onValueChanged.AddListener ((value) => onDisableCarsOnWallHitValueChanged (value));
        adaptiveMutationProbabilityToggle.onValueChanged.AddListener ((value) => onAdaptiveMutationProbabilityToggleValueChanged (value));
        multiplyDistanceByVelocity.onValueChanged.AddListener ((value) => onMultiplyDistanceByVelocityValueChanged (value));
        explosionsToggle.onValueChanged.AddListener ((value) => onExpolosionsToggleValueChanged (value));

        sensorsLengthController.OnValueChanged += onSensorLengthValueChanged;
        angleBetweenSensorsController.OnValueChanged += onAngleBetweenSensorsValueChanged;
        mutationProbabilityController.OnValueChanged += onMutationProbabilityValueChanged;
        totalCarsCountController.OnValueChanged += totalCarsCountValueChanged;
        newRandomCarsEveryGenController.OnValueChanged += newRandomCarsValueChanged;

        sensorsLengthController.Setup (MAX_SENSOR_LENGTH, MIN_SENSORS_LENGTH, SENSOR_LENGTH_D);
        angleBetweenSensorsController.Setup (MAX_ANGLE_BETWEEN_SENSORS, MIN_ANGLE_BEETWEEN_SENSORS, ANGLE_D);
        mutationProbabilityController.Setup (MAX_MUTATION_PROBABILITY, MIN_MUTATION_PROBABILITY, MUTATION_PROBABILITY_D);
        totalCarsCountController.Setup (100, 10, 1);
        newRandomCarsEveryGenController.Setup (10, 0, 1);

        multiplyDistanceByVelocity.isOn = geneticsManager.FitnessType == FitnessType.DIST_MUL_SPEED;
        crossbreedSensorsToggle.isOn = geneticsManager.CrossbreedSensors;
        sensorsLengthController.SetValue (geneticsManager.SensorsLength);
        angleBetweenSensorsController.SetValue (geneticsManager.AngleBetweenSensors);
        onCrossbreedSensorsToggleValueChanged (geneticsManager.CrossbreedSensors);
        disableCarsOnWallHit.isOn = geneticsManager.DisableOnWallHit;
        adaptiveMutationProbabilityToggle.isOn = geneticsManager.AdaptiveMutationProbability;
        onAdaptiveMutationProbabilityToggleValueChanged (geneticsManager.AdaptiveMutationProbability);
        newRandomCarsEveryGenController.SetValue (geneticsManager.NewRandomCarsCount);
        totalCarsCountController.SetValue (geneticsManager.CarsCount);
        explosionsToggle.isOn = geneticsManager.Explosions;

        mutationProbabilityController.Format = "0.00";

        timeScaleController.OnValueChanged += onTimeScaleValueChanged;
        timeScaleController.SetValue (TimeScaleController.TargetTimeScale);

        RefreshViews ();
    }

    void onResetAllCarsClicked ()
    {
        PopupWithMessage.Instance.Show ("ARE YOU SURE WANT TO RESET?", true, true, onResetAllCarsConfirmed);
    }

    void onResetAllCarsConfirmed (bool confirmed)
    {
        if (confirmed)
        {
            geneticsManager.ResetSimulation ();
            geneticsManager.ActivateCars ();
            RefreshViews ();
        }
    }

    void onResetCarsPositionsButtonClicked ()
    {
        geneticsManager.ResetCars ();
        geneticsManager.ActivateCars ();
    }

    void onNewGenCreated ()
    {
        RefreshViews ();
    }

    void onCameraFollowDrag ()
    {
        carInspector.SetCarToFollow (null);
        cameraFollow.enabled = false;
        cameraController.enabled = true;
    }

    void onCarClicked (CarNeuralCoreBase carNeuralCoreBase)
    {
        carInspector.SetCarToFollow (carNeuralCoreBase);
        cameraFollow.enabled = true;
        cameraController.enabled = false;
        cameraFollow.SetTarget (carNeuralCoreBase.transform);
    }

    public void RefreshViews ()
    {
        generationText.text = "GENERATION " + geneticsManager.Generation.ToString ("D3");
        mutationProbabilityController.SetValue (geneticsManager.MutationProbability);
    }

    void onShowSensorValueChanged (bool value)
    {
        geneticsManager.SetSensorsVisible (value);
    }

    void onCrossbreedSensorsToggleValueChanged (bool value)
    {
        if (value)
        {
            sensorsLengthController.Disable ();
            angleBetweenSensorsController.Disable ();
        }
        else
        {
            sensorsLengthController.Enable ();
            angleBetweenSensorsController.Enable ();
        }

        geneticsManager.CrossbreedSensors = value;
    }

    void onExpolosionsToggleValueChanged (bool value)
    {
        geneticsManager.Explosions = value;
    }

    void onMultiplyDistanceByVelocityValueChanged (bool value)
    {
        geneticsManager.FitnessType = value ? FitnessType.DIST_MUL_SPEED : FitnessType.ONLY_DISTANCE;
    }

    void onAdaptiveMutationProbabilityToggleValueChanged (bool value)
    {
        if (value)
        {
            mutationProbabilityController.Disable ();
        }
        else
        {
            mutationProbabilityController.Enable ();
        }

        geneticsManager.AdaptiveMutationProbability = value;
    }

    void onSensorLengthValueChanged (float newLength)
    {
        geneticsManager.SensorsLength = newLength;
    }

    void onAngleBetweenSensorsValueChanged (float newAngle)
    {
        geneticsManager.AngleBetweenSensors = newAngle;
    }

    void onDisableCarsOnWallHitValueChanged (bool value)
    {
        geneticsManager.DisableOnWallHit = value;
    }

    void onMutationProbabilityValueChanged (float newValue)
    {
        geneticsManager.MutationProbability = newValue;
    }

    void totalCarsCountValueChanged (float newValue)
    {
        geneticsManager.CarsCount = (int) newValue;
    }

    void newRandomCarsValueChanged (float newValue)
    {
        geneticsManager.NewRandomCarsCount = (int) newValue;
    }

    void onTimeScaleValueChanged (float value)
    {
        TimeScaleController.SetTimeScale (value);
    }
}
