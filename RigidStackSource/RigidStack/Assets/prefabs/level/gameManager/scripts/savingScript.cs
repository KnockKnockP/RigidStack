//Warning : Big spaghetti ahead.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LoadedPlayerData {
    public static PlayerData playerData = new PlayerData();
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

public class PlayerGraphics {
    public bool isBackgroundScalingKeepAspectRatio = false, isBackgroundEnabled = false;
    public int graphics = QualitySettings.GetQualityLevel(), verticalSyncCount = QualitySettings.vSyncCount;
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

    #region Graphics variables' names.
    public string[] graphicsVariablesNames = new string[] {
        ";Please take some time to read",
        ";https://docs.unity3d.com/ScriptReference/QualitySettings.html,",
        ";https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/index.html",
        ";to know what each value does.",
        ";Graphics settings menu values.",
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
        ";Universal render pipeline asset values.",
        nameof(UniversalRenderPipelineAsset.supportsCameraDepthTexture),
        nameof(UniversalRenderPipelineAsset.supportsCameraOpaqueTexture),
        nameof(UniversalRenderPipelineAsset.supportsHDR),
        nameof(UniversalRenderPipelineAsset.msaaSampleCount),
        nameof(UniversalRenderPipelineAsset.renderScale),
        //";Per Object Limit settings should be here but, I could not find it anywhere on the UniversalRenderPipelineAsset class.",
        nameof(UniversalRenderPipelineAsset.shadowDistance),
        nameof(UniversalRenderPipelineAsset.shadowCascadeOption),
        nameof(UniversalRenderPipelineAsset.shadowDepthBias),
        nameof(UniversalRenderPipelineAsset.shadowNormalBias),
        nameof(UniversalRenderPipelineAsset.colorGradingMode),
        nameof(UniversalRenderPipelineAsset.colorGradingLutSize),
        nameof(UniversalRenderPipelineAsset.useSRPBatcher),
        nameof(UniversalRenderPipelineAsset.supportsDynamicBatching),
        nameof(UniversalRenderPipelineAsset.shaderVariantLogLevel),
        ";Quality settings values.",
        nameof(QualitySettings.masterTextureLimit),
        nameof(QualitySettings.anisotropicFiltering),
        nameof(QualitySettings.realtimeReflectionProbes),
        nameof(QualitySettings.billboardsFaceCameraPosition),
        nameof(QualitySettings.resolutionScalingFixedDPIFactor),
        nameof(QualitySettings.streamingMipmapsActive),
        nameof(QualitySettings.streamingMipmapsAddAllCameras),
        nameof(QualitySettings.streamingMipmapsMemoryBudget),
        nameof(QualitySettings.streamingMipmapsMaxLevelReduction),
        nameof(QualitySettings.streamingMipmapsMaxFileIORequests),
        nameof(QualitySettings.skinWeights),
        nameof(QualitySettings.lodBias),
        nameof(QualitySettings.maximumLODLevel),
        nameof(QualitySettings.particleRaycastBudget),
        nameof(QualitySettings.asyncUploadTimeSlice),
        nameof(QualitySettings.asyncUploadBufferSize),
        nameof(QualitySettings.asyncUploadPersistentBuffer),
        ";If targetFramesPerSecond is set to -1, the game will render at the platform's default frame rate.",
        nameof(targetFramesPerSecond)
    };
    #endregion
}

public class savingScript : MonoBehaviour {
    //Common variables.
    private static byte count = 0;
    [Header("Common.")]
    [SerializeField] private UniversalRenderPipelineAsset[] _universalRenderPipelineAssets = null;
    public static UniversalRenderPipelineAsset[] universalRenderPipelineAssets;
    [SerializeField] private settingsScript _settingsScript = null;

    //Variables for the pre main menu.
    [Header("Pre main menu.")]
    [SerializeField] private Text noticeText = null;

    //Variables for the settings menu.
    [Header("Settings menu.")]
    [SerializeField] private Text savingText = null;
    [SerializeField] private Button saveButton = null, loadButton = null;

    //Variables for the profiles menu.
    private bool hasLoadedProfileListOnStart;
    private static readonly string defaultProfileName = "Default";
    private static string lastlySelectedProfileName = defaultProfileName;
    [Header("Profiles menu.")]
    [SerializeField] private Text profileNameText = null, newProfileExceptionText = null;
    [SerializeField] private Dropdown profilesDropdown = null;
    [SerializeField] private Button deleteButton = null;

    private class TypeAndObject {
        public Type type;
        public object _object;

        public TypeAndObject(Type _type, object __object) {
            type = _type;
            _object = __object;
        }
    };

    private void Start() {
        //Doing this useless operation to remove the unused variable warning.
        if (noticeText != null) {
            noticeText.text = noticeText.text;
        }
        if ((_universalRenderPipelineAssets != null) && (universalRenderPipelineAssets == null)) {
            universalRenderPipelineAssets = _universalRenderPipelineAssets;
        }
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

    #region Profiles.
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

    #region Profile lists.
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
                PlayerData loadedPlayerData = load(readLine, false);
                LoadedPlayerData.profiles.Add(loadedPlayerData);
                profileNames.Add(loadedPlayerData.name);
            }
            streamReader.Close();
            profilesDropdown.ClearOptions();
            profilesDropdown.AddOptions(profileNames);
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }
    #endregion

    #region Profile datas.
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

    public void load() {
        load(LoadedPlayerData.playerData.name, true);
        return;
    }

    private PlayerData load(string profileName, bool _loadGraphicsSettings) {
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
            if (_loadGraphicsSettings == true) {
                loadGraphicsSettings(profileName);
            }
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

    #region Graphics settings.
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
                    if (i != (LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length - 1)) {
                        streamWriter.Write("\r\n");
                    }
                    continue;
                }
                streamWriter.Write(LoadedPlayerData.playerGraphics.graphicsVariablesNames[i] + " = ");
                string variableName = LoadedPlayerData.playerGraphics.graphicsVariablesNames[i];
                object objectToWrite = "Something went wrong.";
                TypeAndObject[] typesAndObjects = new TypeAndObject[3] {
                    new TypeAndObject(typeof(PlayerGraphics), LoadedPlayerData.playerGraphics),
                    new TypeAndObject(typeof(QualitySettings), typeof(QualitySettings)),
                    new TypeAndObject(typeof(UniversalRenderPipelineAsset), QualitySettings.renderPipeline),
                };
                bool foundValue = false;
                foreach (TypeAndObject typeAndObject in typesAndObjects) {
                    FieldInfo fieldInfo = typeAndObject.type.GetField(variableName);
                    if (fieldInfo == null) {
                        PropertyInfo propertyInfo = typeAndObject.type.GetProperty(variableName);
                        if (propertyInfo != null) {
                            objectToWrite = propertyInfo.GetValue(typeAndObject._object);
                            foundValue = true;
                            break;
                        }
                    } else {
                        objectToWrite = fieldInfo.GetValue(typeAndObject._object);
                        foundValue = true;
                        break;
                    }
                }
                if (foundValue == false) {
                    Debug.LogWarning("\"" + variableName + "\"" + " is not a valid variable name.");
                    continue;
                }
                streamWriter.Write(objectToWrite);
                if (i != (LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length - 1)) {
                    streamWriter.Write("\r\n");
                }
            }
            streamWriter.Close();
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    public void loadGraphicsSettings(string profileName) {
        #if ENABLE_IL2CPP
            IL2CPPWarning("Loading");
            return;
        #endif
        if (count == 2) {
            count = 0;
            return;
        }
        count++;
        try {
            StreamReader streamReader = new StreamReader(getPath(false, false, true, profileName));
            //UniversalRenderPipelineAsset universalRenderPipelineAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            //StaticClass.CopyAllTo(universalRenderPipelineAssets[QualitySettings.GetQualityLevel()], universalRenderPipelineAsset);
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
                string variableName = null;
                for (int i = 0; i < LoadedPlayerData.playerGraphics.graphicsVariablesNames.Length; i++) {
                    if (splitLine[0] == LoadedPlayerData.playerGraphics.graphicsVariablesNames[i]) {
                        variableName = LoadedPlayerData.playerGraphics.graphicsVariablesNames[i];
                        hasFoundTheVariableName = true;
                        break;
                    }
                }
                if (hasFoundTheVariableName == false) {
                    Debug.LogWarning("\"" + splitLine[0] + "\" is not a valid variable name.");
                    continue;
                }
                TypeAndObject[] typesAndObjects = new TypeAndObject[3] {
                    new TypeAndObject(typeof(PlayerGraphics), LoadedPlayerData.playerGraphics),
                    new TypeAndObject(typeof(QualitySettings), typeof(QualitySettings)),
                    //new TypeAndObject(typeof(UniversalRenderPipelineAsset), universalRenderPipelineAsset),
                    new TypeAndObject(typeof(UniversalRenderPipelineAsset), QualitySettings.renderPipeline),
                };
                foreach (TypeAndObject typeAndObject in typesAndObjects) {
                    FieldInfo fieldInfo = typeAndObject.type.GetField(variableName);
                    if (fieldInfo == null) {
                        PropertyInfo propertyInfo = typeAndObject.type.GetProperty(variableName);
                        if (propertyInfo != null) {
                            setValues(null, propertyInfo, splitLine[1], typeAndObject);
                            break;
                        }
                    } else {
                        setValues(fieldInfo, null, splitLine[1], typeAndObject);
                        break;
                    }
                }
            }
            //QualitySettings.renderPipeline = universalRenderPipelineAsset;
            streamReader.Close();
            _settingsScript.updateGraphics(LoadedPlayerData.playerGraphics.graphics);
            _settingsScript.updateBackgroundEnabled(LoadedPlayerData.playerGraphics.isBackgroundEnabled);
            _settingsScript.updateBackgroundScaling(LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    private void setValues(FieldInfo fieldInfo, PropertyInfo propertyInfo, string variableInString, TypeAndObject typeAndObject) {
        if ((fieldInfo == null && propertyInfo == null)) {
            Debug.LogError("Both fieldInfo and propertyInfo can not be null");
            return;
        }
        Type variableType;
        if (fieldInfo != null) {
            variableType = fieldInfo.FieldType;
        } else {
            variableType = propertyInfo.PropertyType;
        }
        object objectToWriteAs;
        if (variableType == typeof(Vector3)) {
            string vector3Line = variableInString.Trim('(', ')');
            string[] splitVector3Line = vector3Line.Split(',');
            objectToWriteAs = new Vector3(float.Parse(splitVector3Line[0]), float.Parse(splitVector3Line[1]), float.Parse(splitVector3Line[2]));
        } else if (variableType.IsEnum == true) {
            objectToWriteAs = Enum.Parse(variableType, variableInString);
        } else {
            objectToWriteAs = Convert.ChangeType(variableInString, variableType);
        }
        if (fieldInfo != null) {
            fieldInfo.SetValue(typeAndObject._object, objectToWriteAs);
        } else {
            propertyInfo.SetValue(typeAndObject._object, objectToWriteAs);
        }
        return;
    }
    #endregion

    private void updateAll() {
        _settingsScript.updateManualChecking(LoadedPlayerData.playerData.isManualCheckingEnabled);
        _settingsScript.updateDifficulty(LoadedPlayerData.playerData.difficulty);
        _settingsScript.updateBackgroundEnabled(LoadedPlayerData.playerGraphics.isBackgroundEnabled);
        _settingsScript.updateBackgroundScaling(LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
        _settingsScript.updateVerticalSyncCount(LoadedPlayerData.playerGraphics.verticalSyncCount);
        return;
    }

    private string getPath(bool getSavesFolder, bool getProfileList, bool getGraphicsSettings, string playerName) {
        #if UNITY_EDITOR
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

    private bool checkIfFileExists(string path) {
        if (File.Exists(path) == false) {
            savingText.color = Color.red;
            savingText.text = "Save file was not found.";
            Debug.LogError("Save file was not found in " + path);
            return false;
        }
        return true;
    }

    private void disableOrEnabledButtons(bool status) {
        if ((saveButton != null) && (loadButton != null)) {
            saveButton.interactable = status;
            loadButton.interactable = status;
        }
        return;
    }

    #region IL2CPP warning.
    #if ENABLE_IL2CPP
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
    #endif
    #endregion

    private void catchException(Exception exception) {
        savingText.gameObject.SetActive(true);
        savingText.color = Color.red;
        savingText.text = exception.Message;
        Debug.LogError(exception);
        return;
    }
}