using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SavedTopologyListElement : MonoBehaviour
{
    public delegate void SavedTopologyListElementOnClickEventHandler (SavedTopologyListElement requester);
    public event SavedTopologyListElementOnClickEventHandler OnClick;

    [SerializeField] GameObject emptyListElementContent;
    [SerializeField] GameObject saveTopologContnet;

    [SerializeField] Text topologyName;
    [SerializeField] Text sensors;
    [SerializeField] Text additionalInputs;
    [SerializeField] Text hiddenLayer;
    [SerializeField] Text outputs;

    [SerializeField] Button button;
    [SerializeField] Image background;
    [SerializeField] Image selectionBackground;

    Color defaultBackgroundColor = Color.white;
    Color selectedBackgroundColor = new Color (242f/255f, 253/255f, 1f);
    
    public SavedTopologyData SavedTopologyData
    {
        get;
        private set;
    }

    public bool IsSelected
    {
        get;
        private set;
    }

    public void Select ()
    {
        IsSelected = true;
        selectionBackground.gameObject.SetActive (true);
        background.color = selectedBackgroundColor;
    }

    public void Deselect ()
    {
        IsSelected = false;
        selectionBackground.gameObject.SetActive (false);
        background.color = defaultBackgroundColor;
    }

    private void Awake ()
    {
        button.onClick.AddListener (() => onButtonClicked ());
    }

    public void Setup (SavedTopologyData savedTopologyData)
    {
        if (savedTopologyData != null)
        {
            SavedTopologyData = savedTopologyData;
            saveTopologContnet.SetActive (true);
            emptyListElementContent.SetActive (false);
            topologyName.text = savedTopologyData.TopologyName.ToUpper ();

            if (savedTopologyData.TopologyData != null)
            {
                sensors.text = savedTopologyData.TopologyData.SensorsCount.ToString ();
                hiddenLayer.text = savedTopologyData.TopologyData.HiddenLayerNeuronsCount.ToString ();

                int tmp = (savedTopologyData.TopologyData.MovementAngleInput ? 1 : 0) +
                    (savedTopologyData.TopologyData.SteerAngleInput ? 1 : 0) +
                    (savedTopologyData.TopologyData.TorqueInput ? 1 : 0) +
                    (savedTopologyData.TopologyData.VelocityInput ? 1 : 0);
                additionalInputs.text = tmp.ToString ();

                tmp = (savedTopologyData.TopologyData.HandbrakeOutput ? 1 : 0) +
                    (savedTopologyData.TopologyData.TorqueOutput ? 1 : 0) + 1;
                outputs.text = tmp.ToString ();
            }
        }
        else
        {
            SavedTopologyData = null;
            saveTopologContnet.SetActive (false);
            emptyListElementContent.SetActive (true);
        }
    }

    void onButtonClicked ()
    {
        OnClick?.Invoke (this);
    }
}
