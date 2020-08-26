using UnityEngine;
using UnityEngine.UI;

public enum Difficulty : byte {
    Easy = 0,
    Moderate = 1,
    Difficult = 2,
    Extreme = 3
};

public static class PlayerSettings {
    public static bool isBackgroundEnabled = true;
    public static int verticalSyncCount = 0;
    public static Difficulty difficulty = Difficulty.Easy;
}

public class settingsScript : MonoBehaviour {
    [SerializeField] private Text difficultyText = null;
    [SerializeField] private Dropdown difficultyDropdown = null;


    [SerializeField] private Text verticalSyncCountText = null;
    [SerializeField] private Dropdown verticalSyncCountDropdown = null;


    [SerializeField] private Text backgroundEnabledText = null;

    public void changeDifficulty() {
        updateDifficulty((Difficulty)(difficultyDropdown.value));
        return;
    }

    public void updateDifficulty(Difficulty _difficulty) {
        PlayerSettings.difficulty = _difficulty;
        difficultyText.text = ("Difficulty : " + PlayerSettings.difficulty + ".");
        difficultyDropdown.value = (int)(PlayerSettings.difficulty);
        Debug.Log("Updated difficulty to " + PlayerSettings.difficulty + ".");
        return;
    }

    public void changeVerticalSyncCount() {
        updateVerticalSyncCount(verticalSyncCountDropdown.value);
        return;
    }

    public void updateVerticalSyncCount(int _verticalSyncCount) {
        QualitySettings.vSyncCount = _verticalSyncCount;
        PlayerSettings.verticalSyncCount = QualitySettings.vSyncCount;
        verticalSyncCountText.text = ("Vertical sync count : " + QualitySettings.vSyncCount + ".");
        verticalSyncCountDropdown.value = QualitySettings.vSyncCount;
        Debug.Log("Updated vertical sync count to " + QualitySettings.vSyncCount + ".");
        return;
    }

    public void toggleBackgroundEnabled() {
        updateBackgroundEnabled(!PlayerSettings.isBackgroundEnabled);
        return;
    }

    public void updateBackgroundEnabled(bool _isBackgroundEnabled) {
        PlayerSettings.isBackgroundEnabled = _isBackgroundEnabled;
        backgroundEnabledText.text = ("Is background enabled : " + PlayerSettings.isBackgroundEnabled + ".");
        Debug.Log("Updated is background enabled to " + PlayerSettings.isBackgroundEnabled + ".");
        return;
    }
}