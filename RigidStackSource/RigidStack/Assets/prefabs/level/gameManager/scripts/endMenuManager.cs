﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endMenuManager : MonoBehaviour {
    //Variables for slowing down time.
    private bool shouldSlowDownTime;
    [HideInInspector] public bool shouldMoveTheCamera;
    [SerializeField] private cameraScript _cameraScript = null;

    //Variables for the end menu.
    [SerializeField] private Text endMenuScoreText = null;
    [SerializeField] private objectiveScript _objectiveScript = null;
    private heightScript _heightScript;
    [SerializeField] private GameObject endMenu = null;

    //Variables for gameplay panels.
    [SerializeField] private GameObject dock = null, objectEditingPanel = null;

    //A variable for determining if the game has ended.
    public static bool isGameEnded;

    //Variables for dimming and undimming objects
    private bool isObjectUndimmed;
    [HideInInspector] public List<SpriteRenderer> allPlacedObjectsSpriteRenderers = new List<SpriteRenderer>();

    private void Start() {
        _heightScript = FindObjectOfType<heightScript>();
    }

    private void Update() {
        if (shouldMoveTheCamera == true) {
            Vector3 newPosition = sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position;
            newPosition.y = Mathf.SmoothStep(newPosition.y, _cameraScript.originalCameraPosition.y, Time.fixedUnscaledDeltaTime);
            sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position = newPosition;
            if (((sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position.x - _cameraScript.originalCameraPosition.x) < 0.01f) && ((sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position.y - _cameraScript.originalCameraPosition.y) < 0.01f)) {
                shouldMoveTheCamera = false;
            }
        }
        if (shouldSlowDownTime == true) {
            Time.timeScale = Mathf.SmoothStep(Time.timeScale, 0f, Time.unscaledDeltaTime);
            if (Time.timeScale < 0.001f) {
                Time.timeScale = 0f;
                shouldSlowDownTime = false;
            }
        }
        return;
    }

    private void OnDestroy() {
        isGameEnded = false;
        return;
    }

    public void endGame() {
        isGameEnded = true;
        endMenuScoreText.text = ("Game over!\r\n" +
                                 "Score : " + _heightScript.currentGameMaxHeight + " / " + LoadedPlayerData.playerData.maxHeight + ".");
        enableOrDisableEndMenu(true);
        dock.SetActive(false);
        if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
            Destroy(dragAndDropScript._dragAndDropScript.placedGameObject);
        }
        objectEditingPanel.SetActive(false);
        moveCamera();
        slowMotion();
        return;
    }

    public void explore() {
        enableOrDisableEndMenu(false);
        if (isObjectUndimmed == false) {
            undimObjects();
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
        foreach (SpriteRenderer spriteRenderer in allPlacedObjectsSpriteRenderers) {
            spriteRenderer.color = new Color32(255, 255, 255, 255);
        }
        return;
    }

    private void moveCamera() {
        shouldMoveTheCamera = true;
        return;
    }

    private void slowMotion() {
        shouldSlowDownTime = true;
        return;
    }

    public void restart() {
        _objectiveScript.objectiveScore = 0;
        _heightScript.currentGameMaxHeight = 0;
        FindObjectOfType<savingScript>().save();
        FindObjectOfType<loadSceneScript>().loadLevel();
        return;
    }
}