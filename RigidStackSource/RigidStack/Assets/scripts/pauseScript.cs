using UnityEngine;

public class pauseScript : MonoBehaviour {
    //Pause menu.
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
        Debug.LogWarning("Call the save function here.\r\n" +
                         "Also, put the core of the exit function into the save manager script when we make it.");
        Application.Quit();
        return;
    }
}