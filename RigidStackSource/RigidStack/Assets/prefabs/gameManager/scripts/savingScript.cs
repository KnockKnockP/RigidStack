using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerData {
    public static bool isBackgroundEnabled = true, isManualCheckingEnabled = false;
    public static int verticalSyncCount = 0, maxHeight = 0;
    public static Difficulty difficulty = Difficulty.Easy;
}

[Serializable]
public class SerializablePlayerData {
    public bool isBackgroundEnabled = true, isManualCheckingEnabled = false;
    public int verticalSyncCount = 0, maxHeight = 0;
    public Difficulty difficulty = Difficulty.Easy;

    public SerializablePlayerData(bool isSaving = false) {
        if (isSaving == true) {
            isBackgroundEnabled = PlayerData.isBackgroundEnabled;
            isManualCheckingEnabled = PlayerData.isManualCheckingEnabled;
            verticalSyncCount = PlayerData.verticalSyncCount;
            maxHeight = PlayerData.maxHeight;
            difficulty = PlayerData.difficulty;
        }
        return;
    }
}

public class savingScript : MonoBehaviour {
    [SerializeField] private settingsScript _settingsScript = null;
    [SerializeField] private Button saveButton = null, loadButton = null;

    public void save() {
        saveButton.interactable = false;
        loadButton.interactable = false;
        #if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
            string path = (Application.dataPath + "/Save.RS");
        #else
            string path = (Application.persistentDataPath + "/Save.RS");
        #endif
        try {
            FileStream fileStream = File.Create(path);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SerializablePlayerData serializablePlayerData = new SerializablePlayerData(true);
            binaryFormatter.Serialize(fileStream, serializablePlayerData);
            fileStream.Close();
        } finally {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
        return;
    }

    public void load() {
        saveButton.interactable = false;
        loadButton.interactable = false;
        #if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
            string path = (Application.dataPath + "/Save.RS");
        #else
            string path = (Application.persistentDataPath + "/Save.RS");
        #endif
        try {
            if (File.Exists(path) == false) {
                Debug.LogError("Save file was not found in " + path);
                return;
            }
            FileStream fileStream = File.OpenRead(path);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SerializablePlayerData serializablePlayerData = (SerializablePlayerData)(binaryFormatter.Deserialize(fileStream));
            fileStream.Close();
            _settingsScript.updateBackgroundEnabled(serializablePlayerData.isBackgroundEnabled);
            _settingsScript.updateManualChecking(serializablePlayerData.isManualCheckingEnabled);
            _settingsScript.updateVerticalSyncCount(serializablePlayerData.verticalSyncCount);
            PlayerData.maxHeight = serializablePlayerData.maxHeight;
            Debug.Log("Updated max height to " + PlayerData.maxHeight);
            _settingsScript.updateDifficulty(serializablePlayerData.difficulty);
        } finally {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
        return;
    }
}