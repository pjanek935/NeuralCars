using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWithMessage : Popup
{
    public delegate void OnPopupWithMessageClosed (bool withConfirm);

    [SerializeField] protected Button confirmButton;
    [SerializeField] protected Button cancelButton;
    [SerializeField] protected Text message;

    OnPopupWithMessageClosed onPopupClosed = null;

    public static PopupWithMessage Instance
    {
        get;
        private set;
    }

    protected new void Awake ()
    {
        base.Awake ();

        confirmButton.onClick.AddListener (() => onConfirmClicked ());
        cancelButton.onClick.AddListener (() => onCancelClicked ());

        if (Instance != null)
        {
            Destroy (this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Show (string message, bool confirmButtonVisible, bool cancelButtonVisible, OnPopupWithMessageClosed onPopupClosed)
    {
        this.message.text = message;
        confirmButton.gameObject.SetActive (confirmButtonVisible);
        cancelButton.gameObject.SetActive (cancelButtonVisible);
        this.onPopupClosed = onPopupClosed;

        base.Show ();
    }

    protected virtual void onConfirmClicked ()
    {
        Hide ();
        onPopupClosed?.Invoke (true);
    }

    protected virtual void onCancelClicked ()
    {
        Hide ();
        onPopupClosed?.Invoke (false);
    }

    protected override void onBackgroundButtonClicked ()
    {
        onCancelClicked ();
    }
}
