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

    [SerializeField] new Camera camera;
    [SerializeField] Button widthDownButton;
    [SerializeField] Button widthUpButton;
    [SerializeField] Text wdithText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button deleteButton;

    [SerializeField] GameObject deletButonContainer;
    [SerializeField] GameObject widthControllerContainer;

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
            float halfFlagHeight = StageConsts.FlagHeight / 2f;
            Vector3 center = camera.WorldToScreenPoint (transformToFollow.position);
            Vector3 upper = camera.WorldToScreenPoint (transformToFollow.position + new Vector3 (0, 0, halfFlagHeight));
            Vector3 lower = camera.WorldToScreenPoint (transformToFollow.position - new Vector3 (0, 0, halfFlagHeight));
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

    void setAnchoredYPos (GameObject gameObject, float value)
    {
        if (gameObject != null)
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2 (rectTransform.anchoredPosition.x, value);
            }
        }
    }

    void hide (UnityAction onComplete = null)
    {
        IsHiding = true;
        IsVisible = false;

        LeanTween.value (deletButonContainer, 0, StageConsts.HiddenComponentYPos, StageConsts.ShowAndHideTime).setOnUpdate ((float val) =>
        {
            setAnchoredYPos (deletButonContainer, val);
        }).setEase (LeanTweenType.easeInBack);

        LeanTween.value (widthControllerContainer, 0, -StageConsts.HiddenComponentYPos, StageConsts.ShowAndHideTime).setOnUpdate ((float val) =>
        {
            setAnchoredYPos (widthControllerContainer, val);
        }).setEase (LeanTweenType.easeInBack);

        LeanTween.alphaCanvas (canvasGroup, 0f, StageConsts.ShowAndHideTime).setOnComplete (() =>
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
        float currentDeleteButtonContainerAnchoredYPos = StageConsts.HiddenComponentYPos;
        float currentWidthControllerAchoredYPos = -StageConsts.HiddenComponentYPos;

        if (deletButonContainer != null)
        {
            RectTransform rectTransform = deletButonContainer.GetComponent<RectTransform> ();
            currentDeleteButtonContainerAnchoredYPos = rectTransform.anchoredPosition.y;
        }

        if (widthControllerContainer != null)
        {
            RectTransform rectTransform = widthControllerContainer.GetComponent<RectTransform> ();
            currentWidthControllerAchoredYPos = rectTransform.anchoredPosition.y;
        }

        LeanTween.value (deletButonContainer, currentDeleteButtonContainerAnchoredYPos, 0, StageConsts.ShowAndHideTime).setOnUpdate ((float val) =>
        {
            setAnchoredYPos (deletButonContainer, val);
        }).setEase (LeanTweenType.easeOutBack);

        LeanTween.value (widthControllerContainer, currentWidthControllerAchoredYPos, 0, StageConsts.ShowAndHideTime).setOnUpdate ((float val) =>
        {
            setAnchoredYPos (widthControllerContainer, val);
        }).setEase (LeanTweenType.easeOutBack);

        LeanTween.alphaCanvas (canvasGroup, 1f, StageConsts.ShowAndHideTime).setOnComplete (() =>
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

        if (width >= StageConsts.MaxNodeWidth)
        {
            widthUpButton.interactable = false;
        }
        else
        {
            widthUpButton.interactable = true;
        }

        if (width <= StageConsts.MinNodeWidth)
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
