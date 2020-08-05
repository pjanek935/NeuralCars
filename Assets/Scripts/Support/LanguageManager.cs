using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    NOT_DEFINED = -1,
    ENGLISH = 0,
    POLISH
}

public class LanguageManager
{
    static LanguageManager instance;
    const string localizationFilePath = "GenusLocalization";
    string [,] localization;

    public delegate void OnLanguageChangedEventHandler (Language newLanguage);
    public OnLanguageChangedEventHandler OnLanguageChanged;

    public static bool IsInstanceActive
    {
        get { return instance != null; }
    }

    public static LanguageManager Instance
    {
        get
        {
            createInstanceIfNeeded ();

            return instance;
        }
    }

    static void createInstanceIfNeeded ()
    {
        if (instance == null)
        {
            instance = new LanguageManager ();
        }
    }

    private LanguageManager ()
    {
        TextAsset textAsset = Resources.Load<TextAsset> (localizationFilePath);

        if (textAsset != null)
        {
            string localizationString = textAsset.text;
            localization = CSVReader.SplitCsvGrid (localizationString);;
        }
    }

    public string GetLocalization (string key, Language language)
    {
        string result = key;

        if (localization != null)
        {
            int row = -1;
            int column = 2 + (int)language;

            if (column < localization.GetLength (0))
            {
                for (int i = 1; i < localization.GetLength (1); i++)
                {
                    if (string.Equals (localization [0, i], key))
                    {
                        row = i;

                        break;
                    }
                }

                if (row != -1)
                {
                    result = localization [column, row];
                }
            }
        }

        return result;
    }

    public void RequestLanguageChanged (Language newLanguage)
    {
        if (OnLanguageChanged != null)
        {
            OnLanguageChanged (newLanguage);
        }
    }
}
