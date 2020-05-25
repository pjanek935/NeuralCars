using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelController : MonoBehaviour
{
    public delegate void OnSaveOrLoadRequestedEventHandler (int slotId);
    public event OnSaveOrLoadRequestedEventHandler OnSaveClicked;
    public event OnSaveOrLoadRequestedEventHandler OnLoadClicked;

    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;

    [SerializeField] SaveList saveList;
    [SerializeField] SaveList loadList;

    private void Awake ()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener (() => onSaveButtonClicked ());
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener (() => onLoadButtonClicked ());
        }

        if (saveList != null)
        {
            saveList.OnElementOnListClicked += onElementOnListClicekd;
        }

        if (saveList != null)
        {
            loadList.OnElementOnListClicked += onElementOnListClicekd;
        }
    }

    void onElementOnListClicekd (SaveList list, int slotId)
    {
        if (list == saveList)
        {
            OnSaveClicked?.Invoke (slotId);
        }
        else if (list == loadList)
        {
            OnLoadClicked?.Invoke (slotId);
        }

        list.Refresh ();
    }

    void onSaveButtonClicked ()
    {
        if (saveList != null)
        {
            saveList.gameObject.SetActive (!saveList.gameObject.activeSelf);
        }

        if (loadList != null)
        {
            loadList.gameObject.SetActive (false);
        }
    }

    void onLoadButtonClicked ()
    {
        if (saveList != null)
        {
            saveList.gameObject.SetActive (false);
        }

        if (loadList != null)
        {
            loadList.gameObject.SetActive (! loadList.gameObject.activeSelf);
        }
    }
}
