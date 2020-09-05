#region Using tags.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#endregion

#region "MonoBehaviour" inherited "endMenuManager" class.
public class endMenuManager : MonoBehaviour {
    #region Variables.
    #region Variables for slowing down time.
    private bool shouldSlowDownTime;
    [HideInInspector] public bool shouldMoveTheCamera;
    private float startTime, originalYPosition;
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;
    [SerializeField] private cameraScript _cameraScript = null;
    #endregion

    #region Variables for the end menu.
    [SerializeField] private Text endMenuScoreText = null;
    [SerializeField] private objectiveScript _objectiveScript = null;
    [SerializeField] private GameObject endMenu = null;
    #endregion

    #region Variables for gameplay panels.
    [SerializeField] private GameObject dock = null, objectEditingPanel = null;
    #endregion

    #region A variable for determining if the game has ended.
    public static bool isGameEnded;
    #endregion

    #region Variables for dimming and undimming objects
    private bool isObjectUndimmed;
    [HideInInspector] public List<SpriteRenderer> allPlacedObjectsSpriteRenderers = new List<SpriteRenderer>();
    #endregion
    #endregion

    #region Start function.
    private void Start() {
        startTime = Time.time;
    }
    #endregion

    #region Update function.
    private void Update() {
        if (shouldMoveTheCamera == true) {
            float time = ((Time.time - startTime) / Mathf.Abs(originalYPosition * 10));
            Vector3 newPosition = new Vector3(Mathf.SmoothStep(_sharedMonobehaviour.mainCamera.transform.position.x, _cameraScript.originalCameraPosition.x, time), Mathf.SmoothStep(_sharedMonobehaviour.mainCamera.transform.position.y, _cameraScript.originalCameraPosition.y, time), _sharedMonobehaviour.mainCamera.transform.position.z);
            _sharedMonobehaviour.mainCamera.transform.position = newPosition;
            if (((_sharedMonobehaviour.mainCamera.transform.position.x - _cameraScript.originalCameraPosition.x) < 0.01f) && ((_sharedMonobehaviour.mainCamera.transform.position.y - _cameraScript.originalCameraPosition.y) < 0.01f)) {
                shouldMoveTheCamera = false;
            }
        }
        if (shouldSlowDownTime == true) {
            float time = ((Time.time - startTime) / 300f);
            Time.timeScale = Mathf.SmoothStep(Time.timeScale, 0f, time);
            if (Time.timeScale < 0.001f) {
                Time.timeScale = 0f;
                shouldSlowDownTime = false;
            }
        }
        return;
    }
    #endregion

    #region Ending the game.
    public void endGame() {
        isGameEnded = true;
        endMenuScoreText.text = ("Game over!\r\n" +
                                 "Score : " + heightScript.currentGameMaxHeight + " / " + LoadedPlayerData.playerData.maxHeight + ".");
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
    #endregion

    #region Exploring the game world.
    public void explore() {
        enableOrDisableEndMenu(false);
        if (isObjectUndimmed == false) {
            undimObjects();
        }
        return;
    }
    #endregion

    #region Toggling the end menu.
    public void toggleEndMenu() {
        enableOrDisableEndMenu(!endMenu.activeSelf);
        return;
    }

    private void enableOrDisableEndMenu(bool status) {
        endMenu.SetActive(status);
    }
    #endregion

    #region Undimming objects.
    private void undimObjects() {
        isObjectUndimmed = true;
        foreach (SpriteRenderer spriteRenderer in allPlacedObjectsSpriteRenderers) {
            spriteRenderer.color = new Color32(255, 255, 255, 255);
        }
        return;
    }
    #endregion

    #region Moving the camera.
    private void moveCamera() {
        originalYPosition = _sharedMonobehaviour.mainCamera.transform.position.y;
        shouldMoveTheCamera = true;
        return;
    }
    #endregion

    #region Activating the slow motion effect.
    private void slowMotion() {
        shouldSlowDownTime = true;
        return;
    }
    #endregion

    #region Restarting the game.
    public void restart() {
        _objectiveScript.objectiveScore = 0;
        heightScript.currentGameMaxHeight = 0;
        FindObjectOfType<savingScript>().save();
        FindObjectOfType<loadSceneScript>().loadLevel();
        return;
    }
    #endregion
}
#endregion