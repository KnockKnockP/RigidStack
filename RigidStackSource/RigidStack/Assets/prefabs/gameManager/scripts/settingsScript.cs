using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Difficulty : byte {
    Sandbox = 0,
    Easy = 1,
    Moderate = 2,
    Difficult = 3,
    Extreme = 4
};

public static class GraphicsLevel {
    private static readonly string[] graphicsLevels = {
        "Potato.",
        "Low.",
        "Medium.",
        "High.",
        "Very high.",
    };

    public static string getGraphicsLevelString(int graphicsLevel) {
        return graphicsLevels[graphicsLevel];
    }
}

public class settingsScript : MonoBehaviour {
    [Header("Gameplay settings.")]
    [SerializeField] private Text difficultyText = null;
    [SerializeField] private Dropdown difficultyDropdown = null;


    [SerializeField] private Text manualCheckingText = null;


    [Header("Graphics settings.")]
    [SerializeField] private Text graphicsText = null;
    [SerializeField] private Dropdown graphicsDropdown = null;


    [SerializeField] private Text verticalSyncCountText = null;
    [SerializeField] private Dropdown verticalSyncCountDropdown = null;


    [SerializeField] private Text backgroundEnabledText = null;


    [SerializeField] private Text backgroundScalingText = null;

    private void Start() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == SceneNames.GameplaySettingsMenu) {
            updateDifficulty(LoadedPlayerData.playerData.difficulty);
            updateManualChecking(LoadedPlayerData.playerData.isManualCheckingEnabled);
        } else if (sceneName == SceneNames.GraphicsSettingsMenu) {
            updateGraphics(LoadedPlayerData.playerGraphics.graphics);
            updateVerticalSyncCount(LoadedPlayerData.playerGraphics.verticalSyncCount);
            updateBackgroundEnabled(LoadedPlayerData.playerGraphics.isBackgroundEnabled);
            updateBackgroundScaling(LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
        }
        return;
    }

    public void changeDifficulty() {
        updateDifficulty((Difficulty)(difficultyDropdown.value));
        return;
    }

    public void updateDifficulty(Difficulty _difficulty) {
        LoadedPlayerData.playerData.difficulty = _difficulty;
        difficultyText.text = ("Difficulty : " + _difficulty + ".");
        difficultyDropdown.value = (int)(_difficulty);
        Debug.Log("Updated difficulty to " + _difficulty + ".");
        return;
    }

    public void changeGraphics() {
        updateGraphics(graphicsDropdown.value);
        return;
    }

    public void updateGraphics(int graphicsLevel) {
        QualitySettings.SetQualityLevel(graphicsLevel, true);
        LoadedPlayerData.playerGraphics.graphics = graphicsLevel;
        string graphicsLevelString = GraphicsLevel.getGraphicsLevelString(graphicsLevel);
        graphicsText.text = ("Graphics : " + graphicsLevelString);
        graphicsDropdown.value = graphicsLevel;
        Debug.Log("Updated graphics to " + graphicsLevelString);
        return;
    }

    public void changeVerticalSyncCount() {
        updateVerticalSyncCount(verticalSyncCountDropdown.value);
        return;
    }

    public void updateVerticalSyncCount(int _verticalSyncCount) {
        QualitySettings.vSyncCount = _verticalSyncCount;
        LoadedPlayerData.playerGraphics.verticalSyncCount = _verticalSyncCount;
        verticalSyncCountText.text = ("Vertical sync count : " + _verticalSyncCount + ".");
        verticalSyncCountDropdown.value = _verticalSyncCount;
        Debug.Log("Updated vertical sync count to " + _verticalSyncCount + ".");
        return;
    }

    public void toggleManualChecking() {
        updateManualChecking(!LoadedPlayerData.playerData.isManualCheckingEnabled);
        return;
    }

    public void updateManualChecking(bool _isManualCheckingEnabled) {
        LoadedPlayerData.playerData.isManualCheckingEnabled = _isManualCheckingEnabled;
        manualCheckingText.text = ("Is manual height checking enabled : " + _isManualCheckingEnabled + ".");
        Debug.Log("Updated is manual height checking enabled to " + _isManualCheckingEnabled + ".");
        return;
    }

    public void toggleBackgroundEnabled() {
        updateBackgroundEnabled(!LoadedPlayerData.playerGraphics.isBackgroundEnabled);
        return;
    }

    public void updateBackgroundEnabled(bool _isBackgroundEnabled) {
        LoadedPlayerData.playerGraphics.isBackgroundEnabled = _isBackgroundEnabled;
        backgroundEnabledText.text = ("Is background enabled : " + _isBackgroundEnabled + ".");
        Debug.Log("Updated is background enabled to " + _isBackgroundEnabled + ".");
        return;
    }

    public void toggleBackgroundScaling() {
        updateBackgroundScaling(!LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
        return;
    }

    public void updateBackgroundScaling(bool _isBackgroundScalingStretch) {
        LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio = _isBackgroundScalingStretch;
        string temp = "Stretch";
        if (_isBackgroundScalingStretch == true) {
            temp = "Keep aspect ratio";
        }
        backgroundScalingText.text = ("Background scaling : " + temp + ".");
        Debug.Log("Updated background scaling to " + temp + ".");
        return;
    }
}