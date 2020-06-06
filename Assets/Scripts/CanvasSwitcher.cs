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

    private void Awake ()
    {
        stageEditorButton.onClick.AddListener (() => onStageEditorButtonClicked ());
        neuralNetowrkButton.onClick.AddListener (() => onNeuralNetworkButtonClicked ());
    }

    void onStageEditorButtonClicked ()
    {
        stageEditor.EnableStageEditor ();
        stageEditorCanvas.SetActive (true);
        neuralNetworkCanvas.SetActive (false);
    }

    void onNeuralNetworkButtonClicked ()
    {
        stageEditor.DisableStageEditor ();
        stageEditorCanvas.SetActive (false);
        neuralNetworkCanvas.SetActive (true);
    }
}
