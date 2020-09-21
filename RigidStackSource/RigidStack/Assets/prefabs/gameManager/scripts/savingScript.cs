//Warning : Big spaghetti ahead.

#region Using tags.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endregion

#region Static LoadedPlayerData class.
public static class LoadedPlayerData {
    #region Variables for accessing player datas.
    public static PlayerData playerData = new PlayerData();
    public static PlayerGraphics playerGraphics = new PlayerGraphics();
    public static List<PlayerData> profiles = new List<PlayerData>();
    #endregion
}
#endregion

#region Serializable PlayerData class.
[Serializable]
public class PlayerData {
    #region Generic player variables.
    public bool isManualCheckingEnabled = false;
    public int maxHeight = 0;
    public string name = "";
    public Difficulty difficulty = Difficulty.Easy;
    #endregion
}
#endregion

#region Serializable PlayerGraphics class.
[Serializable]
public class PlayerGraphics {
    #region Graphics variables.
    public bool isBackgroundScalingKeepAspectRatio = false, isBackgroundEnabled = false;
    public int graphics = 0, verticalSyncCount = 0;
    private static int _targetFramesPerSecond = 60;
    public static int targetFramesPerSecond {
        get {
            return _targetFramesPerSecond;
        }
        set {
            _targetFramesPerSecond = value;
            Application.targetFrameRate = value;
        }
    }
    public string[] graphicsVariablesNames = new string[] {
        ";https://docs.unity3d.com/ScriptReference/QualitySettings.html",
        ";Graphics settings menu values.",
        ";",
        ";graphics = [0 (\"Potato.\"), 1 (\"Low.\"), 2 (\"Medium.\"), 3 (\"High.\"), 4 (\"Very high.\")].",
        ";The game will set the graphics value first and then overwrite all other values.",
        nameof(graphics),
        nameof(verticalSyncCount),
        ";isBackgroundScalingKeepAspectRatio = [False, True].",
        ";When isBackgroundScalingKeepAspectRatio is set to false, the game will stretch background objects to fit the screen.",
        nameof(isBackgroundScalingKeepAspectRatio),
        ";isBackgroundEnabled = [False, True].",
        ";When isBackgroundEnabled is set to false, the game will not generate any background objects.",
        nameof(isBackgroundEnabled),
        ";",
        ";QualitySettings values.",
        ";",
        nameof(QualitySettings.pixelLightCount),
        nameof(QualitySettings.masterTextureLimit),
        nameof(QualitySettings.anisotropicFiltering),
        nameof(QualitySettings.antiAliasing),
        nameof(QualitySettings.softParticles),
        nameof(QualitySettings.realtimeReflectionProbes),
        nameof(QualitySettings.billboardsFaceCameraPosition),
        nameof(QualitySettings.resolutionScalingFixedDPIFactor),
        nameof(QualitySettings.streamingMipmapsActive),
        nameof(QualitySettings.streamingMipmapsAddAllCameras),
        nameof(QualitySettings.streamingMipmapsMemoryBudget),
        nameof(QualitySettings.streamingMipmapsMaxLevelReduction),
        nameof(QualitySettings.streamingMipmapsMaxFileIORequests),
        nameof(QualitySettings.shadowmaskMode),
        nameof(QualitySettings.shadows),
        nameof(QualitySettings.shadowResolution),
        nameof(QualitySettings.shadowProjection),
        nameof(QualitySettings.shadowDistance),
        nameof(QualitySettings.shadowNearPlaneOffset),
        nameof(QualitySettings.shadowCascades),
        nameof(QualitySettings.shadowCascade2Split),
        nameof(QualitySettings.shadowCascade4Split),
        nameof(QualitySettings.skinWeights),
        nameof(QualitySettings.lodBias),
        nameof(QualitySettings.maximumLODLevel),
        nameof(QualitySettings.particleRaycastBudget),
        nameof(QualitySettings.asyncUploadTimeSlice),
        nameof(QualitySettings.asyncUploadBufferSize),
        nameof(QualitySettings.asyncUploadPersistentBuffer),
        nameof(targetFramesPerSecond)
    };
    #endregion
}
#endregion

#region "MonoBehaviour" inherited "savingScript" class.
public class savingScript : MonoBehaviour {
    #region Varibales.
    #region Variables for the pre main menu.
    [SerializeField] private Text noticeText = null;
    #endregion

    #region Variables for the settings menu.
    [SerializeField] private settingsScript _settingsScript = null;
    [SerializeField] private Text savingText = null;
    [SerializeField] private Button saveButton = null, loadButton = null;
    #endregion

    #region Variables for the profiles menu.
    private bool hasLoadedProfileListOnStart;
    private static readonly string defaultProfileName = "Default";
    private static string lastlySelectedProfileName = defaultProfileName;
    [SerializeField] private Text profileNameText = null, newProfileExceptionText = null;
    [SerializeField] private Dropdown profilesDropdown = null;
    [SerializeField] private Button deleteButton = null;
    #endregion
    #endregion

    #region Start function.
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
                selectProfile(lastlySelectedProfileName);
                hasLoadedProfileListOnStart = true;
            } else {
                selectProfile(lastlySelectedProfileName);
            }
        }
        return;
    }
    #endregion

    #region Making a new profile.
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
    #endregion

    #region Deleting a profile.
    public void deleteProfile() {
        try {
            File.Delete(getPath(false, false, false, LoadedPlayerData.profiles[profilesDropdown.value].name));
            File.Delete(getPath(false, false, true, LoadedPlayerData.profiles[profilesDropdown.value].name));
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
    #endregion

    #region Selecting a profile.
    public void selectProfile() {
        selectProfile(LoadedPlayerData.profiles[profilesDropdown.value].name);
        return;
    }

    private void selectProfile(string profileName) {
        for (int i = 0; i < LoadedPlayerData.profiles.Count; i++) {
            if (LoadedPlayerData.profiles[i].name == profileName) {
                LoadedPlayerData.playerData = LoadedPlayerData.profiles[i];
                lastlySelectedProfileName = LoadedPlayerData.playerData.name;
                profilesDropdown.value = i;
                profileNameText.text = (LoadedPlayerData.playerData.name + ".");
                load();
                saveProfiles();
                break;
            }
        }
        deleteButton.interactable = true;
        if (LoadedPlayerData.playerData.name == defaultProfileName) {
            deleteButton.interactable = false;
        }
        return;
    }
    #endregion

    #region Saving a profile list.
    public void saveProfiles() {
        string path = getPath(false, true, false, null);
        try {
            StreamWriter streamWriter = new StreamWriter(path);
            for (int i = 0; i < LoadedPlayerData.profiles.Count; i++) {
                streamWriter.Write(LoadedPlayerData.profiles[i].name + "\r\n");
            }
            streamWriter.Write("Lastly selected profile name : " + lastlySelectedProfileName);
            streamWriter.Close();
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }
    #endregion

    #region Loading a profile list.
    private void loadProfiles() {
        string path = getPath(false, true, false, null);
        try {
            if (checkIfFileExists(path) == false) {
                return;
            }
            StreamReader streamReader = new StreamReader(path);
            List<string> profileNames = new List<string>();
            LoadedPlayerData.profiles.Clear();
            string profileToLoad = defaultProfileName;
            while (streamReader.EndOfStream == false) {
                string readLine = streamReader.ReadLine();
                string[] splitLine = readLine.Split(':');
                if (splitLine.Length == 2) {
                    profileToLoad = splitLine[1].Trim(' ');
                    continue;
                }
                if (checkName(readLine) == false) {
                    continue;
                }
                PlayerData loadedPlayerData = load(readLine);
                LoadedPlayerData.profiles.Add(loadedPlayerData);
                profileNames.Add(loadedPlayerData.name);
            }
            streamReader.Close();
            profilesDropdown.ClearOptions();
            profilesDropdown.AddOptions(profileNames);
            selectProfile(profileToLoad);
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }
    #endregion

    #region Saving a profile data.
    public void save() {
        save(LoadedPlayerData.playerData.name);
        return;
    }

    private void save(string profileName) {
        disableOrEnabledButtons(false);
        try {
            FileStream fileStream = File.Create(getPath(false, false, false, profileName));
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, LoadedPlayerData.playerData);
            fileStream.Close();
            saveGraphicsSettings(profileName);
        } catch (Exception exception) {
            catchException(exception);
        } finally {
            disableOrEnabledButtons(true);
        }
        return;
    }
    #endregion

    #region Loading a profile data.
    public void load() {
        load(LoadedPlayerData.playerData.name);
        return;
    }

    private PlayerData load(string profileName) {
        disableOrEnabledButtons(false);
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
            catchException(exception);
        } finally {
            disableOrEnabledButtons(true);
        }
        return playerData;
    }
    #endregion

    #region Saving a profile's graphics settings.
    private void saveGraphicsSettings(string profileName) {
        #if ENABLE_IL2CPP
            IL2CPPWarning("Saving");
            return;
        #endif
        try {
            StreamWriter streamWriter = new StreamWriter(getPath(false, false, true, profileName));
            for (int i = 0; i < LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length; i++) {
                if (LoadedPlayerData.playerGraphics.graphicsVariablesNames[i].Contains(';') == true) {
                    streamWriter.Write(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                    streamWriter.Flush();
                    if (i != (LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length - 1)) {
                        streamWriter.Write("\r\n");
                        streamWriter.Flush();
                    }
                    continue;
                }
                streamWriter.Write(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i] + " = ");
                streamWriter.Flush();
                FieldInfo fieldInfo = typeof(PlayerGraphics).GetField(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                object objectToWrite;
                if (fieldInfo == null) {
                    PropertyInfo propertyInfo = typeof(QualitySettings).GetProperty(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                    if (propertyInfo == null) {
                        propertyInfo = typeof(PlayerGraphics).GetProperty(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                        if (propertyInfo == null) {
                            continue;
                        }
                        objectToWrite = propertyInfo.GetValue(LoadedPlayerData.playerGraphics);
                    } else {
                        objectToWrite = propertyInfo.GetValue(typeof(QualitySettings));
                    }
                } else {
                    objectToWrite = fieldInfo.GetValue(LoadedPlayerData.playerGraphics);
                }
                streamWriter.Write(objectToWrite);
                streamWriter.Flush();
                if (i != (LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length - 1)) {
                    streamWriter.Write("\r\n");
                    streamWriter.Flush();
                }
            }
            streamWriter.Flush();
            streamWriter.Close();
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }
    #endregion

    #region Loading a profile's graphics settings.
    public void loadGraphicsSettings(string profileName) {
        #if ENABLE_IL2CPP
            IL2CPPWarning("Loading");
            return;
        #endif
        try {
            StreamReader streamReader = new StreamReader(getPath(false, false, true, profileName));
            while (streamReader.EndOfStream == false) {
                string readLine = streamReader.ReadLine();
                if (readLine.Contains(";") == true) {
                    continue;
                }
                List<string> splitLine = readLine.Split('=').ToList();
                if (splitLine.Count != 2) {
                    Debug.LogWarning("Invalid syntax read.");
                    continue;
                }
                splitLine[0] = splitLine[0].Trim(' ');
                splitLine[1] = splitLine[1].Trim(' ');
                if (splitLine[1] == "") {
                    Debug.LogWarning("There is no value to set to.");
                    continue;
                }
                bool hasFoundTheVariableName = false;
                int i;
                for (i = 0; i < LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length; i++) {
                    if (splitLine[0] == LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]) {
                        hasFoundTheVariableName = true;
                        break;
                    }
                }
                if (hasFoundTheVariableName == false) {
                    Debug.LogWarning("\"" + splitLine[0] + "\" is not a valid variable name.");
                    continue;
                }
                Type infoType = typeof(PlayerGraphics);
                FieldInfo fieldInfo = infoType.GetField(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                if (fieldInfo == null) {
                    PropertyInfo propertyInfo = infoType.GetProperty(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                    if (propertyInfo == null) {
                        infoType = typeof(QualitySettings);
                        propertyInfo = infoType.GetProperty(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]);
                        if (propertyInfo == null) {
                            continue;
                        }
                    }
                    Type variableType = propertyInfo.PropertyType;
                    try {
                        propertyInfo.SetValue(infoType, Convert.ChangeType(splitLine[1], variableType));
                    } catch (Exception exception) {
                        if (exception.GetType() == typeof(InvalidCastException)) {
                            if (variableType == typeof(Vector3)) {
                                string vector3Line = splitLine[1].Trim('(', ')');
                                string[] splitVector3Line = vector3Line.Split(',');
                                propertyInfo.SetValue(typeof(QualitySettings), new Vector3(float.Parse(splitVector3Line[0]), float.Parse(splitVector3Line[1]), float.Parse(splitVector3Line[2])));
                            } else {
                                propertyInfo.SetValue(typeof(QualitySettings), Enum.Parse(variableType, splitLine[1]));
                            }
                        } else {
                            catchException(exception);
                        }
                    }
                    continue;
                }
                try {
                    fieldInfo.SetValue(LoadedPlayerData.playerGraphics, Convert.ChangeType(splitLine[1], fieldInfo.FieldType));
                } catch (Exception exception) {
                    catchException(exception);
                    continue;
                }
            }
            streamReader.Close();
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }
    #endregion

    #region Updating player datas.
    private void updateAll() {
        _settingsScript.updateManualChecking(LoadedPlayerData.playerData.isManualCheckingEnabled);
        Debug.Log("Updated max height to " + LoadedPlayerData.playerData.maxHeight);
        _settingsScript.updateDifficulty(LoadedPlayerData.playerData.difficulty);
        _settingsScript.updateBackgroundEnabled(LoadedPlayerData.playerGraphics.isBackgroundEnabled);
        _settingsScript.updateBackgroundScaling(LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
        _settingsScript.updateVerticalSyncCount(LoadedPlayerData.playerGraphics.verticalSyncCount);
        return;
    }
    #endregion

    #region Getting a path.
    private string getPath(bool getSavesFolder, bool getProfileList, bool getGraphicsSettings, string playerName) {
        #if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
            string dataPath = Application.dataPath;
        #else
            string dataPath = Application.persistentDataPath;
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
    #endregion

    #region Name checking.
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
                (inputText[i] == '?') ||
                (inputText[i] == ' ')) {
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
    #endregion

    #region Checking if file exists.
    private bool checkIfFileExists(string path) {
        if (File.Exists(path) == false) {
            savingText.color = Color.red;
            savingText.text = "Save file was not found.";
            Debug.LogError("Save file was not found in " + path);
            return false;
        }
        return true;
    }
    #endregion

    #region Enabling or disabling buttons.
    private void disableOrEnabledButtons(bool status) {
        if ((saveButton != null) && (loadButton != null)) {
            saveButton.interactable = status;
            loadButton.interactable = status;
        }
        return;
    }
    #endregion

    #region IL2CPP warning.
    private void IL2CPPWarning(string loadingOrSaving) {
        string text = (loadingOrSaving + " graphics settings file is not supported in IL2CPP build.");
        Color32 warningColor = new Color32(255, 200, 0, 255);
        if (savingText != null) {
            savingText.color = warningColor;
            savingText.text = text;
            savingText.gameObject.SetActive(true);
        }
        if (noticeText != null) {
            noticeText.color = warningColor;
            noticeText.text = text;
            noticeText.gameObject.SetActive(true);
        }
        return;
    }
    #endregion

    #region Catching an exception.
    private void catchException(Exception exception) {
        savingText.gameObject.SetActive(true);
        savingText.color = Color.red;
        savingText.text = exception.Message;
        Debug.LogError(exception);
        return;
    }
    #endregion
}
#endregion