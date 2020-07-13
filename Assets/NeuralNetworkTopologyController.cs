using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NeuralNetworkTopologyController : MonoBehaviour
{
    [SerializeField] GameObject neuronPrefab;
    [SerializeField] GameObject linePrefab;

    [SerializeField] Button resetButton;

    [SerializeField] ValueController sensorsCountController;
    [SerializeField] ValueController hiddenLayerNeuronsCountController;

    [SerializeField] Transform sensorsContainer;
    [SerializeField] Transform hiddenLayerNeuronsContainer;
    [SerializeField] Transform linesContainer;

    List<GameObject> sensorNeurons = new List<GameObject> ();
    List<GameObject> hiddenLayerNeurons = new List<GameObject> ();
    List<UILineRenderer> lines = new List<UILineRenderer> ();

    [SerializeField] NeuronToggle angleBetweenForwardVectorAndMovementDirectionInputToggle;
    [SerializeField] NeuronToggle velocityInputToggle;
    [SerializeField] NeuronToggle torqueInputToggle;
    [SerializeField] NeuronToggle steerAngleInputToggle;

    [SerializeField] NeuronToggle torqueOutputToggle;
    [SerializeField] NeuronToggle steerAngleOutputToggle;
    [SerializeField] NeuronToggle handbrakeOutputToggle;

    Coroutine refreshLinesCoroutine = null;
    NetworkTopologySimpleData defaultTopology = new NetworkTopologySimpleData ();

    private void Awake ()
    {
        //resetButton.onClick.AddListener (() => onResetClicked ());

        sensorsCountController.Setup (20, 2, 1);
        sensorsCountController.SetValue (1);
        sensorsCountController.OnValueChanged += onSensorsCountValueChanged;

        hiddenLayerNeuronsCountController.Setup (20, 1, 1);
        hiddenLayerNeuronsCountController.SetValue (1);
        hiddenLayerNeuronsCountController.OnValueChanged += hiddenLayerNeuronsCountValueChanged;

        angleBetweenForwardVectorAndMovementDirectionInputToggle.OnValueChanged += onToggleValueChanged;
        velocityInputToggle.OnValueChanged += onToggleValueChanged;
        torqueInputToggle.OnValueChanged += onToggleValueChanged;
        steerAngleInputToggle.OnValueChanged += onToggleValueChanged;

        torqueOutputToggle.OnValueChanged += onToggleValueChanged;
        steerAngleOutputToggle.OnValueChanged += onToggleValueChanged;
        handbrakeOutputToggle.OnValueChanged += onToggleValueChanged;
    }

    public NetworkTopologySimpleData GetNetworkTopologySimpleData ()
    {
        NetworkTopologySimpleData result = new NetworkTopologySimpleData ();

        result.MovementAngleInput = angleBetweenForwardVectorAndMovementDirectionInputToggle.IsOn;
        result.SteerAngleInput = steerAngleInputToggle.IsOn;
        result.TorqueInput = torqueInputToggle.IsOn;
        result.VelocityInput = velocityInputToggle.IsOn;

        result.SensorsCount = sensorNeurons.Count;
        result.HiddenLayerNeuronsCount = hiddenLayerNeurons.Count;

        result.TorqueOutput = torqueOutputToggle.IsOn;
        result.HandbrakeOutput = handbrakeOutputToggle.IsOn;

        return result;
    }

    public void Init (NetworkTopologySimpleData networkTopologySimpleData)
    {
        if (networkTopologySimpleData != null)
        {
            defaultTopology = networkTopologySimpleData.GetCopy ();

            hiddenLayerNeuronsCountController.SetValue (networkTopologySimpleData.HiddenLayerNeuronsCount);
            sensorsCountController.SetValue (networkTopologySimpleData.SensorsCount);

            angleBetweenForwardVectorAndMovementDirectionInputToggle.IsOn = networkTopologySimpleData.MovementAngleInput;
            velocityInputToggle.IsOn = networkTopologySimpleData.VelocityInput;
            torqueInputToggle.IsOn = networkTopologySimpleData.TorqueInput;
            steerAngleInputToggle.IsOn = networkTopologySimpleData.SteerAngleInput;

            torqueOutputToggle.IsOn = networkTopologySimpleData.TorqueOutput;
            handbrakeOutputToggle.IsOn = networkTopologySimpleData.HandbrakeOutput;

            createNeurons (networkTopologySimpleData.SensorsCount, sensorNeurons, sensorsContainer);
            createNeurons (networkTopologySimpleData.HiddenLayerNeuronsCount, hiddenLayerNeurons, hiddenLayerNeuronsContainer);
            startNewRefreshLinesCoroutine ();
        }
    }

    void onToggleValueChanged (NeuronToggle sender, bool isOn)
    {
        refreshLines ();
    }

    void onSensorsCountValueChanged (float newVal)
    {
        createNeurons ((int) newVal, sensorNeurons, sensorsContainer);
        startNewRefreshLinesCoroutine ();
    }

    void hiddenLayerNeuronsCountValueChanged (float newVal)
    {
        createNeurons ((int) newVal, hiddenLayerNeurons, hiddenLayerNeuronsContainer);
        startNewRefreshLinesCoroutine ();
    }

    void onResetClicked ()
    {
        Init (defaultTopology);
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

    IEnumerator waitForNextFrameAndInvoke (UnityAction action)
    {
        if (action != null)
        {
            yield return new WaitForEndOfFrame ();
            action.Invoke ();
        }
    }

    void stopRefreshLineCoroutineIfNeeded ()
    {
        if (refreshLinesCoroutine != null)
        {
            StopCoroutine (refreshLinesCoroutine);
            refreshLinesCoroutine = null;
        }
    }

    /// <summary>
    /// Invokes refreshLines methods after one frame delay - it's to give layouter time to position elements in UI
    /// </summary>
    void startNewRefreshLinesCoroutine ()
    {
        stopRefreshLineCoroutineIfNeeded ();
        refreshLinesCoroutine = StartCoroutine (waitForNextFrameAndInvoke (refreshLines));
    }

    /// <summary>
    /// Immediately creates new lines according to current neurons (input, hidden, output)
    /// </summary>
    void refreshLines ()
    {
        List<Vector2> starts = new List<Vector2> ();
        List<Vector2> ends = new List<Vector2> ();

        if (angleBetweenForwardVectorAndMovementDirectionInputToggle.IsOn)
        {
            addLineFromAInputNeuron (angleBetweenForwardVectorAndMovementDirectionInputToggle, starts, ends);
        }

        if (velocityInputToggle.IsOn)
        {
            addLineFromAInputNeuron (velocityInputToggle, starts, ends);
        }

        if (torqueInputToggle.IsOn)
        {
            addLineFromAInputNeuron (torqueInputToggle, starts, ends);
        }

        if (steerAngleInputToggle.IsOn)
        {
            addLineFromAInputNeuron (steerAngleInputToggle, starts, ends);
        }

        for (int i = 0; i < sensorNeurons.Count; i ++)
        {
            for (int j = 0; j < hiddenLayerNeurons.Count; j++)
            {
                starts.Add (new Vector2 (sensorNeurons [i].transform.position.x, sensorNeurons [i].transform.position.y));
                ends.Add (new Vector2 (hiddenLayerNeurons [j].transform.position.x, hiddenLayerNeurons [j].transform.position.y));
            }
        }

        for (int i = 0; i < hiddenLayerNeurons.Count; i++)
        {
            if (torqueOutputToggle.IsOn)
            {
                starts.Add (new Vector2 (hiddenLayerNeurons [i].transform.position.x, hiddenLayerNeurons [i].transform.position.y));
                ends.Add (new Vector2 (torqueOutputToggle.transform.position.x, torqueOutputToggle.transform.position.y));
            }

            if (steerAngleOutputToggle.IsOn)
            {
                starts.Add (new Vector2 (hiddenLayerNeurons [i].transform.position.x, hiddenLayerNeurons [i].transform.position.y));
                ends.Add (new Vector2 (steerAngleOutputToggle.transform.position.x, steerAngleOutputToggle.transform.position.y));
            }

            if (handbrakeOutputToggle.IsOn)
            {
                starts.Add (new Vector2 (hiddenLayerNeurons [i].transform.position.x, hiddenLayerNeurons [i].transform.position.y));
                ends.Add (new Vector2 (handbrakeOutputToggle.transform.position.x, handbrakeOutputToggle.transform.position.y));
            }
        }

        for (int i = 0; i < starts.Count; i ++)
        {
            if (i >= lines.Count)
            {
                GameObject newLine = Instantiate (linePrefab);
                newLine.gameObject.SetActive (true);
                newLine.transform.SetParent (linesContainer);
                lines.Add (newLine.GetComponent <UILineRenderer> ());
            }

            lines [i].SetPoints (starts [i], ends [i]);
        }

        int diff = lines.Count - starts.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                GameObject tmp = lines [lines.Count - 1].gameObject;
                lines.RemoveAt (lines.Count - 1);
                Destroy (tmp);
            }
        }
    }

    void addLineFromAInputNeuron (NeuronToggle inputToggle, List<Vector2> starts, List<Vector2> ends)
    {
        if (inputToggle == null || starts == null || ends == null)
        {
            return;
        }

        for (int j = 0; j < hiddenLayerNeurons.Count; j++)
        {
            starts.Add (new Vector2 (inputToggle.transform.position.x, inputToggle.transform.position.y));
            ends.Add (new Vector2 (hiddenLayerNeurons [j].transform.position.x, hiddenLayerNeurons [j].transform.position.y));
        }
    }
}
