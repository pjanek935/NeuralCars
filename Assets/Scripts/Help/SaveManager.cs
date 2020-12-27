using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    public const int STAGES_COUNT = 5;
    public const int TOPOLOGIES_COUNT = 7;
    const string stageNameFormat = "STAGE_{0}.stg";
    const string topologyNameFormat = "TOPOLOGY_{0}.tpg";
    const string defaultStagePath = "DefaultStage";

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

        if (slotId >= 0 && slotId < STAGES_COUNT)
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
        if (slotId >= 0 && slotId < STAGES_COUNT)
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

    public StageModel LoadDefaultStage ()
    {
        StageModel result = null;
        TextAsset textAsset = Resources.Load<TextAsset> (defaultStagePath);

        if (textAsset != null)
        {
            result = JsonUtility.FromJson<StageModel> (textAsset.text);
        }

        return result;
    }

    public StageModel LoadStage (int slotId)
    {
        StageModel result = null;

        if (slotId >= 0 && slotId < STAGES_COUNT)
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

    public List <StageModel> GetSavedStages ()
    {
        List<StageModel> stages = new List<StageModel> ();

        for (int i = 0; i < STAGES_COUNT; i ++)
        {
            string fileName = string.Format (stageNameFormat, i);
            string path = Application.persistentDataPath + "/" + fileName;

            if (File.Exists (path))
            {
                FileStream file = File.OpenRead (path);
                BinaryFormatter bf = new BinaryFormatter ();
                string json = (string) bf.Deserialize (file);
                file.Close ();

                StageModel stageModel = JsonUtility.FromJson<StageModel> (json);
                stages.Add (stageModel);
            }
            else
            {
                stages.Add (null);
            }
        }

        return stages;
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

    public void SaveTopologyOnSlot (SavedTopologyData savedTopologyData, int slotId)
    {
        if (savedTopologyData != null && slotId >= 0 && slotId < TOPOLOGIES_COUNT)
        {
            string json = JsonUtility.ToJson (savedTopologyData);
            string fileName = string.Format (topologyNameFormat, slotId);
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
