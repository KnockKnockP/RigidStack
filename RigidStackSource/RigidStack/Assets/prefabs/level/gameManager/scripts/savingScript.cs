//Warning : Big spaghetti ahead.
//I might just yeet everything out and start over.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

[Serializable]
public class ProfileList {
    public string lastlySelectedProfileName = savingScript.defaultProfileName;
    public List<string> profileNames = new List<string>();
    public static List<PlayerData> profiles = new List<PlayerData>();
}

[Serializable]
public class PlayerData {
    public bool isManualCheckingEnabled = false;
    public int maxHeight = 0;
    public string name = savingScript.defaultProfileName;
    public Difficulty difficulty = Difficulty.Easy;
}

#pragma warning disable IDE0044, IDE0052, 414
[JsonObject(MemberSerialization.OptIn)]
public class PlayerGraphics {
    [JsonProperty]
    private string[] _comment = { "If targetFramesPerSecond is set to -1, the game will render at the platform's default frame rate.",
                                  "graphics = [0 (Potato.), 1 (Low.), 2 (Medium.), 3 (High.), 4 (Very high.)].",
                                  "The game will set the graphics value first and then overwrite all other values.",
                                  "When isBackgroundScalingKeepAspectRatio is set to false, the game will stretch background objects to fit the screen." };
    [JsonProperty] public bool isBackgroundScalingKeepAspectRatio = false, isBackgroundEnabled = false;
    [JsonProperty] public int graphics = QualitySettings.GetQualityLevel(), verticalSyncCount = QualitySettings.vSyncCount;
    [JsonProperty]
    public int targetFramesPerSecond {
        get {
            return Application.targetFrameRate;
        }
        set {
            Application.targetFrameRate = value;
            return;
        }
    }
}
#pragma warning restore IDE0044, IDE0052, 414

#pragma warning disable IDE0044, IDE0051, IDE0052, 414
[JsonObject(MemberSerialization.OptIn)]
public class PlayerQualitySettings {
    [JsonProperty] private string _comment = "Documentations : https://docs.unity3d.com/ScriptReference/QualitySettings.html";
    [JsonProperty]
    public int masterTextureLimit {
        get {
            return QualitySettings.masterTextureLimit;
        }
        set {
            QualitySettings.masterTextureLimit = value;
            return;
        }
    }

    [JsonProperty]
    public AnisotropicFiltering anisotropicFiltering {
        get {
            return QualitySettings.anisotropicFiltering;
        }
        set {
            QualitySettings.anisotropicFiltering = value;
            return;
        }
    }

    [JsonProperty]
    public bool realtimeReflectionProbes {
        get {
            return QualitySettings.realtimeReflectionProbes;
        }
        set {
            QualitySettings.realtimeReflectionProbes = value;
            return;
        }
    }

    [JsonProperty]
    public bool billboardsFaceCameraPosition {
        get {
            return QualitySettings.billboardsFaceCameraPosition;
        }
        set {
            QualitySettings.billboardsFaceCameraPosition = value;
            return;
        }
    }

    [JsonProperty]
    public float resolutionScalingFixedDPIFactor {
        get {
            return QualitySettings.resolutionScalingFixedDPIFactor;
        }
        set {
            QualitySettings.resolutionScalingFixedDPIFactor = value;
            return;
        }
    }

    [JsonProperty]
    public bool streamingMipmapsActive {
        get {
            return QualitySettings.streamingMipmapsActive;
        }
        set {
            QualitySettings.streamingMipmapsActive = value;
            return;
        }
    }

    [JsonProperty]
    public bool streamingMipmapsAddAllCameras {
        get {
            return QualitySettings.streamingMipmapsAddAllCameras;
        }
        set {
            QualitySettings.streamingMipmapsAddAllCameras = value;
            return;
        }
    }

    [JsonProperty]
    public float streamingMipmapsMemoryBudget {
        get {
            return QualitySettings.streamingMipmapsMemoryBudget;
        }
        set {
            QualitySettings.streamingMipmapsMemoryBudget = value;
            return;
        }
    }

    [JsonProperty]
    public int streamingMipmapsMaxLevelReduction {
        get {
            return QualitySettings.streamingMipmapsMaxLevelReduction;
        }
        set {
            QualitySettings.streamingMipmapsMaxLevelReduction = value;
            return;
        }
    }

    [JsonProperty]
    public int streamingMipmapsMaxFileIORequests {
        get {
            return QualitySettings.streamingMipmapsMaxFileIORequests;
        }
        set {
            QualitySettings.streamingMipmapsMaxFileIORequests = value;
            return;
        }
    }

    [JsonProperty]
    public SkinWeights skinWeights {
        get {
            return QualitySettings.skinWeights;
        }
        set {
            QualitySettings.skinWeights = value;
            return;
        }
    }

    [JsonProperty]
    public float lodBias {
        get {
            return QualitySettings.lodBias;
        }
        set {
            QualitySettings.lodBias = value;
            return;
        }
    }

    [JsonProperty]
    public int maximumLODLevel {
        get {
            return QualitySettings.maximumLODLevel;
        }
        set {
            QualitySettings.maximumLODLevel = value;
            return;
        }
    }

    [JsonProperty]
    public int particleRaycastBudget {
        get {
            return QualitySettings.particleRaycastBudget;
        }
        set {
            QualitySettings.particleRaycastBudget = value;
            return;
        }
    }

    [JsonProperty]
    public int asyncUploadTimeSlice {
        get {
            return QualitySettings.asyncUploadTimeSlice;
        }
        set {
            QualitySettings.asyncUploadTimeSlice = value;
            return;
        }
    }

    [JsonProperty]
    public int asyncUploadBufferSize {
        get {
            return QualitySettings.asyncUploadBufferSize;
        }
        set {
            QualitySettings.asyncUploadBufferSize = value;
            return;
        }
    }

    [JsonProperty]
    public bool asyncUploadPersistentBuffer {
        get {
            return QualitySettings.asyncUploadPersistentBuffer;
        }
        set {
            QualitySettings.asyncUploadPersistentBuffer = value;
            return;
        }
    }
}
#pragma warning restore IDE0044, IDE0051, IDE0052, 414

public static class LoadedPlayerData {
    public static ProfileList profileList = new ProfileList();
    public static PlayerData playerData = new PlayerData();
    public static PlayerGraphics playerGraphics = new PlayerGraphics();
    public static PlayerQualitySettings playerQualitySettings = new PlayerQualitySettings();
}

public class savingScript : MonoBehaviour {
    [Header("Common."), SerializeField] private UniversalRenderPipelineAsset[] _universalRenderPipelineAssets = null;
    public static UniversalRenderPipelineAsset[] universalRenderPipelineAssets;
    [SerializeField] private settingsScript _settingsScript = null;

    [Header("Main menu."), SerializeField] private Text noticeText = null;

    [Header("Settings menu."), SerializeField] private Text savingText = null;
    [SerializeField] private Button saveButton = null, loadButton = null;

    private bool hasLoadedProfileListOnStart;
    public const string defaultProfileName = "Default";
    [Header("Profiles menu."), SerializeField] private Text profileNameText = null, newProfileExceptionText = null;
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
        _ = noticeText;

        if ((_universalRenderPipelineAssets != null) && (universalRenderPipelineAssets == null)) {
            universalRenderPipelineAssets = _universalRenderPipelineAssets;
        }
        string savesFolderPath = getPath(true, false, false, false, false, null);
        if (Directory.Exists(savesFolderPath) == false) {
            Directory.CreateDirectory(savesFolderPath);
        }
        if (SceneManager.GetActiveScene().name == SceneNames.MainMenu) {
            if (File.Exists(getPath(false, true, false, false, false, null)) == false) {
                makeNewProfile(defaultProfileName);
            } else if (hasLoadedProfileListOnStart == false) {
                loadProfiles();
                selectProfile(LoadedPlayerData.profileList.lastlySelectedProfileName);
                hasLoadedProfileListOnStart = true;
            } else {
                selectProfile(LoadedPlayerData.profileList.lastlySelectedProfileName);
            }
        }
        GC.Collect();
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
        ProfileList.profiles.Add(newPlayerData);
        Dropdown.OptionData optionData = new Dropdown.OptionData {
            text = newPlayerData.name
        };
        profilesDropdown.options.Add(optionData);
        LoadedPlayerData.playerData = newPlayerData;
        save(newPlayerData.name);
        selectProfile(newPlayerData.name);
        Debug.Log("Added a new profile with the name : " + newPlayerData.name + ".");
        return;
    }

    public void deleteProfile() {
        try {
            string name = ProfileList.profiles[profilesDropdown.value].name;
            File.Delete(getPath(false, false, true, false, false, name));
            File.Delete(getPath(false, false, false, true, false, name));
            File.Delete(getPath(false, false, false, false, true, name));
        } catch (Exception exception) {
            catchException(exception);
        }
        ProfileList.profiles[profilesDropdown.value] = null;
        List<PlayerData> newProfileList = new List<PlayerData>();
        foreach (PlayerData playerData in ProfileList.profiles) {
            if (playerData != null) {
                newProfileList.Add(playerData);
            }
        }
        ProfileList.profiles = newProfileList;
        saveProfiles();
        loadProfiles();
        return;
    }

    public void selectProfile() {
        selectProfile(ProfileList.profiles[profilesDropdown.value].name);
        return;
    }

    private void selectProfile(string profileName) {
        int index = ScanProfileList(profileName);
        if (index != -1) {
            LoadedPlayerData.playerData = ProfileList.profiles[index];
            LoadedPlayerData.profileList.lastlySelectedProfileName = LoadedPlayerData.playerData.name;
            profilesDropdown.value = index;
            profileNameText.text = (LoadedPlayerData.playerData.name + ".");
            load();
            saveProfiles();
        }
        deleteButton.interactable = (ProfileList.profiles.Count > 1);
        return;
    }

    //Returns the index of the element when found, returns -1 when it did not.
    private static int ScanProfileList(string profileName) {
        for (int i = 0; i < ProfileList.profiles.Count; i++) {
            if (ProfileList.profiles[i].name == profileName) {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region Profile lists.
    public void saveProfiles() {
        LoadedPlayerData.profileList.profileNames.Clear();
        for (int i = 0; i < ProfileList.profiles.Count; i++) {
            LoadedPlayerData.profileList.profileNames.Add(ProfileList.profiles[i].name);
        }
        try {
            File.WriteAllText(getPath(false, true, false, false, false, null), CleanJson(JsonUtility.ToJson(LoadedPlayerData.profileList, true)));
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    private void loadProfiles() {
        try {
            string path = getPath(false, true, false, false, false, null);
            if (checkIfFileExists(path) == true) {
                LoadedPlayerData.profileList = JsonUtility.FromJson<ProfileList>(File.ReadAllText(path));
                //Remove invalid profile names.
                List<string> temp = new List<string>();
                foreach (string name in LoadedPlayerData.profileList.profileNames) {
                    if (checkName(name, false) == true) {
                        temp.Add(name);
                    }
                }
                LoadedPlayerData.profileList.profileNames = temp;
                if (LoadedPlayerData.profileList.profileNames.Contains(LoadedPlayerData.profileList.lastlySelectedProfileName) == false) {
                    LoadedPlayerData.profileList.lastlySelectedProfileName = defaultProfileName;
                }

                profilesDropdown.ClearOptions();
                profilesDropdown.AddOptions(LoadedPlayerData.profileList.profileNames);

                ProfileList.profiles.Clear();
                for (int i = 0; i < LoadedPlayerData.profileList.profileNames.Count; i++) {
                    PlayerData loadedPlayerData = load(LoadedPlayerData.profileList.profileNames[i], false);
                    ProfileList.profiles.Add(loadedPlayerData);
                }
                //selectProfile(LoadedPlayerData.profileList.lastlySelectedProfileName);
            }
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
            FileStream fileStream = File.Create(getPath(false, false, false, false, false, profileName));
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
        string path = getPath(false, false, false, false, false, profileName);
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
        try {
            File.WriteAllText(getPath(false, false, true, false, false, profileName), CleanJson(JsonConvert.SerializeObject(LoadedPlayerData.playerGraphics, Formatting.Indented)));
            File.WriteAllText(getPath(false, false, false, true, false, profileName), CleanJson(JsonUtility.ToJson(QualitySettings.renderPipeline, true)));
            File.WriteAllText(getPath(false, false, false, false, true, profileName), CleanJson(JsonConvert.SerializeObject(LoadedPlayerData.playerQualitySettings, Formatting.Indented)));
        } catch (Exception exception) {
            catchException(exception);
        }
        return;
    }

    public void loadGraphicsSettings(string profileName) {
        try {
            string path = getPath(false, false, true, false, false, profileName);
            if (File.Exists(path) == true) {
                LoadedPlayerData.playerGraphics = JsonConvert.DeserializeObject<PlayerGraphics>(File.ReadAllText(path));
            }
            path = getPath(false, false, false, true, false, profileName);
            if (File.Exists(path) == true) {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), QualitySettings.renderPipeline);
            }
            path = getPath(false, false, false, false, true, profileName);
            if (File.Exists(path) == true) {
                LoadedPlayerData.playerQualitySettings = JsonConvert.DeserializeObject<PlayerQualitySettings>(File.ReadAllText(path));
            }
        } catch (Exception exception) {
            catchException(exception);
        }
        saveGraphicsSettings(profileName);
        return;
    }
    #endregion

    private string getPath(bool getSavesFolder, bool getProfileList, bool getGraphicsSettings, bool getURP, bool getQualitySettings, string playerName) {
#if UNITY_EDITOR
        string dataPath = (Application.dataPath + "/Saves/");
#else
            string dataPath = (Application.persistentDataPath + "/Saves/");
#endif
        if (getSavesFolder == true) {
            return dataPath;
        } else if (getProfileList == true) {
            return (dataPath + "Profiles.json");
        } else if (getGraphicsSettings == true) {
            return (dataPath + playerName + "Graphics.json");
        } else if (getURP == true) {
            return (dataPath + playerName + "URP.json");
        } else if (getQualitySettings == true) {
            return (dataPath + playerName + "QualitySettings.json");
        }
        return (dataPath + playerName + ".RS");
    }

    private static string CleanJson(string json) {
        if (json.Contains("\r\n") == false) {
            json = json.Replace("\n", "\r\n");
        }
        json = json.Replace(": ", " : ");
        json = json.TrimEnd('\r', '\n');
        return json;
    }

    #region Name checking.
    public void checkNameNoReturn(InputField inputField) {
        checkName(inputField);
        return;
    }

    private bool checkName(InputField inputField) {
        return checkName(inputField.text, true);
    }

    private bool checkName(string inputText, bool checkDuplicate) {
        newProfileExceptionText.color = Color.red;
        if (inputText.Length == 0) {
            newProfileExceptionText.text = "Profile name can not be empty.";
            newProfileExceptionText.gameObject.SetActive(true);
            return false;
        }
        if (checkDuplicate == true) {
            foreach (PlayerData playerData in ProfileList.profiles) {
                if (playerData.name == inputText) {
                    newProfileExceptionText.text = "Duplicate profile name found.";
                    newProfileExceptionText.gameObject.SetActive(true);
                    return false;
                }
            }
        }
        const string bannedCharacrers = "\\\"*|<>:? ";
        for (int i = 0; i < inputText.Length; i++) {
            if (bannedCharacrers.Contains(inputText[i]) == true) {
                newProfileExceptionText.text = "Profile name can not contain \"" + inputText[i] + "\".";
                newProfileExceptionText.gameObject.SetActive(true);
                return false;
            }

            /*
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
            */
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void catchException(Exception exception) {
        savingText.gameObject.SetActive(true);
        savingText.color = Color.red;
        savingText.text = exception.Message;
        Debug.LogError(exception);
        return;
    }
}