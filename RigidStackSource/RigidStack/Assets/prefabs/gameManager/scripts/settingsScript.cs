using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Difficulty : byte {
    Easy = 0,
    Moderate = 1,
    Difficult = 2,
    Extreme = 3
};

public class settingsScript : MonoBehaviour {
    [SerializeField] private Text difficultyText = null;
    [SerializeField] private Dropdown difficultyDropdown = null;


    [SerializeField] private Text manualCheckingText = null;


    [SerializeField] private Text verticalSyncCountText = null;
    [SerializeField] private Dropdown verticalSyncCountDropdown = null;


    [SerializeField] private Text backgroundEnabledText = null;

    private void Start() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == SceneNames.GameplaySettingsMenu) {
            updateDifficulty(PlayerData.difficulty);
            updateManualChecking(PlayerData.isManualCheckingEnabled);
        } else if (sceneName == SceneNames.GraphicsSettingsMenu) {
            updateVerticalSyncCount(PlayerData.verticalSyncCount);
            updateBackgroundEnabled(PlayerData.isBackgroundEnabled);
        }
        return;
    }

    public void changeDifficulty() {
        updateDifficulty((Difficulty)(difficultyDropdown.value));
        return;
    }

    public void updateDifficulty(Difficulty _difficulty) {
        PlayerData.difficulty = _difficulty;
        difficultyText.text = ("Difficulty : " + PlayerData.difficulty + ".");
        difficultyDropdown.value = (int)(PlayerData.difficulty);
        Debug.Log("Updated difficulty to " + PlayerData.difficulty + ".");
        return;
    }

    public void changeVerticalSyncCount() {
        updateVerticalSyncCount(verticalSyncCountDropdown.value);
        return;
    }

    public void updateVerticalSyncCount(int _verticalSyncCount) {
        QualitySettings.vSyncCount = _verticalSyncCount;
        PlayerData.verticalSyncCount = QualitySettings.vSyncCount;
        verticalSyncCountText.text = ("Vertical sync count : " + QualitySettings.vSyncCount + ".");
        verticalSyncCountDropdown.value = QualitySettings.vSyncCount;
        Debug.Log("Updated vertical sync count to " + QualitySettings.vSyncCount + ".");
        return;
    }

    public void toggleManualChecking() {
        updateManualChecking(!PlayerData.isManualCheckingEnabled);
        return;
    }

    public void updateManualChecking(bool _isManualCheckingEnabled) {
        PlayerData.isManualCheckingEnabled = _isManualCheckingEnabled;
        manualCheckingText.text = ("Is manual height checking enabled : " + PlayerData.isManualCheckingEnabled + ".");
        Debug.Log("Updated is manual height checking enabled to " + PlayerData.isManualCheckingEnabled + ".");
        return;
    }

    public void toggleBackgroundEnabled() {
        updateBackgroundEnabled(!PlayerData.isBackgroundEnabled);
        return;
    }

    public void updateBackgroundEnabled(bool _isBackgroundEnabled) {
        PlayerData.isBackgroundEnabled = _isBackgroundEnabled;
        backgroundEnabledText.text = ("Is background enabled : " + PlayerData.isBackgroundEnabled + ".");
        Debug.Log("Updated is background enabled to " + PlayerData.isBackgroundEnabled + ".");
        return;
    }
}