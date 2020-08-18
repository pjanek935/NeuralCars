using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelController : MonoBehaviour
{
    public event SaveOrLoadStagePopup.OnLoadStageClickedEventHandler OnLoadClicked;

    public delegate void TopPanelControllerEventHandler ();
    public event TopPanelControllerEventHandler OnSaveClicked;
    public event TopPanelControllerEventHandler OnResetClicked;
    public event TopPanelControllerEventHandler OnClearClicked;
    public event TopPanelControllerEventHandler OnDefaultWidthUpClicked;
    public event TopPanelControllerEventHandler OnDefaultWidthDownClicked;

    public delegate void OnSnapToGridToggleClickedEventHandler (bool value);
    public event OnSnapToGridToggleClickedEventHandler OnSnapToGridToggleClicked;

    [SerializeField] Stage stage;
    [SerializeField] StageEditor stageEditor;
    [SerializeField] Button saveButton;
    [SerializeField] Text stageName;
    [SerializeField] Button resetStageButton;
    [SerializeField] Button clearStageButton;
    [SerializeField] Toggle snapToGridToggle;
    [SerializeField] Text defaultWidthText;
    [SerializeField] Button defaultWidthUpButton;
    [SerializeField] Button defaultWdithDownButton;
    [SerializeField] SaveOrLoadStagePopup saveOrLoadStagePopup;

    private void Awake ()
    {
        saveButton.onClick.AddListener (() => onSaveButtonClicked ());
        resetStageButton.onClick.AddListener (() => OnResetClicked?.Invoke ());
        clearStageButton.onClick.AddListener (() => OnClearClicked?.Invoke ());
        snapToGridToggle.onValueChanged.AddListener ((val) => OnSnapToGridToggleClicked?.Invoke (val));
        defaultWdithDownButton.onClick.AddListener (() => OnDefaultWidthDownClicked?.Invoke ());
        defaultWidthUpButton.onClick.AddListener (() => OnDefaultWidthUpClicked?.Invoke ());
        saveOrLoadStagePopup.OnLoadStageClicked += onLoadStageClicked;
    }

    public void Refresh ()
    {
        stageName.text = "STAGE 0" + SaveManager.Instance.CurrentOpenedStageId;
        defaultWidthText.text = stageEditor.DefaultWidth.ToString ();
    }

    public void Refresh (Stage stage)
    {
        if (stage != null)
        {
            bool canUndoLastAction = stage.CanUndoLastAction ();
            resetStageButton.interactable = canUndoLastAction;
            clearStageButton.interactable = stage.GetStageNodes ().Count > 0;
        }

        Refresh ();
    }


    void onSaveButtonClicked ()
    {
        saveOrLoadStagePopup.Setup (stage.StageModel);
        saveOrLoadStagePopup.Show ();
    }

    void onLoadStageClicked (StageModel stageModel)
    {
        OnLoadClicked?.Invoke (stageModel);
        saveOrLoadStagePopup.Hide ();
    }
}
