using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveOrLoadStagePopup : Popup
{
    public delegate void OnLoadStageClickedEventHandler (StageModel stageMode);
    public event OnLoadStageClickedEventHandler OnLoadStageClicked;

    [SerializeField] GameObject listElementPrefab;
    [SerializeField] RectTransform listElementContainer;
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Text saveButtonText;
    [SerializeField] ScrollRect scrollRect;

    List<SavedStageListElement> stages = new List<SavedStageListElement> ();
    SavedStageListElement currentSelected = null;
    StageModel currentStageModel;

    protected new void Awake ()
    {
        base.Awake ();

        saveButton.onClick.AddListener (() => onSaveButtonClicked ());
        loadButton.onClick.AddListener (() => onLoadButtonClicked ());
    }

    void onLoadButtonClicked ()
    {
        if (currentSelected != null && currentSelected.StageModel != null)
        {
            OnLoadStageClicked?.Invoke (currentSelected.StageModel);
        }
    }

    void onSaveButtonClicked ()
    {
        if (currentSelected != null && currentStageModel != null)
        {
            if (currentSelected.StageModel == null)
            {
                PopupWithInputField.Instance.Show ("TYPE NAME FOR SAVED STAGE", onNameChoosen);
            }
            else
            {
                PopupWithMessage.Instance.Show ("ARE YOU SURE WANT TO OVERRIDE <i>"
                    + currentSelected.StageModel.StageName + "</i>?", true, true, onOverrideConfirmed);
            }
        }
    }

    void onNameChoosen (bool confirmed, string message)
    {
        if (confirmed)
        {
            currentStageModel.StageName = message;
            SaveManager.Instance.SaveStage (currentStageModel, getCurrentSelectedId ());
            Hide ();
        }
    }

    void onOverrideConfirmed (bool confirmed)
    {
        if (confirmed)
        {
            PopupWithInputField.Instance.Show ("TYPE NAME FOR SAVED STAGE", onNameChoosen);
        }
    }

    int getCurrentSelectedId ()
    {
        int result = GlobalConst.INVALID_ID;

        for (int i = 0; i < stages.Count; i ++)
        {
            if (currentSelected == stages [i])
            {
                result = i;

                break;
            }
        }

        return result;
    }

    public override void Show ()
    {
        base.Show ();
        StartCoroutine (resetScrollRectVerticalNormalizedPosition ());
    }

    public void Setup (StageModel currentStageModel)
    {
        List<StageModel> stageData = SaveManager.Instance.GetSavedStages ();
        this.currentStageModel = currentStageModel;

        for (int i = 0; i < stageData.Count; i++)
        {
            if (i >= stages.Count)
            {
                createNewSavedStageElement ();
            }

            stages [i].Setup (stageData [i]);
        }

        int diff = stages.Count - stageData.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                deleteLastListElement ();
            }
        }

        if (currentSelected != null)
        {
            currentSelected.Deselect ();
        }

        currentSelected = null;
        refreshButtons ();
    }

    void createNewSavedStageElement ()
    {
        GameObject newGameObject = Instantiate (listElementPrefab);
        newGameObject.SetActive (true);
        newGameObject.transform.SetParent (listElementContainer, false);
        SavedStageListElement savedStageListElement = newGameObject.GetComponent<SavedStageListElement> ();
        savedStageListElement.OnClick += onListElementClicked;
        stages.Add (newGameObject.GetComponent <SavedStageListElement> ());
    }

    void deleteLastListElement ()
    {
        if (stages.Count > 0)
        {
            GameObject tmp = stages [stages.Count - 1].gameObject;
            stages.RemoveAt (stages.Count - 1);
            Destroy (tmp);
        }
    }

    void refreshButtons ()
    {
        if (currentSelected != null && currentSelected.StageModel != null)
        {
            loadButton.interactable = true;
        }
        else
        {
            loadButton.interactable = false;
        }

        if (currentSelected != null)
        {
            saveButton.interactable = true;

            if (currentSelected.StageModel == null)
            {
                saveButtonText.text = "SAVE";
            }
            else
            {
                saveButtonText.text = "OVERRIDE";
            }
        }
        else
        {
            saveButtonText.text = "SAVE";
            saveButton.interactable = false;
        }
    }

    void onListElementClicked (SavedStageListElement savedStageListElement)
    {
        for (int i = 0; i < stages.Count; i++)
        {
            if (savedStageListElement == stages [i])
            {
                stages [i].Select ();
            }
            else
            {
                stages [i].Deselect ();
            }
        }

        currentSelected = savedStageListElement;
        refreshButtons ();
    }

    IEnumerator resetScrollRectVerticalNormalizedPosition ()
    {
        yield return new WaitForEndOfFrame ();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
