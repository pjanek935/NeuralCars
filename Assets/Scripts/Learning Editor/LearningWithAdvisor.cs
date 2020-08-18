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

    private void OnEnable ()
    {
        car.OnLastGatePassed += onLastGatePassed;
    }

    private void OnDisable ()
    {
        car.OnLastGatePassed -= onLastGatePassed;
    }

    void onLastGatePassed ()
    {
        List<double []> trainingData = car.TrainingData;

        if (trainingData != null && trainingData.Count > 0)
        {
            car.Train (trainingData);

            PopupWithMessage.Instance.Show ("Training finished. Use trained data?", true, true, onTrainingFinishedPopupClosed);
        }
    }

    void onTrainingFinishedPopupClosed (bool confirmed)
    {
        if (confirmed)
        {
            int carsCount = geneticsManager.CarsCount;
            List<CarSimpleData> cars = new List<CarSimpleData> ();
            CarSimpleData template = car.GetCarSimpleData ();

            for (int i = 0; i < carsCount; i ++)
            {
                cars.Add (template.GetCopy ());
            }

            geneticsManager.SetNewCars (cars);
        }
        else
        {

        }
    }

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
