using UnityEngine;

public class pauseScript : MonoBehaviour {
    //Variables for pausing the game.
    private bool isPaused;
    [SerializeField] private GameObject pauseMenuPanel = null;

    //A variable for enabling and diabling the end menu.
    [SerializeField] private endMenuManager _endMenuManager = null;

    public void pauseOrResume() {
        if (endMenuManager.isGameEnded == false) {
            if (isPaused == false) {
                Time.timeScale = 0f;
                isPaused = true;
            } else {
                Time.timeScale = 1f;
                isPaused = false;
            }
            pauseMenuPanel.SetActive(isPaused);
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
        FindObjectOfType<savingScript>().save();
        Application.Quit();
        Debug.Log("Exited the game.");
        return;
    }
}