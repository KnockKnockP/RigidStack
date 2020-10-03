#region Using tags.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#endregion

#region "Difficulty" byte enum.
public enum Difficulty : byte {
    Sandbox = 0,
    Easy = 1,
    Moderate = 2,
    Difficult = 3,
    Extreme = 4
};
#endregion

#region Static "GraphicsLevel" class.
public static class GraphicsLevel {
    #region Variables.
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
    #endregion
}
#endregion

#region "MonoBehaviour" inherited "settingsScript" class.
public class settingsScript : MonoBehaviour {
    #region Variables for setting up the scene.
    [Header("Gameplay settings.")]
    [SerializeField] private Text difficultyText = null;
    [SerializeField] private Dropdown difficultyDropdown = null;


    [SerializeField] private Text manualCheckingText = null;


    [Header("Graphics settings.")]
    [SerializeField] private Text graphicsText = null;
    [SerializeField] private Dropdown graphicsDropdown = null;
    [SerializeField] private savingScript _savingScript = null;


    [SerializeField] private Text verticalSyncCountText = null;
    [SerializeField] private Dropdown verticalSyncCountDropdown = null;


    [SerializeField] private Text backgroundEnabledText = null;


    [SerializeField] private Text backgroundScalingText = null;
    #endregion

    #region Start.
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
    #endregion

    #region Difficulty.
    public void changeDifficulty() {
        updateDifficulty((Difficulty)(difficultyDropdown.value));
        return;
    }

    public void updateDifficulty(Difficulty _difficulty) {
        LoadedPlayerData.playerData.difficulty = _difficulty;
        if ((difficultyText != null) && (difficultyDropdown != null)) {
            difficultyText.text = ("Difficulty : " + _difficulty + ".");
            difficultyDropdown.value = (int)(_difficulty);
        }
        return;
    }
    #endregion

    #region Manual checking
    public void toggleManualChecking() {
        updateManualChecking(!LoadedPlayerData.playerData.isManualCheckingEnabled);
        return;
    }

    public void updateManualChecking(bool _isManualCheckingEnabled) {
        LoadedPlayerData.playerData.isManualCheckingEnabled = _isManualCheckingEnabled;
        if (manualCheckingText != null) {
            manualCheckingText.text = ("Is manual height checking enabled : " + _isManualCheckingEnabled + ".");
        }
        return;
    }
    #endregion

    #region Graphics.
    public void changeGraphics() {
        updateGraphics(graphicsDropdown.value);
        return;
    }

    public void updateGraphics(int graphicsLevel) {
        QualitySettings.SetQualityLevel(graphicsLevel, true);
        LoadedPlayerData.playerGraphics.graphics = graphicsLevel;
        string graphicsLevelString = GraphicsLevel.getGraphicsLevelString(graphicsLevel);
        if ((graphicsText != null) && (graphicsDropdown != null)) {
            graphicsText.text = ("Graphics : " + graphicsLevelString);
            graphicsDropdown.value = graphicsLevel;
        }
        _savingScript.loadGraphicsSettings(LoadedPlayerData.playerData.name);
        _savingScript.save();
        return;
    }
    #endregion

    #region Vertical sync count.
    public void changeVerticalSyncCount() {
        updateVerticalSyncCount(verticalSyncCountDropdown.value);
        return;
    }

    public void updateVerticalSyncCount(int _verticalSyncCount) {
        QualitySettings.vSyncCount = _verticalSyncCount;
        LoadedPlayerData.playerGraphics.verticalSyncCount = _verticalSyncCount;
        if ((verticalSyncCountText != null) && (verticalSyncCountDropdown != null)) {
            verticalSyncCountText.text = ("Vertical sync count : " + _verticalSyncCount + ".");
            verticalSyncCountDropdown.value = _verticalSyncCount;
        }
        return;
    }
    #endregion

    #region Background enabled.
    public void toggleBackgroundEnabled() {
        updateBackgroundEnabled(!LoadedPlayerData.playerGraphics.isBackgroundEnabled);
        return;
    }

    public void updateBackgroundEnabled(bool _isBackgroundEnabled) {
        LoadedPlayerData.playerGraphics.isBackgroundEnabled = _isBackgroundEnabled;
        if (backgroundEnabledText != null) {
            backgroundEnabledText.text = ("Is background enabled : " + _isBackgroundEnabled + ".");
        }
        return;
    }
    #endregion

    #region Background scaling.
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
        if (backgroundScalingText != null) {
            backgroundScalingText.text = ("Background scaling : " + temp + ".");
        }
        return;
    }
    #endregion
}
#endregion