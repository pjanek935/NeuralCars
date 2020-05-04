using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] Button deleteButton;

    Transform transformToFollow;

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
        if (transformToFollow != null && camera != null)
        {
            Vector3 newPos = camera.WorldToScreenPoint (transformToFollow.position);
            this.transform.position = newPos;
        }
    }

    public void Setup (Flag flag)
    {
        if (flag != null)
        {
            transformToFollow = flag.transform;
            setWidth (flag.Width);

            if (flag.Width >= 5f)
            {
                widthUpButton.interactable = false;
            }
            else
            {
                widthUpButton.interactable = true;
            }

            if (flag.Width <= 0.5f)
            {
                widthDownButton.interactable = false;
            }
            else
            {
                widthDownButton.interactable = true;
            }
        }
        else
        {
            transformToFollow = null;
            //hide
        }
    }

    void setWidth (float width)
    {
        if (wdithText != null)
        {
            wdithText.text = width.ToString ();
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
