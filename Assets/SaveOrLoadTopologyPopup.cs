using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Jobs;

public class SaveOrLoadTopologyPopup : Popup
{
    [SerializeField] GameObject listElementPrefab;
    [SerializeField] RectTransform listElementContainer;

    List<SavedTopologyListElement> topologies = new List<SavedTopologyListElement> ();

    public void Setup ()
    {
        List<SavedTopologyData> topologiesData = SaveManager.Instance.GetSavedTopologies ();

        for (int i = 0; i < topologiesData.Count; i ++)
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
            for (int i = 0; i < diff; i ++)
            {
                deleteLastListElement ();
            }
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
    }
}
