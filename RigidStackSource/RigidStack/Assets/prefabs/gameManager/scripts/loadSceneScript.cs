using UnityEngine;
using UnityEngine.SceneManagement;

public class loadSceneScript : MonoBehaviour {
    public void loadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
        return;
    }
}