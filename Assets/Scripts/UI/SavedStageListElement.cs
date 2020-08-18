using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedStageListElement : MonoBehaviour
{
    public delegate void SavedStageListElementOnClickEventHandler (SavedStageListElement requester);
    public event SavedStageListElementOnClickEventHandler OnClick;

    [SerializeField] Text nameText;
    [SerializeField] Text lengthText;
    [SerializeField] Button button;
    [SerializeField] GameObject emptyContent;
    [SerializeField] GameObject savedStageContent;

    [SerializeField] Image background;
    [SerializeField] Image selectionBackground;

    Color defaultBackgroundColor = Color.white;
    Color selectedBackgroundColor = new Color (242f / 255f, 253 / 255f, 1f);

    public bool IsSelected
    {
        get;
        private set;
    }

    private void Awake ()
    {
        button.onClick.AddListener (() => onButtonClicked ());
    }

    void onButtonClicked ()
    {
        OnClick?.Invoke (this);
    }

    public StageModel StageModel
    {
        get;
        set;
    }

    public void Setup (StageModel stageModel)
    {
        if (stageModel != null)
        {
            emptyContent.SetActive (false);
            savedStageContent.SetActive (true);
            this.StageModel = stageModel;
            nameText.text = stageModel.StageName;
            lengthText.text = stageModel.GetTotalStageLength ().ToString ();
        }
        else
        {
            emptyContent.SetActive (true);
            savedStageContent.SetActive (false);
        }
    }

    public void Select ()
    {
        IsSelected = true;
        selectionBackground.gameObject.SetActive (true);
        background.color = selectedBackgroundColor;
    }

    public void Deselect ()
    {
        IsSelected = false;
        selectionBackground.gameObject.SetActive (false);
        background.color = defaultBackgroundColor;
    }
}
