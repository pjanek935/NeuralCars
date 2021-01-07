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

    protected void Awake ()
    {
        backgroundButton.onClick.AddListener (() => onBackgroundButtonClicked  ());
    }

    public virtual void Show ()
    {
        cancelAnimation ();
        this.gameObject.SetActive (true);
        IsVisible = true;
        canvas.blocksRaycasts = true;
        canvas.interactable = true;
        canvas.DOFade (1f, GlobalConst.SHOW_AND_HIDE_TIME * Time.timeScale);
        mainFrame.DOScale (1f, GlobalConst.SHOW_AND_HIDE_TIME * Time.timeScale);
    }

    public virtual void Hide ()
    {
        cancelAnimation ();
        IsVisible = false;
        canvas.blocksRaycasts = false;
        canvas.DOFade (0f, GlobalConst.SHOW_AND_HIDE_TIME * Time.timeScale);
        mainFrame.DOScale (0.3f, GlobalConst.SHOW_AND_HIDE_TIME * Time.timeScale).OnComplete (() => this.gameObject.SetActive (false));
    }

    void cancelAnimation ()
    {
        DOTween.Kill (canvas);
        DOTween.Kill (mainFrame);
    }

    protected virtual void onBackgroundButtonClicked ()
    {
        Hide ();
    }
}
