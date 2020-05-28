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

    public delegate void OnSnapToGridToggleClickedEventHandler (bool value);
    public event OnSnapToGridToggleClickedEventHandler OnSnapToGridToggleClicked;

    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] SaveList loadList;
    [SerializeField] Text stageName;
    [SerializeField] Button resetStageButton;
    [SerializeField] Button clearStageButton;
    [SerializeField] GameObject saveStar;
    [SerializeField] Toggle snapToGridToggle;

    private void Awake ()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener (() => onSaveButtonClicked ());
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener (() => onLoadButtonClicked ());
        }

        if (loadList != null)
        {
            loadList.OnElementOnListClicked += onElementOnListClicekd;
        }

        if (resetStageButton != null)
        {
            resetStageButton.onClick.AddListener (() => OnResetClicked?.Invoke ());
        }

        if (clearStageButton != null)
        {
            clearStageButton.onClick.AddListener (() => OnClearClicked?.Invoke ());
        }

        if (snapToGridToggle != null)
        {
            snapToGridToggle.onValueChanged.AddListener ((val) => OnSnapToGridToggleClicked?.Invoke (val));
        }
    }

    public void Refresh ()
    {
        if (stageName != null)
        {
            stageName.text = "STAGE 0" + SaveManager.Instance.CurrentOpenedStageId;
        }

        loadList.Refresh ();
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
        if (loadList != null)
        {
            loadList.gameObject.SetActive (false);
        }

        OnSaveClicked?.Invoke ();
    }

    void onLoadButtonClicked ()
    {
        if (loadList != null)
        {
            loadList.gameObject.SetActive (! loadList.gameObject.activeSelf);
        }
    }
}
