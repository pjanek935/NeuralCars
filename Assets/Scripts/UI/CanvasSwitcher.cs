using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] Button stageEditorButton;
    [SerializeField] Button neuralNetowrkButton;
    [SerializeField] Button networkTopologyButton;
    [SerializeField] Button backToNeuralButton;

    [SerializeField] GameObject stageEditorCanvas;
    [SerializeField] GameObject neuralNetworkCanvas;
    [SerializeField] GameObject networkTopologyCanvas;

    [SerializeField] StageEditor stageEditor;
    [SerializeField] GeneticsManager geneticsManager;
    [SerializeField] NeuralNetworkTopologyController networkTopologyController;
    [SerializeField] GeneticsUIController geneticsUIController;

    bool wasPaused = false;

    private void Awake ()
    {
        stageEditorButton.onClick.AddListener (() => onStageEditorButtonClicked ());
        neuralNetowrkButton.onClick.AddListener (() => onNeuralNetworkButtonClicked ());
        networkTopologyButton.onClick.AddListener (() => onNetworkTopologyButtonClicked ());
        backToNeuralButton.onClick.AddListener (() => backToNeural ());

        onNeuralNetworkButtonClicked ();
        geneticsManager.Init ();
    }

    void onStageEditorButtonClicked ()
    {
        stageEditor.EnableStageEditor ();
        stageEditorCanvas.SetActive (true);
        neuralNetworkCanvas.SetActive (false);

        if (geneticsManager.IsPaused)
        {
            wasPaused = true;
            geneticsManager.Resume ();
            geneticsManager.gameObject.SetActive (false);
        }
        else
        {
            wasPaused = false;
            geneticsManager.gameObject.SetActive (false);
        }
    }

    void onNeuralNetworkButtonClicked ()
    {
        stageEditor.DisableStageEditor ();
        stageEditorCanvas.SetActive (false);
        neuralNetworkCanvas.SetActive (true);

        if (wasPaused)
        {
            geneticsManager.Pause ();
        }

        geneticsManager.gameObject.SetActive (true);
        geneticsManager.ResetCars ();
        geneticsManager.ActivateCars ();
    }

    void onNetworkTopologyButtonClicked ()
    {
        neuralNetworkCanvas.SetActive (false);
        networkTopologyCanvas.SetActive (true);
        networkTopologyController.Init (geneticsManager.CurrentTopology);

        if (geneticsManager.IsPaused)
        {
            wasPaused = true;
            geneticsManager.Resume ();
            geneticsManager.gameObject.SetActive (false);
        }
        else
        {
            wasPaused = false;
            geneticsManager.gameObject.SetActive (false);
        }
    }

    void backToNeural ()
    {
        networkTopologyCanvas.SetActive (false);
        neuralNetworkCanvas.SetActive (true);

        NetworkTopologySimpleData currentTopology = geneticsManager.CurrentTopology;
        NetworkTopologySimpleData newTopology = networkTopologyController.GetNetworkTopologySimpleData ();
        bool isTpologyDifferent = currentTopology.IsDifferent (newTopology);

        if (isTpologyDifferent)
        {
            geneticsManager.SetNetworkTopology (newTopology);
        }
      
        if (wasPaused)
        {
            geneticsManager.Pause ();
        }

        geneticsManager.gameObject.SetActive (true);

        if (isTpologyDifferent)
        {
            geneticsManager.ResetSimulation ();
        }
        else
        {
            geneticsManager.ResetCars ();
            geneticsManager.ActivateCars ();
        }

        geneticsUIController.RefreshViews ();
    }
}
