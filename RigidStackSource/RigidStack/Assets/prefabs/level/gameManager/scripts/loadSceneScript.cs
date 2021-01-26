using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames {
    public const string MainMenu = "mainMenu",
                        Level = "level";
}

public class loadSceneScript : MonoBehaviour {
    public static void loadScene(string sceneName) {
        if (sceneName != SceneNames.Level) {
            Destroy(NetworkManager.singleton.gameObject);
        }
        Time.timeScale = 1f;
        heightScript _heightScript = FindObjectOfType<heightScript>();
        if (_heightScript != null) {
            _heightScript.currentGameMaxHeight = 0;
        }
        if (NetworkManagerScript.isSingleplayerGame == true) {
            NetworkManager.singleton.StopHost();
        } else if (NetworkManagerScript.isMultiplayerGame == true) {
            NetworkManager.singleton.ServerChangeScene(sceneName);
        }
        SceneManager.LoadScene(sceneName);
        return;
    }

    public void loadLevel() {
        NetworkManagerScript.isSingleplayerGame = true;
        NetworkManagerScript.isMultiplayerGame = false;
        loadScene(SceneNames.Level);
        return;
    }
}