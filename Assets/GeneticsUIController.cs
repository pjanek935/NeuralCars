using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeneticsUIController : MonoBehaviour
{
    [SerializeField] GeneticsManager geneticsManager;

    [SerializeField] Button pauseButton;
    [SerializeField] Button playButton;
    [SerializeField] Toggle showSensorsToggle;
    [SerializeField] Toggle crossbreedSensorsToggle;

    [SerializeField] ValueController sensorsLengthController;
    [SerializeField] ValueController angleBetweenSensorsController;

    const float MAX_SENSOR_LENGTH = 50f;
    const float MIN_SENSORS_LENGTH = 5f;
    const float SENSOR_LENGTH_D = 2.5f;
    const float MAX_ANGLE_BETWEEN_SENSORS = 20f;
    const float MIN_ANGLE_BEETWEEN_SENSORS = 1f;
    const float ANGLE_D = 1f;

    private void Awake ()
    {
        pauseButton.onClick.AddListener (() => onPauseButtonClicked ());
        playButton.onClick.AddListener (() => onPlayButtonClicked ());
        showSensorsToggle.onValueChanged.AddListener ((value) => onShowSensorValueChanged (value));
        crossbreedSensorsToggle.onValueChanged.AddListener ((value) => onCrossbreedSensorsToggleValueChanged (value));
        sensorsLengthController.Setup (MAX_SENSOR_LENGTH, MIN_SENSORS_LENGTH, SENSOR_LENGTH_D);
        angleBetweenSensorsController.Setup (MAX_ANGLE_BETWEEN_SENSORS, MIN_ANGLE_BEETWEEN_SENSORS, ANGLE_D);
        sensorsLengthController.OnValueChanged += onSensorLengthValueChanged;
        angleBetweenSensorsController.OnValueChanged += onAngleBetweenSensorsValueChanged;
    }

    void onPlayButtonClicked ()
    {
        playButton.gameObject.SetActive (false);
        pauseButton.gameObject.SetActive (true);

        geneticsManager.Resume ();
    }

    void onPauseButtonClicked ()
    {
        playButton.gameObject.SetActive (true);
        pauseButton.gameObject.SetActive (false);

        geneticsManager.Pause ();
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
    }

    void onSensorLengthValueChanged (float newLength)
    {
        geneticsManager.SetSensorsLength (newLength);
    }

    void onAngleBetweenSensorsValueChanged (float newAngle)
    {
        geneticsManager.SetAngleBetweenSensors (newAngle);
    }
}
