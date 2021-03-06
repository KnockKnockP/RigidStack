﻿using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endMenuManager : NetworkBehaviour {
    private const string _resume = "Resume.", _explore = "Explore.";

    [Header("Variables for slowing down time.")]
    private bool shouldSlowDownTime;
    [NonSerialized] public bool shouldMoveTheCamera;
    [SyncVar(hook = nameof(syncTimeScale))] private float timeScale = 1f;
    [SerializeField] private cameraScript _cameraScript = null;

    [Header("Variables for the end menu."), SerializeField] private Text endMenuScoreText = null;
    [SerializeField] private objectiveScript _objectiveScript = null;
    [SerializeField] private heightScript _heightScript = null;
    [SerializeField] private Text resumeButtonText = null, gameOverText = null;
    [SerializeField] private Button kickButton = null;
    [SerializeField] private GameObject endMenu = null;

    [Header("Variables for gameplay panels.")]
    [SerializeField] private GameObject dock = null, objectEditingPanel = null;

    private bool isPaused;
    public static bool isGameEnded;

    private bool isObjectUndimmed;
    [NonSerialized] public List<objectInformation> objectInformations = new List<objectInformation>();

    private void Awake() {
        kickButton?.gameObject.SetActive(NetworkManagerScript.isMultiplayerGame || Debug.isDebugBuild);
    }

    private void Update() {
        if (shouldMoveTheCamera == true) {
            Vector3 newPosition = sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position;
            newPosition.y = Mathf.SmoothStep(newPosition.y, _cameraScript.originalCameraPosition.y, Time.fixedUnscaledDeltaTime);
            sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position = newPosition;
            if (((newPosition.x - _cameraScript.originalCameraPosition.x) < 0.01f) && ((newPosition.y - _cameraScript.originalCameraPosition.y) < 0.01f)) {
                shouldMoveTheCamera = false;
            }
        }
        if (shouldSlowDownTime == true) {
            timeScale = Mathf.SmoothStep(timeScale, 0f, Time.unscaledDeltaTime);
            if (timeScale < 0.01f) {
                timeScale = 0f;
                shouldSlowDownTime = false;
            }
        }
        return;
    }

    private void OnDestroy() {
        isGameEnded = false;
        return;
    }

    [Server]
    public void endGame() {
        clientRPCEndGame(_heightScript.currentGameMaxHeight);
        return;
    }

    [ClientRpc]
    private void clientRPCEndGame(int currentGameMaxHeight) {
        isGameEnded = true;
        endMenuScoreText.text = ("Game over!\r\n" +
                                 "Score : " + currentGameMaxHeight + " / " + LoadedPlayerData.playerData.maxHeight + ".");
        resumeButtonText.text = _explore;
        enableOrDisableEndMenu(true);
        dock.SetActive(false);
        if ((dragAndDropScript._dragAndDropScript != null) && (dragAndDropScript._dragAndDropScript.placedGameObject != null)) {
            Destroy(dragAndDropScript._dragAndDropScript.placedGameObject);
        }
        objectEditingPanel.SetActive(false);
        shouldMoveTheCamera = true;
        shouldSlowDownTime = true;
        return;
    }

    public void explore() {
        if (resumeButtonText.text == _resume) {
            pauseOrResume();
        } else {
            enableOrDisableEndMenu(false);
            if (isObjectUndimmed == false) {
                undimObjects();
            }
        }
        return;
    }

    #region Toggling the end menu.
    public void toggleEndMenu() {
        enableOrDisableEndMenu(!endMenu.activeSelf);
        return;
    }

    private void enableOrDisableEndMenu(bool status) {
        endMenu.SetActive(status);
    }
    #endregion

    private void undimObjects() {
        isObjectUndimmed = true;
        foreach (objectInformation _objectInformation in objectInformations) {
            if (_objectInformation.unDimDelegate == null) {
                _objectInformation.unDimDelegate = _objectInformation.UnDim;
            }
            _objectInformation.unDimDelegate();
        }
        return;
    }

    private void syncTimeScale(float oldTimeScale, float newTimeScale) {
        Time.timeScale = newTimeScale;
        return;
    }

    public void pauseOrResume() {
        if (isGameEnded == false) {
            if (NetworkManagerScript.isSingleplayerGame == true) {
                Time.timeScale = Convert.ToInt32(isPaused);
            }
            isPaused = !isPaused;
            resumeButtonText.text = _resume;
            gameOverText.text = "Paused.";
            endMenu.SetActive(isPaused);
        } else {
            resumeButtonText.text = _explore;
            toggleEndMenu();
        }
        return;
    }

    public void exit() {
        FindObjectOfType<savingScript>().save();
        if ((NetworkManagerScript.isMultiplayerGame == true) && (NetworkManager.singleton != null) && (NetworkManager.singleton.isNetworkActive == true)) {
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

    public void toMainMenu() {
        FindObjectOfType<savingScript>().save();
        if (isServer == true) {
            clientRPCStopConnection(true, true, SceneNames.MainMenu, "Server stopped the multiplayer lobby.");
        } else {
            stopConnection(true, true, SceneNames.MainMenu, null);
        }
        return;
    }

    #region Restarting the level.
    public void restart() {
        if (isServer == true) {
            commandRestart();
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandRestart() {
        if (NetworkManagerScript.isMultiplayerGame == false) {
            FindObjectOfType<savingScript>().save();
        } else {
            _objectiveScript.objectiveScore = 0;
            _heightScript.currentGameMaxHeight = 0;
        }
        loadSceneScript.loadScene(SceneNames.Level);
        return;
    }
    #endregion

    public void kick() {
        pauseOrResume();
        return;
    }
}