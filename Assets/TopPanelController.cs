using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelController : MonoBehaviour
{
    public delegate void OnLoadRequestedEventHandler (int slotId);
    public event OnLoadRequestedEventHandler OnLoadClicked;

    public delegate void TopPanelControllerEventHandler ();
    public event TopPanelControllerEventHandler OnSaveClicked;
    public event TopPanelControllerEventHandler OnResetClicked;
    public event TopPanelControllerEventHandler OnClearClicked;
    public event TopPanelControllerEventHandler OnDefaultWidthUpClicked;
    public event TopPanelControllerEventHandler OnDefaultWidthDownClicked;

    public delegate void OnSnapToGridToggleClickedEventHandler (bool value);
    public event OnSnapToGridToggleClickedEventHandler OnSnapToGridToggleClicked;

    [SerializeField] StageEditor stageEditor;
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] SaveList loadList;
    [SerializeField] Text stageName;
    [SerializeField] Button resetStageButton;
    [SerializeField] Button clearStageButton;
    [SerializeField] GameObject saveStar;
    [SerializeField] Toggle snapToGridToggle;
    [SerializeField] Text defaultWidthText;
    [SerializeField] Button defaultWidthUpButton;
    [SerializeField] Button defaultWdithDownButton;

    private void Awake ()
    {
        saveButton.onClick.AddListener (() => onSaveButtonClicked ());
        loadButton.onClick.AddListener (() => onLoadButtonClicked ());
        loadList.OnElementOnListClicked += onElementOnListClicekd;
        resetStageButton.onClick.AddListener (() => OnResetClicked?.Invoke ());
        clearStageButton.onClick.AddListener (() => OnClearClicked?.Invoke ());
        snapToGridToggle.onValueChanged.AddListener ((val) => OnSnapToGridToggleClicked?.Invoke (val));
        defaultWdithDownButton.onClick.AddListener (() => OnDefaultWidthDownClicked?.Invoke ());
        defaultWidthUpButton.onClick.AddListener (() => OnDefaultWidthUpClicked?.Invoke ());
    }

    public void Refresh ()
    {
        stageName.text = "STAGE 0" + SaveManager.Instance.CurrentOpenedStageId;
        loadList.Refresh ();
        defaultWidthText.text = stageEditor.DefaultWidth.ToString ();
    }

    public void Refresh (StageModel stageModel)
    {
        if (stageModel != null)
        {
            bool canUndoLastAction = stageModel.CanUndoLastAction ();
            resetStageButton.interactable = canUndoLastAction;
            clearStageButton.interactable = stageModel.Nodes.Count > 0;
            saveStar.gameObject.SetActive (canUndoLastAction);
        }

        Refresh ();
    }

    void onElementOnListClicekd (SaveList list, int slotId)
    {
        if (list == loadList)
        {
            OnLoadClicked?.Invoke (slotId);
        }

        Refresh ();
    }

    void onSaveButtonClicked ()
    {
        loadList.gameObject.SetActive (false);
        OnSaveClicked?.Invoke ();
    }

    void onLoadButtonClicked ()
    {
        loadList.gameObject.SetActive (!loadList.gameObject.activeSelf);
    }
}
