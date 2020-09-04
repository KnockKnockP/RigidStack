//Warning : Big spaghetti ahead.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LoadedPlayerData {
    public static PlayerData playerData = null;
    public static PlayerGraphics playerGraphics = new PlayerGraphics();
    public static List<PlayerData> profiles = new List<PlayerData>();
}

[Serializable]
public class PlayerData {
    public bool isManualCheckingEnabled = false;
    public int maxHeight = 0;
    public string name = "";
    public Difficulty difficulty = Difficulty.Easy;
}

[Serializable]
public class PlayerGraphics {
    public bool isBackgroundEnabled = true, isBackgroundScalingKeepAspectRatio = false;
    public int graphics = 0, verticalSyncCount = 0;
    public string[] graphicsVariablesNames = new string[4] {
        nameof(isBackgroundEnabled),
        nameof(isBackgroundScalingKeepAspectRatio),
        nameof(graphics),
        nameof(verticalSyncCount)
    };
}

public class savingScript : MonoBehaviour {
    /*
        Sha512("___END_OF_FILE___") == "b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df".
        I hope no one ever decides to make a profile named "b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df".
    */

    [SerializeField] private settingsScript _settingsScript = null;
    [SerializeField] private Text savingText = null;
    [SerializeField] private Button saveButton = null, loadButton = null;


    private static bool hasLoadedProfileListOnStart;
    private readonly string defaultProfileName = "Default";
    [SerializeField] private Text profileNameText = null, newProfileExceptionText = null;
    [SerializeField] private Dropdown profilesDropdown = null;
    [SerializeField] private Button deleteButton = null;

    private void Start() {
        string savesFolderPath = getPath(true, false, false, null);
        if (Directory.Exists(savesFolderPath) == false) {
            Directory.CreateDirectory(savesFolderPath);
        }
        if (SceneManager.GetActiveScene().name == SceneNames.preMainMenu) {
            if (File.Exists(getPath(false, true, false, null)) == false) {
                makeNewProfile(defaultProfileName);
            } else if (hasLoadedProfileListOnStart == false) {
                loadProfiles();
                hasLoadedProfileListOnStart = true;
            } else {
                selectProfile(LoadedPlayerData.playerData.name);
            }
        }
        return;
    }

    public void checkNameNoReturn(InputField inputField) {
        checkName(inputField);
        return;
    }

    private bool checkName(InputField inputField) {
        return checkName(inputField.text);
    }

    private bool checkName(string inputText) {
        newProfileExceptionText.color = Color.red;
        if (inputText.Length == 0) {
            newProfileExceptionText.text = "Profile name can not be empty.";
            newProfileExceptionText.gameObject.SetActive(true);
            return false;
        }
        for (int i = 0; i < inputText.Length; i++) {
            if ((inputText[i] == '\\') ||
                (inputText[i] == '\"') ||
                (inputText[i] == '*') ||
                (inputText[i] == '|') ||
                (inputText[i] == '<') ||
                (inputText[i] == '>') ||
                (inputText[i] == ':') ||
                (inputText[i] == '?')) {
                newProfileExceptionText.text = "Profile name can not contain \"" + inputText[i] + "\".";
                newProfileExceptionText.gameObject.SetActive(true);
                return false;
            }
        }
        foreach (PlayerData playerData in LoadedPlayerData.profiles) {
            if (playerData.name == inputText) {
                newProfileExceptionText.text = "Duplicate profile name found.";
                newProfileExceptionText.gameObject.SetActive(true);
                return false;
            }
        }
        newProfileExceptionText.gameObject.SetActive(false);
        return true;
    }

    public void makeNewProfile(InputField inputField) {
        if (checkName(inputField) == true) {
            makeNewProfile(inputField.text);
        }
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
        selectProfile(newPlayerData.name);
        Debug.Log("Added a new profile with the name : " + newPlayerData.name + ".");
        return;
    }

    public void deleteProfile() {
        try {
            File.Delete(getPath(false, false, false, LoadedPlayerData.profiles[profilesDropdown.value].name));
        } catch (Exception exception) {
            catchException(exception);
        }
        LoadedPlayerData.profiles[profilesDropdown.value] = null;
        List<PlayerData> newProfileList = new List<PlayerData>();
        foreach (PlayerData playerData in LoadedPlayerData.profiles) {
            if (playerData != null) {
                newProfileList.Add(playerData);
            }
        }
        LoadedPlayerData.profiles = newProfileList;
        saveProfiles();
        loadProfiles();
        return;
    }

    public void selectProfile() {
        LoadedPlayerData.playerData = LoadedPlayerData.profiles[profilesDropdown.value];
        profileNameText.text = (LoadedPlayerData.playerData.name + ".");
        deleteButton.interactable = true;
        if (LoadedPlayerData.playerData.name == defaultProfileName) {
            deleteButton.interactable = false;
        }
        return;
    }

    private void selectProfile(string profileName) {
        for (int i = 0; i < LoadedPlayerData.profiles.Count; i++) {
            if (LoadedPlayerData.profiles[i].name == profileName) {
                LoadedPlayerData.playerData = LoadedPlayerData.profiles[i];
                profilesDropdown.value = i;
                profileNameText.text = (LoadedPlayerData.playerData.name + ".");
                break;
            }
        }
        deleteButton.interactable = true;
        if (LoadedPlayerData.playerData.name == defaultProfileName) {
            deleteButton.interactable = false;
        }
        return;
    }

    public void saveProfiles() {
        string path = getPath(false, true, false, null);
        try {
            StreamWriter streamWriter = new StreamWriter(path);
            for (int i = 0; i < LoadedPlayerData.profiles.Count; i++) {
                streamWriter.Write(LoadedPlayerData.profiles[i].name + "\r\n");
            }
            streamWriter.Write("b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df");
            streamWriter.Close();
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    private void loadProfiles() {
        string path = getPath(false, true, false, null);
        try {
            if (checkIfFileExists(path) == false) {
                return;
            }
            StreamReader streamReader = new StreamReader(path);
            List<string> profileNames = new List<string>();
            LoadedPlayerData.profiles.Clear();
            while (true) {
                string readLine = streamReader.ReadLine();
                if (readLine == "b4072cc4df126417585770a45b3e06de26625044b0ccbfeabc439038c879b4afd0007e8ec6f09b5c5bfca4405b2b566f42da55a4086d3c75c60cdcb5213024df") {
                    break;
                } else if (checkName(readLine) == false) {
                    continue;
                }
                PlayerData loadedPlayerData = load(readLine);
                LoadedPlayerData.profiles.Add(loadedPlayerData);
                profileNames.Add(loadedPlayerData.name);
            }
            streamReader.Close();
            profilesDropdown.ClearOptions();
            profilesDropdown.AddOptions(profileNames);
            selectProfile(LoadedPlayerData.profiles[0].name);
        } catch (Exception exception) {
            catchException(exception);
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
        try {
            /*
                Make variables.
                Make an array of strings that contains the name of the variables.
                Use reflector to find the variable and save it.
            */
            FileStream fileStream = File.Create(getPath(false, false, false, profileName));
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, LoadedPlayerData.playerData);
            fileStream.Close();
            saveGraphicsSettings(profileName);
        } catch (Exception exception) {
            catchException(exception);
        } finally {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
        return;
    }

    private void saveGraphicsSettings(string profileName) {
        try {
            StreamWriter streamWriter = new StreamWriter(getPath(false, false, true, profileName));
            for (int i = 0; i < LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length; i++) {
                streamWriter.Write(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i] + " = ");
                streamWriter.Write(LoadedPlayerData.playerGraphics.GetType().GetField(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]).GetValue(LoadedPlayerData.playerGraphics));
                if (i != (LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length - 1)) {
                    streamWriter.Write("\r\n");
                }
            }
            streamWriter.Flush();
            streamWriter.Close();
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    public PlayerData load(string profileName) {
        saveButton.interactable = false;
        loadButton.interactable = false;
        string path = getPath(false, false, false, profileName);
        PlayerData playerData = null;
        try {
            if (checkIfFileExists(path) == false) {
                return playerData;
            }
            FileStream fileStream = File.OpenRead(path);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            playerData = (PlayerData)(binaryFormatter.Deserialize(fileStream));
            fileStream.Close();
            loadGraphicsSettings(profileName);
            if (SceneManager.GetActiveScene().name != "preMainMenu") {
                updateAll();
            }
        } catch (Exception exception) {
            savingText.color = Color.red;
            savingText.text = exception.Message + ".";
        } finally {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
        return playerData;
    }

    private void loadGraphicsSettings(string profileName) {
        try {
            StreamReader streamReader = new StreamReader(getPath(false, false, true, profileName));
            while (streamReader.EndOfStream == false) {
                string readLine = streamReader.ReadLine();
                if (readLine.Contains(";") == true) {
                    continue;
                }
                string[] splitLine = readLine.Split('=');
                splitLine[0] = splitLine[0].Trim(' ');
                splitLine[1] = splitLine[1].Trim(' ');
                int i;
                for (i = 0; i < LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length; i++) {
                    if (splitLine[0] == LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]) {
                        break;
                    }
                }
                FieldInfo fieldInfo = LoadedPlayerData.playerGraphics.GetType().GetField(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                object valueToSet = Convert.ChangeType(splitLine[1], fieldInfo.FieldType);
                fieldInfo.SetValue(LoadedPlayerData.playerGraphics, valueToSet);
            }
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    private void updateAll() {
        _settingsScript.updateManualChecking(LoadedPlayerData.playerData.isManualCheckingEnabled);
        Debug.Log("Updated max height to " + LoadedPlayerData.playerData.maxHeight);
        _settingsScript.updateDifficulty(LoadedPlayerData.playerData.difficulty);
        _settingsScript.updateBackgroundEnabled(LoadedPlayerData.playerGraphics.isBackgroundEnabled);
        _settingsScript.updateBackgroundScaling(LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
        _settingsScript.updateVerticalSyncCount(LoadedPlayerData.playerGraphics.verticalSyncCount);
        return;
    }

    private string getPath(bool getSavesFolder, bool getProfileList, bool getGraphicsSettings, string playerName) {
        #if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
            string dataPath = Application.dataPath;
        #else
            string dataPath = Application.persistentDataPath
        #endif
        if (getSavesFolder == true) {
            return (dataPath + "/Saves/");
        }
        if (getProfileList == true) {
            return (dataPath + "/Saves/Profiles.txt");
        }
        if (getGraphicsSettings == true) {
            return (dataPath + "/Saves/" + playerName + "Graphics.txt");
        }
        return (dataPath + "/Saves/" + playerName + ".RS");
    }

    private bool checkIfFileExists(string path) {
        if (File.Exists(path) == false) {
            savingText.color = Color.red;
            savingText.text = "Save file was not found.";
            Debug.LogError("Save file was not found in " + path);
            return false;
        }
        return true;
    }

    private void catchException(Exception exception) {
        savingText.gameObject.SetActive(true);
        savingText.color = Color.red;
        savingText.text = exception.Message + ".";
        Debug.LogError(exception);
        return;
    }
}