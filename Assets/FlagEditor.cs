using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FlagEditor : MonoBehaviour
{
    public delegate void OnFlagEditorButtonClickedEventHandler ();
    public event OnFlagEditorButtonClickedEventHandler OnWidthDownClicked;
    public event OnFlagEditorButtonClickedEventHandler OnWidthUpClicked;
    public event OnFlagEditorButtonClickedEventHandler OnDeleteClicked;

    const float flagHeight = 4f; //flag object size in world space, used to calculate fag editor height based on camera position
    const float showAndHideTime = 0.3f;

    [SerializeField] new Camera camera;
    [SerializeField] Button widthDownButton;
    [SerializeField] Button widthUpButton;
    [SerializeField] Text wdithText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button deleteButton;

    Transform transformToFollow;

    public bool IsVisible
    {
        get;
        private set;
    }

    public bool IsShowing
    {
        get;
        private set;
    }

    public bool IsHiding
    {
        get;
        private set;
    }

    private void Awake ()
    {
        if (widthDownButton != null)
        {
            widthDownButton.onClick.AddListener (() => onWidthDownButtonClicked ());
        }

        if (widthUpButton != null)
        {
            widthUpButton.onClick.AddListener (() => onWidthUpButtonClicked ());
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener (() => onDeleteButtonClicked ());
        }
    }

    private void Update ()
    {
        if (transformToFollow != null && camera != null && ! IsHiding)
        {
            Vector3 center = camera.WorldToScreenPoint (transformToFollow.position);
            Vector3 upper = camera.WorldToScreenPoint (transformToFollow.position + new Vector3 (0, 0, flagHeight / 2f));
            Vector3 lower = camera.WorldToScreenPoint (transformToFollow.position - new Vector3 (0, 0, flagHeight / 2f));
            float height = Mathf.Abs (upper.y - lower.y);
            RectTransform rectTransform = (RectTransform) transform;
            rectTransform.sizeDelta = new Vector2 (rectTransform.sizeDelta.x, height);
            this.transform.position = center;
        }
    }

    public void Setup (Flag flag)
    {
        if (flag != null)
        {
            if (transformToFollow != flag.transform)
            {//new and prev transform differs = hide in current position and show in antother
                transformToFollow = flag.transform;
                hide (() =>
                {
                    setWidth (flag.Width);
                    show ();
                });
            }
            else if (! IsHiding)
            {
                setWidth (flag.Width);
            }
        }
        else
        {
            transformToFollow = null;
            hide ();
        }
    }

    void hide (UnityAction onComplete = null)
    {
        IsHiding = true;
        IsVisible = false;
        LeanTween.alphaCanvas (canvasGroup, 0f, showAndHideTime).setOnComplete (() =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            IsHiding = false;
            onComplete?.Invoke ();
        });
    }

    void show (UnityAction onComplete = null)
    {
        IsShowing = true;
        IsVisible = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        LeanTween.alphaCanvas (canvasGroup, 1f, showAndHideTime).setOnComplete (() =>
        {
            IsShowing = false;
            onComplete?.Invoke ();
        });
    }

    void setWidth (float width)
    {
        if (wdithText != null)
        {
            wdithText.text = width.ToString ();
        }

        if (width >= 5f)
        {
            widthUpButton.interactable = false;
        }
        else
        {
            widthUpButton.interactable = true;
        }

        if (width <= 0.5f)
        {
            widthDownButton.interactable = false;
        }
        else
        {
            widthDownButton.interactable = true;
        }
    }

    void onWidthDownButtonClicked ()
    {
        OnWidthDownClicked?.Invoke ();
    }

    void onWidthUpButtonClicked ()
    {
        OnWidthUpClicked?.Invoke ();
    }

    void onDeleteButtonClicked ()
    {
        OnDeleteClicked?.Invoke ();
    }
}
