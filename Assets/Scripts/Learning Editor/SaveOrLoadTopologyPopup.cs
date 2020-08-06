using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class SaveOrLoadTopologyPopup : Popup
{
    public delegate void OnNewTopologyLoadedEventHandler (SavedTopologyData savedTopologyData);
    public event OnNewTopologyLoadedEventHandler OnNewTopologyLoaded;

    [SerializeField] GameObject listElementPrefab;
    [SerializeField] RectTransform listElementContainer;
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Text saveButtonText;
    [SerializeField] ScrollRect scrollRect;

    List<SavedTopologyListElement> topologies = new List<SavedTopologyListElement> ();
    SavedTopologyListElement currentSelected = null;
    NetworkTopologySimpleData currentTopologyData = null;
    List<CarSimpleData> carsData = new List<CarSimpleData> ();

    protected new void Awake ()
    {
        base.Awake ();

        saveButton.onClick.AddListener (() => onSaveButtonClicked ());
        loadButton.onClick.AddListener (() => onLoadButtonClicked ());
    }

    void onSaveButtonClicked ()
    {
        int currentSelectedIndex = getCurrentSelectedIndex ();

        if (currentSelectedIndex != GlobalConst.INVALID_ID &&
            currentTopologyData != null &&
            carsData != null)
        {
            //TODO add a confirmation popup
            //TODO add a name input popup

            SavedTopologyData savedTopologyData = new SavedTopologyData (currentTopologyData,
                carsData, "topology " + currentSelectedIndex);
            SaveManager.Instance.SaveTopologyOnSlot (savedTopologyData, currentSelectedIndex);
            refresh ();
        }
    }

    void onLoadButtonClicked ()
    {
        if (currentSelected != null &&
            currentSelected.SavedTopologyData != null &&
            currentSelected.SavedTopologyData.TopologyData != null)
        {
            OnNewTopologyLoaded?.Invoke (currentSelected.SavedTopologyData);
            Hide ();
        }
    }

    int getCurrentSelectedIndex ()
    {
        int result = GlobalConst.INVALID_ID;
        
        if (currentSelected != null)
        {
            for (int i = 0; i < topologies.Count; i ++)
            {
                if (currentSelected == topologies [i])
                {
                    result = i;

                    break;
                }
            }
        }

        return result;
    }

    public void Setup (NetworkTopologySimpleData currentTopologyData, List <CarSimpleData> carsData)
    {
        this.currentTopologyData = currentTopologyData;
        this.carsData = carsData;
        refresh ();
    }

    void refresh ()
    {
        List<SavedTopologyData> topologiesData = SaveManager.Instance.GetSavedTopologies ();

        for (int i = 0; i < topologiesData.Count; i++)
        {
            if (i >= topologies.Count)
            {
                createNewTopologyListElement ();
            }

            topologies [i].Setup (topologiesData [i]);
        }

        int diff = topologies.Count - topologiesData.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                deleteLastListElement ();
            }
        }

        if (currentSelected != null)
        {
            currentSelected.Deselect ();
        }

        currentSelected = null;
        refreshButtons ();

        StartCoroutine (resetScrollRectVerticalNormalizedPosition ());
    }

    IEnumerator resetScrollRectVerticalNormalizedPosition ()
    {
        yield return new WaitForEndOfFrame ();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void refreshButtons ()
    {
        if (currentSelected != null && currentSelected.SavedTopologyData != null)
        {
            loadButton.interactable = true;
        }
        else
        {
            loadButton.interactable = false;
        }

        if (currentSelected != null)
        {
            saveButton.interactable = true;

            if (currentSelected.SavedTopologyData == null)
            {
                saveButtonText.text = "SAVE";
            }
            else
            {
                saveButtonText.text = "OVERRIDE";
            }
        }
        else
        {
            saveButtonText.text = "SAVE";
            saveButton.interactable = false;
        }
    }

    void deleteLastListElement ()
    {
        if (topologies.Count > 0)
        {
            GameObject tmp = topologies [topologies.Count - 1].gameObject;
            topologies.RemoveAt (topologies.Count - 1);
            Destroy (tmp);
        }
    }

    void createNewTopologyListElement ()
    {
        GameObject newGameObject = Instantiate (listElementPrefab);
        newGameObject.SetActive (true);
        newGameObject.transform.SetParent (listElementContainer);
        SavedTopologyListElement savedTopologyListElement = newGameObject.GetComponent<SavedTopologyListElement> ();
        savedTopologyListElement.OnClick += onListElementClicked;
        topologies.Add (savedTopologyListElement);
    }

    void onListElementClicked (SavedTopologyListElement savedTopologyListElement)
    {
        for (int i = 0; i < topologies.Count; i ++)
        {
            if (savedTopologyListElement == topologies [i])
            {
                topologies [i].Select ();
            }
            else
            {
                topologies [i].Deselect ();
            }
        }

        currentSelected = savedTopologyListElement;
        refreshButtons ();
    }
}
