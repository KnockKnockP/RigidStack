#region Using tags.
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

#region Static "SceneNames" class.
public static class SceneNames {
    #region Variables for managing scenes.
    public static readonly string preMainMenu = "preMainMenu",
                                  MainMenu = "mainMenu",
                                  GameplaySettingsMenu = "gameplaySettingsMenu",
                                  GraphicsSettingsMenu = "graphicsSettingsMenu",
                                  Level = "level";
    #endregion
}
#endregion

#region "MonoBehaviour" inherited "loadSceneScript" class.
public class loadSceneScript : MonoBehaviour {
    #region Loading a scene.
    public void loadScene(string sceneName) {
        Time.timeScale = 1f;
        heightScript.currentGameMaxHeight = 0;
        SceneManager.LoadScene(sceneName);
        return;
    }
    #endregion

    #region Loading a level.
    public void loadLevel() {
        loadScene(SceneNames.Level);
        return;
    }
    #endregion
}
#endregion