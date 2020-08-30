﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LoadedPlayerData {
    public static PlayerData playerData = null;
    public static List<PlayerData> profiles = new List<PlayerData>();
}

[Serializable]
public class PlayerData {
    public bool isBackgroundEnabled = true, isManualCheckingEnabled = false, isBackgroundScalingKeepAspectRatio = false;
    public int verticalSyncCount = 0, maxHeight = 0;
    public string name = "";
    public Difficulty difficulty = Difficulty.Easy;
}

public class savingScript : MonoBehaviour {
    /*
        Sha512("___END_OF_FILE___") == "b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df".
        I hope no one ever decides to make a profile named "b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df".
    */

    [SerializeField] private settingsScript _settingsScript = null;
    [SerializeField] private Text savingText = null;
    [SerializeField] private Button saveButton = null, loadButton = null;


    [SerializeField] private Text profileNameText = null;
    [SerializeField] private Dropdown profilesDropdown = null;

    private void Start() {
        if (SceneManager.GetActiveScene().name == SceneNames.preMainMenu) {
            if (File.Exists(getPath(true)) == false) {
                makeNewProfile("Player 1");
                return;
            }
            loadProfiles();
        }
        return;
    }

    public void makeNewProfile(InputField inputField) {
        makeNewProfile(inputField.text);
        return;
    }

    public void makeNewProfile(string inputName) {
        PlayerData newPlayerData = new PlayerData {
            name = inputName
        };
        LoadedPlayerData.profiles.Add(newPlayerData);
        Dropdown.OptionData optionData = new Dropdown.OptionData {
            text = newPlayerData.name
        };
        profilesDropdown.options.Add(optionData);
        LoadedPlayerData.playerData = newPlayerData;
        saveProfiles();
        save(newPlayerData.name);
        profileNameText.text = (LoadedPlayerData.playerData.name + ".");
        Debug.Log("Added a new profile with the name : " + newPlayerData.name + ".");
        return;
    }

    public void deleteProfile() {
        LoadedPlayerData.profiles[profilesDropdown.value] = null;
        List<string> newProfileListName = new List<string>();
        List<PlayerData> newProfileList = new List<PlayerData>();
        foreach (PlayerData playerData in LoadedPlayerData.profiles) {
            if (playerData != null) {
                newProfileListName.Add(playerData.name);
                newProfileList.Add(playerData);
            }
        }
        LoadedPlayerData.profiles = newProfileList;
        profilesDropdown.ClearOptions();
        profilesDropdown.AddOptions(newProfileListName);
        return;
    }

    public void selectProfile() {
        LoadedPlayerData.playerData = LoadedPlayerData.profiles[profilesDropdown.value];
        profileNameText.text = (LoadedPlayerData.playerData.name + ".");
        return;
    }

    public void saveProfiles() {
        string path = getPath(true);
        try {
            StreamWriter streamWriter = new StreamWriter(path);
            for (int i = 0; i < LoadedPlayerData.profiles.Count; i++) {
                streamWriter.Write(LoadedPlayerData.profiles[i].name + "\r\n");
            }
            streamWriter.Write("b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df");
            streamWriter.Close();
        } catch (Exception exception) {
            savingText.gameObject.SetActive(true);
            savingText.color = Color.red;
            savingText.text = exception.Message + ".";
        }
        return;
    }

    private void loadProfiles() {
        string path = getPath(true);
        try {
            if (File.Exists(path) == false) {
                savingText.gameObject.SetActive(true);
                savingText.color = Color.red;
                savingText.text = "Save file was not found.";
                Debug.LogError("Save file was not found in " + path);
                return;
            }
            StreamReader streamReader = new StreamReader(path);
            List<string> profileNames = new List<string>();
            while (true) {
                string readLine = streamReader.ReadLine();
                if (readLine == "b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df") {
                    streamReader.Close();
                    break;
                }
                PlayerData loadedPlayerData = load(readLine);
                LoadedPlayerData.profiles.Add(loadedPlayerData);
                profileNames.Add(loadedPlayerData.name);
            }
            streamReader.Close();
            profilesDropdown.ClearOptions();
            profilesDropdown.AddOptions(profileNames);
            LoadedPlayerData.playerData = LoadedPlayerData.profiles[0];
            profileNameText.text = (LoadedPlayerData.playerData.name + ".");
        } catch (Exception exception) {
            savingText.gameObject.SetActive(true);
            savingText.color = Color.red;
            savingText.text = exception.Message + ".";
        }
        return;
    }

    public void save() {
        save(LoadedPlayerData.playerData.name);
        return;
    }

    private void save(string profileName) {
        saveButton.interactable = false;
        loadButton.interactable = false;
        string path = getPath(false, profileName);
        try {
            FileStream fileStream = File.Create(path);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, LoadedPlayerData.playerData);
            fileStream.Close();
        } catch (Exception exception) {
            savingText.color = Color.red;
            savingText.text = exception.Message;
        } finally {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
        return;
    }

    public PlayerData load(string profileName) {
        saveButton.interactable = false;
        loadButton.interactable = false;
        string path = getPath(false, profileName);
        PlayerData playerData = null;
        try {
            if (File.Exists(path) == false) {
                savingText.color = Color.red;
                savingText.text = "Save file was not found.";
                Debug.LogError("Save file was not found in " + path);
                return playerData;
            }
            FileStream fileStream = File.OpenRead(path);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            playerData = (PlayerData)(binaryFormatter.Deserialize(fileStream));
            fileStream.Close();
            updateAll();
        } catch (Exception exception) {
            savingText.color = Color.red;
            savingText.text = exception.Message + ".";
        } finally {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
        return playerData;
    }

    private void updateAll() {
        _settingsScript.updateBackgroundEnabled(LoadedPlayerData.playerData.isBackgroundEnabled);
        _settingsScript.updateManualChecking(LoadedPlayerData.playerData.isManualCheckingEnabled);
        _settingsScript.updateBackgroundScaling(LoadedPlayerData.playerData.isBackgroundScalingKeepAspectRatio);
        _settingsScript.updateVerticalSyncCount(LoadedPlayerData.playerData.verticalSyncCount);
        Debug.Log("Updated max height to " + LoadedPlayerData.playerData.maxHeight);
        _settingsScript.updateDifficulty(LoadedPlayerData.playerData.difficulty);
        return;
    }

    private string getPath(bool isProfileList, string playerName = "") {
        if (isProfileList == false) {
            #if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
                return (Application.dataPath + "/" + playerName + ".RS");
            #else
                return (Application.persistentDataPath + "/" + LoadedPlayerData.playerData.name + ".RS");
            #endif
        }
        #if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
                return (Application.dataPath + "/Profiles.txt");
        #else
                return (Application.persistentDataPath + "/Profiles.txt");
        #endif
    }
}