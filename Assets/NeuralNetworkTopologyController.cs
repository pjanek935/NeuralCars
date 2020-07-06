using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkTopologyController : MonoBehaviour
{
    [SerializeField] GameObject neuronPrefab;

    [SerializeField] ValueController sensorsCountController;
    [SerializeField] ValueController hiddenLayerNeuronsCountController;

    [SerializeField] Transform sensorsContainer;
    [SerializeField] Transform hiddenLayerNeuronsContainer;

    List<GameObject> sensorNeurons = new List<GameObject> ();
    List<GameObject> hiddenLayerNeurons = new List<GameObject> ();

    [SerializeField] NeuronToggle angleBetweenForwardVectorAndMovementDirectionInputToggle;
    [SerializeField] NeuronToggle velocityInputToggle;
    [SerializeField] NeuronToggle torqueInputToggle;
    [SerializeField] NeuronToggle steerAngleInputToggle;

    [SerializeField] NeuronToggle torqueOutputToggle;
    [SerializeField] NeuronToggle steerAngleOutputToggle;
    [SerializeField] NeuronToggle handbrakeOutputToggle;

    private void Awake ()
    {
        sensorsCountController.Setup (20, 2, 1);
        sensorsCountController.SetValue (1);
        sensorsCountController.OnValueChanged += onSensorsCountValueChanged;

        hiddenLayerNeuronsCountController.Setup (20, 1, 1);
        hiddenLayerNeuronsCountController.SetValue (1);
        hiddenLayerNeuronsCountController.OnValueChanged += hiddenLayerNeuronsCountValueChanged;
    }

    void onSensorsCountValueChanged (float newVal)
    {
        createNeurons ((int) newVal, sensorNeurons, sensorsContainer);
    }

    void hiddenLayerNeuronsCountValueChanged (float newVal)
    {
        createNeurons ((int) newVal, hiddenLayerNeurons, hiddenLayerNeuronsContainer);
    }

    void createNeurons (int count, List <GameObject> list, Transform container)
    {
        if (count < 0 || list == null || container == null)
        {
            return;
        }

        int diff = count - list.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                GameObject newGameObject = Instantiate (neuronPrefab);
                newGameObject.SetActive (true);
                newGameObject.transform.SetParent (container);
                list.Add (newGameObject);
            }
        }
        else if (diff < 0)
        {
            diff = Mathf.Abs (diff);

            for (int i = 0; i < diff; i++)
            {
                GameObject tmp = list [list.Count - 1];
                Destroy (tmp);
                list.RemoveAt (list.Count - 1);
            }
        }
    }
}
