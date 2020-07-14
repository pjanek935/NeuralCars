using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarInspectorController : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] CarNeuralCore carToFollow;
    [SerializeField] Stage stage;

    [SerializeField] Text distTravelled;
    [SerializeField] Text avgSpeed;
    [SerializeField] Text fitness;
    [SerializeField] CanvasGroup canvasGroup;

    CarFitness carFitness;

    public bool IsVisible
    {
        get;
        private set;
    }

    public void SetCarToFollow (CarNeuralCore carToFollow)
    {
        this.carToFollow = carToFollow;

        if (carToFollow != null)
        {
            show ();
        }
        else
        {
            hide ();
        }
    }

    void cacheCarFitnessIfNeeded ()
    {
        if (carToFollow != null)
        {
            carFitness = carToFollow.GetComponent<CarFitness> ();
        }
    }

    void show ()
    {
        IsVisible = true;
        this.gameObject.SetActive (true);
    }

    void hide ()
    {
        IsVisible = false;
        this.gameObject.SetActive (false);
    }

    private void FixedUpdate ()
    {
        if (carToFollow != null && camera != null)
        {
            if (! IsVisible)
            {
                show ();
            }

            Vector3 carPos = carToFollow.transform.position;
            Vector2 screenPos = camera.WorldToScreenPoint (carPos);
            this.transform.position = screenPos;

            if (Time.frameCount % 2 == 0)
            {
                cacheCarFitnessIfNeeded ();
                float dist = stage.GetDistanceFromBeginning (carPos);
                int fit = CarFitness.CalculateFitness (carFitness.GatesPassed, dist, carFitness.AvgVelocity);

                distTravelled.text = (dist).ToString ("0.00");
                avgSpeed.text = (carFitness.AvgVelocity).ToString ("0.00");
                fitness.text = (fit).ToString ();
            }
        }
        else if (IsVisible)
        {
            hide ();
        }
    }
}
