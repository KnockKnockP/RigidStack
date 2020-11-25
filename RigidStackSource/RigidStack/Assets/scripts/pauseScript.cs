using Mirror;
using System;
using UnityEngine;

public class pauseScript : NetworkBehaviour {
    private bool isPaused;
    [SerializeField] private GameObject pauseMenuPanel = null;
    [SerializeField] private endMenuManager _endMenuManager = null;

    public void pauseOrResume() {
        if ((endMenuManager.isGameEnded == false) && (NetworkManagerScript.isMultiplayerGame == false)) {
            Time.timeScale = Convert.ToInt32(isPaused);
            isPaused = !isPaused;
            pauseMenuPanel.SetActive(isPaused);
        } else {
            _endMenuManager.toggleEndMenu();
        }
        return;
    }

    public void toMainMenu() {
        loadSceneScript.loadScene(SceneNames.MainMenu);
        stopConnection(true);
        return;
    }

    public void exit() {
        FindObjectOfType<savingScript>().save();
        if (NetworkManager.singleton != null) {
            stopConnection(false);
        }
        Application.Quit();
        Debug.Log("Exited the game.");
        return;
    }

    private void stopConnection(bool destroy) {
        if (NetworkManager.singleton.isNetworkActive == true) {
            if (isServer == true) {
                NetworkManager.singleton.StopHost();
            } else if (isClientOnly == true) {
                NetworkManager.singleton.StopClient();
            }
            if (destroy == true) {
                Destroy(NetworkManager.singleton.gameObject);
            }
        }
        return;
    }
}