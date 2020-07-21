using UnityEngine;

public class pauseScript : MonoBehaviour {
    private bool isPaused;
    [SerializeField] private GameObject pauseMenuPanel = null;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            pauseOrResume();
        }
        return;
    }

    public void pauseOrResume() {
        pauseMenuPanel.SetActive(!isPaused);
        isPaused = (!isPaused);
        return;
    }

    public void toMainMenu() {
        FindObjectOfType<loadSceneScript>().loadScene(SceneNames.MainMenu);
        return;
    }

    public void exit() {
        Debug.LogWarning("Call the save function here.");
        Application.Quit();
        return;
    }
}