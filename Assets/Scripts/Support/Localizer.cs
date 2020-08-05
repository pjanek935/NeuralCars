using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localizer : MonoBehaviour
{
    [SerializeField] string key;

    Text text;

    public delegate void OnTextChangedEventHandler();
    public event OnTextChangedEventHandler OnTextChanged;

    private void OnEnable()
    {
        text = GetComponent<Text> ();
        localize (Preferences.Language);
    }

    void localize (Language language)
    {
        if (text != null && !string.IsNullOrEmpty (key))
        {
            string localiaztion = LanguageManager.Instance.GetLocalization (key, language);
            text.text = localiaztion;

            OnTextChanged?.Invoke ();
        }
    }

    public static string GetLocalized (string key)
    {
        string result = key;

        if (! string.IsNullOrEmpty (key))
        {
            result = LanguageManager.Instance.GetLocalization (key, Preferences.Language);
        }

        return result;
    }
}
