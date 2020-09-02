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

public class settingsScript : MonoBehaviour {
    [Header("Gameplay settings.")]
    [SerializeField] private Text difficultyText = null;
    [SerializeField] private Dropdown difficultyDropdown = null;


    [SerializeField] private Text manualCheckingText = null;


    [Header("Graphics settings.")]
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
            updateVerticalSyncCount(LoadedPlayerData.playerData.verticalSyncCount);
            updateBackgroundEnabled(LoadedPlayerData.playerData.isBackgroundEnabled);
            updateBackgroundScaling(LoadedPlayerData.playerData.isBackgroundScalingKeepAspectRatio);
        }
        return;
    }

    public void changeDifficulty() {
        updateDifficulty((Difficulty)(difficultyDropdown.value));
        return;
    }

    public void updateDifficulty(Difficulty _difficulty) {
        LoadedPlayerData.playerData.difficulty = _difficulty;
        difficultyText.text = ("Difficulty : " + LoadedPlayerData.playerData.difficulty + ".");
        difficultyDropdown.value = (int)(LoadedPlayerData.playerData.difficulty);
        Debug.Log("Updated difficulty to " + LoadedPlayerData.playerData.difficulty + ".");
        return;
    }

    public void changeVerticalSyncCount() {
        updateVerticalSyncCount(verticalSyncCountDropdown.value);
        return;
    }

    public void updateVerticalSyncCount(int _verticalSyncCount) {
        QualitySettings.vSyncCount = _verticalSyncCount;
        LoadedPlayerData.playerData.verticalSyncCount = QualitySettings.vSyncCount;
        verticalSyncCountText.text = ("Vertical sync count : " + QualitySettings.vSyncCount + ".");
        verticalSyncCountDropdown.value = QualitySettings.vSyncCount;
        Debug.Log("Updated vertical sync count to " + QualitySettings.vSyncCount + ".");
        return;
    }

    public void toggleManualChecking() {
        updateManualChecking(!LoadedPlayerData.playerData.isManualCheckingEnabled);
        return;
    }

    public void updateManualChecking(bool _isManualCheckingEnabled) {
        LoadedPlayerData.playerData.isManualCheckingEnabled = _isManualCheckingEnabled;
        manualCheckingText.text = ("Is manual height checking enabled : " + LoadedPlayerData.playerData.isManualCheckingEnabled + ".");
        Debug.Log("Updated is manual height checking enabled to " + LoadedPlayerData.playerData.isManualCheckingEnabled + ".");
        return;
    }

    public void toggleBackgroundEnabled() {
        updateBackgroundEnabled(!LoadedPlayerData.playerData.isBackgroundEnabled);
        return;
    }

    public void updateBackgroundEnabled(bool _isBackgroundEnabled) {
        LoadedPlayerData.playerData.isBackgroundEnabled = _isBackgroundEnabled;
        backgroundEnabledText.text = ("Is background enabled : " + LoadedPlayerData.playerData.isBackgroundEnabled + ".");
        Debug.Log("Updated is background enabled to " + LoadedPlayerData.playerData.isBackgroundEnabled + ".");
        return;
    }

    public void toggleBackgroundScaling() {
        updateBackgroundScaling(!LoadedPlayerData.playerData.isBackgroundScalingKeepAspectRatio);
        return;
    }

    public void updateBackgroundScaling(bool _isBackgroundScalingStretch) {
        LoadedPlayerData.playerData.isBackgroundScalingKeepAspectRatio = _isBackgroundScalingStretch;
        string temp = "Stretch";
        if (LoadedPlayerData.playerData.isBackgroundScalingKeepAspectRatio == true) {
            temp = "Keep aspect ratio";
        }
        backgroundScalingText.text = ("Background scaling : " + temp + ".");
        Debug.Log("Updated background scaling to " + temp + ".");
        return;
    }
}