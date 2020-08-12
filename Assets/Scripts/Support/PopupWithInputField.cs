using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWithInputField : Popup
{
    public delegate void OnPopupWithInputFieldClosed (bool withConfirm, string message);

    [SerializeField] Button confirmButton;
    [SerializeField] InputField inputField;
    [SerializeField] Text message;

    OnPopupWithInputFieldClosed onPopupClosed;

    public static PopupWithInputField Instance
    {
        get;
        private set;
    }

    protected new void Awake ()
    {
        base.Awake ();

        confirmButton.onClick.AddListener (() => onConfirmClicked ());

        if (Instance != null)
        {
            Destroy (this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Show (string message, OnPopupWithInputFieldClosed onPopupClosed)
    {
        this.message.text = message;
        this.onPopupClosed = onPopupClosed;
        inputField.text = string.Empty;

        base.Show ();
    }

    protected virtual void onConfirmClicked ()
    {
        Hide ();
        onPopupClosed?.Invoke (true, inputField.text);
    }

    protected override void onBackgroundButtonClicked ()
    {
        Hide ();
        onPopupClosed?.Invoke (false, inputField.text);
    }
}
