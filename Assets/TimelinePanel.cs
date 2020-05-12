using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimelinePanel : MonoBehaviour
{
    public UnityAction OnBackClicked;
    public UnityAction OnForwardClicked;

    [SerializeField] Button backButton;
    [SerializeField] Button forwardButton;

    private void Start ()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener (() => { OnBackClicked?.Invoke (); });
        }

        if (forwardButton != null)
        {
            forwardButton.onClick.AddListener (() => { OnForwardClicked?.Invoke (); });
        }
    }

    public void Refresh (StageModel stageModel)
    {
        if (stageModel == null)
        {
            return;
        }

        if (backButton != null)
        {
            backButton.interactable = stageModel.CanUndoLastAction ();
        }

        if (forwardButton != null)
        {
            forwardButton.interactable = stageModel.CanMakeStepForward ();
        }
    }
}
