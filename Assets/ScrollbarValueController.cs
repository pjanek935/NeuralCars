using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarValueController : MonoBehaviour
{
    public delegate void ValueControllerValueChangedEventHandler (float newVal);
    public event ValueControllerValueChangedEventHandler OnValueChanged;

    [SerializeField] Scrollbar scrollbar;
    [SerializeField] Text valueLabel;

    [SerializeField] float maxVal;
    [SerializeField] float minVal;

    public float CurrentValue
    {
        get;
        private set;
    }

    private void OnValidate ()
    {
        if (minVal > maxVal)
        {
            minVal = maxVal;
        }
    }

    private void Awake ()
    {
        scrollbar.onValueChanged.AddListener ((val) => onScrollbarValueChanged (val));
    }

    public void SetValue (float val)
    {
        float normalizedValue = (val - minVal) / (maxVal - minVal);
        scrollbar.value = normalizedValue;
    }

    void onScrollbarValueChanged (float val)
    {
        CurrentValue = val * (maxVal - minVal) + minVal;
        valueLabel.text = CurrentValue.ToString ();
        OnValueChanged?.Invoke (CurrentValue);
    }
}
