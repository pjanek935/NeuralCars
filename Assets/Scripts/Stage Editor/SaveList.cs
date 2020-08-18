using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveList : MonoBehaviour
{
    public delegate void OnElementOnListClickedEventHandler (SaveList list, int slotId);
    public event OnElementOnListClickedEventHandler OnElementOnListClicked;

    [SerializeField] GameObject listElementPrefab;

    List<ListElement> slots = new List<ListElement> ();

    private void Awake ()
    {
        Refresh ();
    }

    public void Refresh ()
    {
        List<bool> saves = new List<bool> ();
        SaveManager saveManager = SaveManager.Instance;

        for (int i = 0; i < SaveManager.STAGES_COUNT; i ++)
        {
            saves.Add (saveManager.IsSlotEmpty (i));
        }

        for (int i = 0; i < saves.Count; i ++)
        {
            if (i >= slots.Count)
            {
                createNewSlot ();
            }

            slots [i].SetName (saves [i] ? "-Empty Slot-" : "Stage 0" + i);
        }

        int diff = slots.Count - saves.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i ++)
            {
                deleteLastSlot ();
            }
        }
    }

    void onListElementClicked (ListElement listElement)
    {
        int slotId = slots.IndexOf (listElement);

        if (slotId >= 0)
        {
            OnElementOnListClicked?.Invoke (this, slotId);
        }
    }

    void createNewSlot ()
    {
        GameObject newGameObject = Instantiate (listElementPrefab);
        newGameObject.SetActive (true);
        newGameObject.transform.SetParent (this.transform, false);
        ListElement listElement = newGameObject.GetComponent<ListElement> ();
        listElement.OnButtonClicked.AddListener (() => { onListElementClicked (listElement); });
        slots.Add (listElement);
    }

    void deleteLastSlot ()
    {
        if (slots.Count > 0)
        {
            GameObject tmp = slots [slots.Count - 1].gameObject;
            slots.RemoveAt (slots.Count - 1);
            Destroy (tmp);
        }
    }
}
