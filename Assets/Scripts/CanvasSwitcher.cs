using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] Button stageEditorButton;
    [SerializeField] Button neuralNetowrkButton;

    [SerializeField] GameObject stageEditorCanvas;
    [SerializeField] GameObject neuralNetworkCanvas;

    [SerializeField] StageEditor stageEditor;
    [SerializeField] GeneticsManager geneticsManager;

    bool wasPaused = false;

    private void Awake ()
    {
        stageEditorButton.onClick.AddListener (() => onStageEditorButtonClicked ());
        neuralNetowrkButton.onClick.AddListener (() => onNeuralNetworkButtonClicked ());

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

        geneticsManager.ResetCars ();
        geneticsManager.gameObject.SetActive (true);
    }
}
