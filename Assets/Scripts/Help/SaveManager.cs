using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    public const int SLOTS_COUNT = 5;
    public const int TOPOLOGIES_COUNT = 7;
    const string stageNameFormat = "STAGE_{0}.stg";
    const string topologyNameFormat = "TOPOLOGY_{0}.tpg";

    static SaveManager instance;

    public int CurrentOpenedStageId
    {
        get;
        private set;
    }

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveManager ();
            }

            return instance;
        }
    }

    public static bool IsInstanceActive
    {
        get { return instance != null; }
    }

    private SaveManager ()
    {
        CurrentOpenedStageId = 0;
    }

    public bool IsSlotEmpty (int slotId)
    {
        bool result = false;

        if (slotId >= 0 && slotId < SLOTS_COUNT)
        {
            string fileName = string.Format (stageNameFormat, slotId);
            string destination = Application.persistentDataPath + "/" + fileName;

            if (! File.Exists (destination))
            {
                result = true;
            }
        }

        return result;
    }

    public void SaveStage (StageModel stageModel, int slotId)
    {
        if (slotId >= 0 && slotId < SLOTS_COUNT)
        {
            if (stageModel != null)
            {
                string json = JsonUtility.ToJson (stageModel);
                string fileName = string.Format (stageNameFormat, slotId);
                string destination = Application.persistentDataPath + "/" + fileName;
                FileStream file;

                if (File.Exists (destination))
                {
                    file = File.OpenWrite (destination);
                }
                else
                {
                    file = File.Create (destination);
                }

                BinaryFormatter bf = new BinaryFormatter ();
                bf.Serialize (file, json);
                file.Close ();
            }
        }
    }

    public StageModel LoadStage (int slotId)
    {
        StageModel result = null;

        if (slotId >= 0 && slotId < SLOTS_COUNT)
        {
            string fileName = string.Format (stageNameFormat, slotId);
            string destination = Application.persistentDataPath + "/" + fileName;
            FileStream file;

            if (File.Exists (destination))
            {
                file = File.OpenRead (destination);
                BinaryFormatter bf = new BinaryFormatter ();
                string json = (string) bf.Deserialize (file);
                file.Close ();

                result = JsonUtility.FromJson<StageModel> (json);

                if (result != null)
                {
                    CurrentOpenedStageId = slotId;
                }
            }
            else
            {
                CurrentOpenedStageId = slotId;
                Debug.Log ("File not found");
            }
        }

        return result;
    }

    public List <SavedTopologyData> GetSavedTopologies ()
    {
        List<SavedTopologyData> topologiesData = new List<SavedTopologyData> ();

        for (int i = 0; i < TOPOLOGIES_COUNT; i++)
        {
            string fileName = string.Format (topologyNameFormat, i);
            string path = Application.persistentDataPath + "/" + fileName;

            if (File.Exists (path))
            {
                FileStream file = File.OpenRead (path);
                BinaryFormatter bf = new BinaryFormatter ();
                string json = (string) bf.Deserialize (file);
                file.Close ();

                SavedTopologyData topology = JsonUtility.FromJson<SavedTopologyData> (json);
                topologiesData.Add (topology);
            }
            else
            {
                topologiesData.Add (null);
            }
        }

        return topologiesData;
    }
}
