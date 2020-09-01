using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsWindow : Popup
{
    [SerializeField] Button exitButton;
    [SerializeField] Dropdown windowModeDropdown;
    [SerializeField] Dropdown resolutionDropdown;

    private new void Awake ()
    {
        base.Awake ();

        exitButton.onClick.AddListener (() => onExitButtonClicked ());
        windowModeDropdown.onValueChanged.AddListener ((value) => onWindowModeValueChanged (value));
        resolutionDropdown.onValueChanged.AddListener ((value) => onResolutionValueChanged (value));
    }

    void onWindowModeValueChanged (int val)
    {
        Screen.fullScreenMode = (FullScreenMode) val;
    }

    void onResolutionValueChanged (int val)
    {
        Resolution [] resolutions = Screen.resolutions;

        if (val >= 0 && val < resolutions.Length)
        {
            Screen.SetResolution (resolutions [val].width, resolutions [val].height, Screen.fullScreen);
        }
    }

    private void OnEnable ()
    {
        refreshDropdowns ();
    }

    void refreshDropdowns ()
    {
        List<Dropdown.OptionData> windowModeOptions = new List<Dropdown.OptionData> ();
        windowModeOptions.Add (new Dropdown.OptionData ("FULL SCREEN"));
        windowModeOptions.Add (new Dropdown.OptionData ("FULL SCREEN WINDOWED"));
        windowModeOptions.Add (new Dropdown.OptionData ("MAXIMIZED WINDOWED"));
        windowModeOptions.Add (new Dropdown.OptionData ("WINDOWED"));
        windowModeDropdown.options = windowModeOptions;
        windowModeDropdown.SetValueWithoutNotify ((int) Screen.fullScreenMode);

        List<Dropdown.OptionData> resolutionOptions = new List<Dropdown.OptionData> ();
        Resolution [] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i ++)
        {
            resolutionOptions.Add (new Dropdown.OptionData (resolutions [i].width + " x " + resolutions [i].height + " " + resolutions [i].refreshRate + "Hz"));
        }

        resolutionDropdown.options = resolutionOptions;
        resolutionDropdown.SetValueWithoutNotify (getCurrentSelectedResolution ());
    }

    int getCurrentSelectedResolution ()
    {
        int result = 0;
        Resolution [] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i ++)
        {
            if (resolutions [i].Equals (Screen.currentResolution))
            {
                result = i;

                break;
            }
        }

        return result;
    }

    void onExitButtonClicked ()
    {
        PopupWithMessage.Instance.Show ("ARE YOU SURE WANT TO QUIT?", true, true, onExitConfirmed);
    }

    void onExitConfirmed (bool confirmed)
    {
        if (confirmed)
        {
            Application.Quit ();
        }
    }
}
