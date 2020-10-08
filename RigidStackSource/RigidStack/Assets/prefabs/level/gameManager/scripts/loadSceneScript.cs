using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames {
    //Variables for managing scenes.
    public static readonly string preMainMenu = "preMainMenu",
                                  MainMenu = "mainMenu",
                                  GameplaySettingsMenu = "gameplaySettingsMenu",
                                  GraphicsSettingsMenu = "graphicsSettingsMenu",
                                  Level = "level";
}

public class loadSceneScript : MonoBehaviour {
    public static bool isMultiplayer = false;

    public void loadScene(string sceneName) {
        Time.timeScale = 1f;
        heightScript _heightScript = FindObjectOfType<heightScript>();
        if (_heightScript != null) {
            _heightScript.currentGameMaxHeight = 0;
        }
        SceneManager.LoadScene(sceneName);
        return;
    }

    public void loadLevel() {
        isMultiplayer = false;
        loadScene(SceneNames.Level);
        return;
    }

    public void loadMultiplayerLevel() {
        isMultiplayer = true;
        loadScene(SceneNames.Level);
        return;
    }
}