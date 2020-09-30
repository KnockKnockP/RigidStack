#region A using tag.
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "pauseScript" class.
public class pauseScript : MonoBehaviour {
    #region Variables.
    #region Variables for pausing the game.
    private bool isPaused;
    [SerializeField] private GameObject pauseMenuPanel = null;
    #endregion

    #region A variable for enabling and diabling the end menu.
    [SerializeField] private endMenuManager _endMenuManager = null;
    #endregion
    #endregion

    #region Pausing or resuming the game.
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
    #endregion

    #region Going back to the main menu.
    public void toMainMenu() {
        FindObjectOfType<loadSceneScript>().loadScene(SceneNames.MainMenu);
        return;
    }
    #endregion

    #region Exiting the game.
    public void exit() {
        FindObjectOfType<savingScript>().save();
        Application.Quit();
        Debug.Log("Exited the game.");
        return;
    }
    #endregion
}
#endregion