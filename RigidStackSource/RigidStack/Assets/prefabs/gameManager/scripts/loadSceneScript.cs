using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames {
    public static readonly string MainMenu = "mainMenu";
    public static readonly string level = "level";
}

public class loadSceneScript : MonoBehaviour {
    public void loadScene(string sceneName) {
        StaticClass.objectiveScore = 0;
        SceneManager.LoadScene(sceneName);
        return;
    }

    public void loadLevel() {
        loadScene(SceneNames.level);
        return;
    }
}