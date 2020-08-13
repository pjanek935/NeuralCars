using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningWithAdvisor : MonoBehaviour
{
    [SerializeField] BackPropNeuralCore car;
    [SerializeField] Stage stage;
    [SerializeField] CameraController cameraController;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] Transform startPosition;
    [SerializeField] GeneticsManager geneticsManager;

    public void Enter ()
    {
        car.Reset ();
        car.AngleBetweenSensors = geneticsManager.AngleBetweenSensors;
        car.SensorsLength = geneticsManager.SensorsLength;
        car.Init (geneticsManager.CurrentTopology);
        car.gameObject.SetActive (true);
        cameraFollow.SetTarget (car.transform);
        cameraFollow.enabled = true;
        cameraController.enabled = false;
        car.transform.position = startPosition.position;
        car.transform.rotation = startPosition.rotation;
    }

    public void Exit ()
    {
        car.gameObject.SetActive (false);
        cameraFollow.enabled = false;
        cameraController.enabled = true;
    }
}
