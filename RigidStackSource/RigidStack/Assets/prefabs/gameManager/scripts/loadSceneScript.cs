using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames {
    public static readonly string preMainMenu = "preMainMenu",
                                  MainMenu = "mainMenu",
                                  GameplaySettingsMenu = "gameplaySettingsMenu",
                                  GraphicsSettingsMenu = "graphicsSettingsMenu",
                                  Level = "level";
}

public class loadSceneScript : MonoBehaviour {
    public void loadScene(string sceneName) {
        Time.timeScale = 1f;
        heightScript.currentGameMaxHeight = 0;
        SceneManager.LoadScene(sceneName);
        return;
    }

    public void loadLevel() {
        loadScene(SceneNames.Level);
        return;
    }
}