using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Toggle))]
public class NeuronToggle : MonoBehaviour
{
    public delegate void NeuronToggleValueChangedEventHandler (NeuronToggle sender, bool value);
    public event NeuronToggleValueChangedEventHandler OnValueChanged;

    [SerializeField] List<Graphic> graphics = new List<Graphic> ();
    Toggle toggle;

    public bool IsOn
    {
        get
        {
            getToggleReferenceIfNeeded ();
            return toggle.isOn;
        }
    }

    private void Awake ()
    {
        getToggleReferenceIfNeeded ();
        toggle.onValueChanged.AddListener ((val) => onToggleValueChanged (val));
        onToggleValueChanged (toggle.isOn);
    }

    void getToggleReferenceIfNeeded ()
    {
        if (toggle == null)
        {
            toggle = GetComponent<Toggle> ();
        }
    }

    void onToggleValueChanged (bool val)
    {
        foreach (Graphic g in graphics)
        {
            Color c = g.color;
            c.a = val ? 1f : 0.5f;
            g.color = c;
        }

        OnValueChanged?.Invoke (this, toggle.isOn);
    }
}
