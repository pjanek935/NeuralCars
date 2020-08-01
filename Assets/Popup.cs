using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;
    [SerializeField] RectTransform mainFrame;
    [SerializeField] Button backgroundButton;

    public bool IsVisible
    {
        get;
        private set;
    }

    private void Awake ()
    {
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        backgroundButton.onClick.AddListener (() => onBackgroundButtonClicked  ());
    }

    public void Show ()
    {
        if (! IsVisible)
        {
            this.gameObject.SetActive (true);
            IsVisible = true;
            canvas.blocksRaycasts = true;
            canvas.DOFade (1f, GlobalConst.SHOW_AND_HIDE_TIME);
            mainFrame.DOScale (1f, GlobalConst.SHOW_AND_HIDE_TIME);
        }
    }

    public void Hide ()
    {
        if (IsVisible)
        {
            IsVisible = false;
            canvas.blocksRaycasts = false;
            canvas.DOFade (0f, GlobalConst.SHOW_AND_HIDE_TIME);
            mainFrame.DOScale (0.3f, GlobalConst.SHOW_AND_HIDE_TIME).OnComplete (() => this.gameObject.SetActive (false));
        }
    }

    void onBackgroundButtonClicked ()
    {
        Hide ();
    }
}
