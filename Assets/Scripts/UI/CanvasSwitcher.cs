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
    [SerializeField] Button saveOrLoadTopologyButton;
    [SerializeField] Button backFromLearningWithAdvisor;
    [SerializeField] Button learningWithAdvisorButton;
    [SerializeField] Button optionsButton;

    [SerializeField] GameObject stageEditorCanvas;
    [SerializeField] GameObject neuralNetworkCanvas;
    [SerializeField] GameObject networkTopologyCanvas;
    [SerializeField] GameObject learningWithAdvisorCanvas;
    [SerializeField] SaveOrLoadTopologyPopup saveOrLoadTopologyPopup;
    [SerializeField] OptionsWindow optionsWindow;

    [SerializeField] StageEditor stageEditor;
    [SerializeField] GeneticsManager geneticsManager;
    [SerializeField] NeuralNetworkTopologyController networkTopologyController;
    [SerializeField] GeneticsUIController geneticsUIController;
    [SerializeField] LearningWithAdvisor learningWithAdvisor;
    [SerializeField] ImageFader imageFader;
    [SerializeField] GameObject forceField;
    [SerializeField] Stage stage;

    private void Awake ()
    {
        stageEditorButton.onClick.AddListener (() => onStageEditorButtonClicked ());
        neuralNetowrkButton.onClick.AddListener (() => onNeuralNetworkButtonClicked ());
        networkTopologyButton.onClick.AddListener (() => onNetworkTopologyButtonClicked ());
        backToNeuralButton.onClick.AddListener (() => backToNeural ());
        saveOrLoadTopologyButton.onClick.AddListener (() => onSaveOrLoadTopologyButtonClicked ());
        backFromLearningWithAdvisor.onClick.AddListener (() => onBackFromLearningWithAdvisorClicked ());
        learningWithAdvisorButton.onClick.AddListener (() => onLearningWithAdvisorButtonClicked ());
        optionsButton.onClick.AddListener (() => onOptionsButtonClicked ());

        saveOrLoadTopologyPopup.OnNewTopologyLoaded += onNewTopologyLoaded;

        onNeuralNetworkButtonClicked ();
        geneticsManager.Init ();

        Application.runInBackground = true;
    }

    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape))
        {
            showOrHideOptionsWindow ();
        }
    }

    void showOrHideOptionsWindow ()
    {
        if (optionsWindow.IsVisible)
        {
            optionsWindow.Hide ();
        }
        else
        {
            optionsWindow.Show ();
        }
    }

    void onOptionsButtonClicked ()
    {
        showOrHideOptionsWindow ();
    }

    void onBackFromLearningWithAdvisorClicked ()
    {
        imageFader.FadeIn (switchToGeneticsLearningFromLearningWithAdvisor);
    }

    void onLearningWithAdvisorButtonClicked ()
    {
        imageFader.FadeIn (switchToLearningWithAdvisor);
    }

    void switchToGeneticsLearningFromLearningWithAdvisor ()
    {
        learningWithAdvisor.Exit ();
        learningWithAdvisorCanvas.SetActive (false);
        neuralNetworkCanvas.SetActive (true);
        forceField.SetActive (false);
        geneticsManager.gameObject.SetActive (true);
        geneticsManager.ResetCars ();
        geneticsManager.ActivateCars ();
        imageFader.FadeOut ();
    }

    void switchToLearningWithAdvisor ()
    {
        learningWithAdvisor.Enter ();
        learningWithAdvisorCanvas.SetActive (true);
        neuralNetworkCanvas.SetActive (false);
        forceField.SetActive (true);
        geneticsManager.gameObject.SetActive (false);
        imageFader.FadeOut ();
    }

    void onStageEditorButtonClicked ()
    {
        TimeScaleController.SetDefaultTimeScale ();
        imageFader.FadeIn (switchFromLearningWindowToStageEditor);
    }

    void switchFromLearningWindowToStageEditor ()
    {
        stageEditor.EnableStageEditor ();
        stageEditorCanvas.SetActive (true);
        neuralNetworkCanvas.SetActive (false);
        forceField.SetActive (true);
        geneticsManager.gameObject.SetActive (false);

        imageFader.FadeOut ();
    }

    void onNeuralNetworkButtonClicked ()
    {
        TimeScaleController.SetSavedTimeScale ();
        imageFader.FadeIn (switchStageEditorToLearningWindow);
    }

    void onNewTopologyLoaded (SavedTopologyData savedTopologyData)
    {
        if (savedTopologyData != null && savedTopologyData.TopologyData != null && savedTopologyData.CarSimpleData != null)
        {
            NetworkTopologySimpleData newTopology = savedTopologyData.TopologyData;
            geneticsManager.SetNetworkTopology (newTopology);
            geneticsManager.ResetSimulation ();
            geneticsManager.SetNewCars (savedTopologyData.CarSimpleData);
            geneticsManager.ResetCars ();
            geneticsManager.ActivateCars ();

            geneticsUIController.RefreshViews ();
        }
    }

    void switchStageEditorToLearningWindow ()
    {
        stageEditor.DisableStageEditor ();
        stageEditorCanvas.SetActive (false);
        neuralNetworkCanvas.SetActive (true);
        stage.PrepareStage ();
        forceField.SetActive (false);
        geneticsManager.gameObject.SetActive (true);
        geneticsManager.ResetCars ();
        geneticsManager.ActivateCars ();
        imageFader.FadeOut ();
    }

    void onNetworkTopologyButtonClicked ()
    {
        TimeScaleController.SetDefaultTimeScale ();
        imageFader.FadeIn (switchFromLearningWindowToNetworkTopology);
    }

    void switchFromLearningWindowToNetworkTopology ()
    {
        neuralNetworkCanvas.SetActive (false);
        networkTopologyCanvas.SetActive (true);
        networkTopologyController.Init (geneticsManager.CurrentTopology);
        geneticsManager.gameObject.SetActive (false);

        imageFader.FadeOut ();
    }

    void backToNeural ()
    {
        TimeScaleController.SetSavedTimeScale ();
        imageFader.FadeIn (switchFromNetworkTopologyToLearningWindow);
    }

    void switchFromNetworkTopologyToLearningWindow ()
    {
        networkTopologyCanvas.SetActive (false);
        neuralNetworkCanvas.SetActive (true);

        NetworkTopologySimpleData currentTopology = geneticsManager.CurrentTopology;
        NetworkTopologySimpleData newTopology = networkTopologyController.GetNetworkTopologySimpleData ();
        bool isTpologyDifferent = currentTopology.IsDifferent (newTopology);
        geneticsManager.gameObject.SetActive (true);

        if (isTpologyDifferent)
        {
            geneticsManager.SetNetworkTopology (newTopology);
            geneticsManager.ResetSimulation ();
        }

        geneticsManager.ResetCars ();
        geneticsManager.ActivateCars ();

        geneticsUIController.RefreshViews ();
        imageFader.FadeOut ();
    }

    void onSaveOrLoadTopologyButtonClicked ()
    {
        saveOrLoadTopologyPopup.Show ();
        saveOrLoadTopologyPopup.Setup (geneticsManager.CurrentTopology, geneticsManager.GetCarSimpleData ());
    }
}
