using UnityEngine;

public class pauseScript : MonoBehaviour {
    private bool isPaused;
    [SerializeField] private GameObject pauseMenuPanel = null;


    [SerializeField] private endMenuManager _endMenuManager = null;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            pauseOrResume();
        }
        return;
    }

    public void pauseOrResume() {
        if (endMenuManager.isGameEnded == false) {
            pauseMenuPanel.SetActive(!isPaused);
            isPaused = (!isPaused);
        } else {
            _endMenuManager.toggleEndMenu();
        }
        return;
    }

    public void toMainMenu() {
        FindObjectOfType<loadSceneScript>().loadScene(SceneNames.MainMenu);
        return;
    }

    public void exit() {
        //SAVE IMPLEMENTATION.
        Debug.LogWarning("Call the save function here.\r\n" +
                         "Also, put the core of the exit function into the save manager script when we make it.");
        Application.Quit();
        return;
    }
}