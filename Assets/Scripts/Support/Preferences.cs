using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;

public static class Preferences
{
    public delegate void BlurEffectValueChangedEventHandler(bool blurValue);
    public static event BlurEffectValueChangedEventHandler OnBlurEffectValueChanged;

    const string SOUND_VOLUME_KEY = "SoundVolumeKey";
    const string SIXTY_FPS_ON_KEY = "SixtyFPSOnKey";
    const string CURRENT_LANGUAGE = "CurrentLanguage";
    const string SAVED_GAME = "SavedGame";
    const string BLUR_EFFECT = "BlurEffect";

    public enum LevelState
    {
        LOCKED, UNLOCKED
    }

    public static SaveGame SavedGame
    {
        get
        {
            SaveGame result;
            string json = PlayerPrefs.GetString (SAVED_GAME, string.Empty);

            if (string.IsNullOrEmpty (json))
            {
                result = new SaveGame ();
            }
            else
            {
                result = JsonUtility.FromJson<SaveGame> (json);
            }

            return result;
        }

        set
        {
            string json = JsonUtility.ToJson (value);
            PlayerPrefs.SetString (SAVED_GAME, json);
            PlayerPrefs.Save ();
        }
    }

    public static float SoundVolume
    {
        get
        {
            return PlayerPrefs.GetFloat (SOUND_VOLUME_KEY, 0.5f);
        }

        set
        {
            PlayerPrefs.SetFloat (SOUND_VOLUME_KEY, value);
            PlayerPrefs.Save ();
            updateSoundVolume ();
        }
    }

    public static bool SixtyFPS
    {
        get
        {
            return PlayerPrefs.GetInt (SIXTY_FPS_ON_KEY, 1) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt (SIXTY_FPS_ON_KEY, value ? 1 : 0);
            PlayerPrefs.Save ();
            updateFramerate (value);
        }
    }

    public static bool BlurEffect
    {
        get
        {
            return PlayerPrefs.GetInt (BLUR_EFFECT, 0) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt (BLUR_EFFECT, value ? 1 : 0);
            PlayerPrefs.Save ();
            updateBlurEffect (value);
        }
    }
    
    public static Language Language
    {
        get
        {
            Language result = Language.NOT_DEFINED;
            result = ((Language)PlayerPrefs.GetInt (CURRENT_LANGUAGE, (int)Language.NOT_DEFINED));

            if (result == Language.NOT_DEFINED)
            {
                result = Language.ENGLISH;

                if (Application.systemLanguage == SystemLanguage.Polish)
                {
                    result = Language.POLISH;
                }

                Language = result;
            }

            return result;
        }

        set
        {
            PlayerPrefs.SetInt (CURRENT_LANGUAGE, (int)value);
            PlayerPrefs.Save ();
            updateLanguage (value);
        }
    }

    public static void SaveValues (float soundVolume, bool sixtyFPS, bool blurEffect, Language language)
    {
        PlayerPrefs.SetFloat (SOUND_VOLUME_KEY, soundVolume);
        PlayerPrefs.SetInt (SIXTY_FPS_ON_KEY, sixtyFPS ? 1 : 0);
        PlayerPrefs.SetInt (CURRENT_LANGUAGE, (int)language);
        PlayerPrefs.SetInt (BLUR_EFFECT, blurEffect ? 1 : 0);
        PlayerPrefs.Save ();

        updateSoundVolume ();
        updateFramerate (sixtyFPS);
        updateLanguage (language);
        updateBlurEffect (blurEffect);
    }

    static void updateSoundVolume()
    {
        //TODO
    }

    static void updateFramerate (bool sixtyFPS)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = sixtyFPS ? 60 : 30;
    }

    static void updateBlurEffect (bool blurEffect)
    {
        OnBlurEffectValueChanged?.Invoke (blurEffect);
    }

    static void updateLanguage (Language newLanguage)
    {
        LanguageManager.Instance.RequestLanguageChanged (newLanguage);
    }

    public static void ClearSavedGame ()
    {
        SaveGame saveGame = new SaveGame ();
        Preferences.SavedGame = saveGame;
    }
}
