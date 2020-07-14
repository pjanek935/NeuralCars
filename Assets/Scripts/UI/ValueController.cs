using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ValueController : MonoBehaviour
{
    public delegate void ValueControllerValueChangedEventHandler (float newVal);
    public event ValueControllerValueChangedEventHandler OnValueChanged;

    public UnityAction OnUpClicked;
    public UnityAction OnDownClicked;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button upButton;
    [SerializeField] Button downButton;
    [SerializeField] Text valueText;

    public float CurrentValue
    {
        get;
        private set;
    }

    public float MaxValue
    {
        get;
        set;
    }

    public float MinValue
    {
        get;
        set;
    }

    public float D
    {
        get;
        set;
    }

    public string Format
    {
        get;
        set;
    }

    private void Awake ()
    {
        upButton.onClick.AddListener (() => onUpClicked ());
        downButton.onClick.AddListener (() => onDownClicked ());
    }

    public void Setup (float max, float min, float d)
    {
        MaxValue = max;
        MinValue = min;
        D = d;
    }

    void onUpClicked ()
    {
        CurrentValue += D;
        CurrentValue = Mathf.Clamp (CurrentValue, MinValue, MaxValue);
        upButton.interactable = CurrentValue < MaxValue;
        downButton.interactable = CurrentValue > MinValue;
        setVal (CurrentValue);
        OnUpClicked?.Invoke ();
        OnValueChanged?.Invoke (CurrentValue);
    }

    void onDownClicked ()
    {
        CurrentValue -= D;
        CurrentValue = Mathf.Clamp (CurrentValue, MinValue, MaxValue);
        downButton.interactable = CurrentValue > MinValue;
        upButton.interactable = CurrentValue < MaxValue;
        setVal (CurrentValue);
        OnDownClicked?.Invoke ();
        OnValueChanged?.Invoke (CurrentValue);
    }

    public void SetValue (float value)
    {
        CurrentValue = value;
        setVal (value);
    }

    public void Enable ()
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void Disable ()
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f;
        }
    }

    void setVal (float val)
    {
        if (string.IsNullOrEmpty (Format))
        {
            valueText.text = val.ToString ();
        }
        else
        {
            valueText.text = val.ToString (Format);
        }
    }
}
