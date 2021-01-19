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
        if (isServer == true) {
            clientRPCStopConnection(true, true, SceneNames.MainMenu, "Server stopped the multiplayer lobby.");
        } else {
            stopConnection(true, true, SceneNames.MainMenu, null);
        }
        return;
    }

    public void exit() {
        FindObjectOfType<savingScript>().save();
        if (NetworkManager.singleton != null) {
            clientRPCStopConnection(false, false, SceneNames.MainMenu, "Server exited the game.");
        }
        Application.Quit();
        Debug.Log("Exited the game.");
        return;
    }

    [ClientRpc]
    private void clientRPCStopConnection(bool destroy, bool switchServer, string sceneName, string disconnectedReason) {
        stopConnection(destroy, switchServer, sceneName, disconnectedReason);
        return;
    }

    private void stopConnection(bool destroy, bool switchServer, string sceneName = null, string disconnectReason = null) {
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
        if (sceneName != null) {
            if ((isServer == true) && (switchServer == true)) {
                loadSceneScript.loadScene(sceneName);
            } else {
                loadSceneScript.loadScene(sceneName);
            }
        }
        if ((isClientOnly == true) && (disconnectReason != null)) {
            //TODO : Display disconnected reason.
        }
        return;
    }
}