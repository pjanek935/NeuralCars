using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ListElement : MonoBehaviour
{
    public UnityEvent OnButtonClicked;

    [SerializeField] Text text;
    [SerializeField] Button button;
    [SerializeField] Image background;

    private void Awake ()
    {
        if (button != null)
        {
            button.onClick.AddListener (() => OnButtonClicked?.Invoke ());
        }
    }

    public void SetName (string name)
    {
        if (text != null)
        {
            text.text = name;
        }
    }

    public void SetBackgroundColor (Color color)
    {
        if (background != null)
        {
            background.color = color;
        }
    }
}
