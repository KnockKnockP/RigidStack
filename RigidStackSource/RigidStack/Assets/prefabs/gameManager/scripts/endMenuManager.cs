using UnityEngine;
using UnityEngine.UI;

public class endMenuManager : MonoBehaviour {
    private bool shouldSlowDownTime;
    [HideInInspector] public bool shouldMoveTheCamera;
    private float startTime, originalYPosition;
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;
    [SerializeField] private cameraScript _cameraScript = null;


    private bool isEndMenuActive;
    [SerializeField] private GameObject endMenu = null;
    [SerializeField] private Text endMenuScoreText = null;
    [SerializeField] private objectiveScript _objectiveScript = null;


    [SerializeField] private GameObject dock = null, objectEditingPanel = null;


    public static bool isGameEnded;

    private void Start() {
        startTime = Time.time;
    }

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

    public void endGame() {
        isGameEnded = true;
        endMenuScoreText.text = ("Game over!\r\n" +
                                 "Score : " + heightScript.currentGameMaxHeight + " / " + heightScript.maxHeight + ".");
        enableEndMenu();
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
        disableEndMenu();
        return;
    }

    public void toggleEndMenu() {
        if (isEndMenuActive == false) {
            enableEndMenu();
        } else {
            disableEndMenu();
        }
        return;
    }

    private void enableEndMenu() {
        isEndMenuActive = true;
        endMenu.SetActive(true);
        return;
    }

    private void disableEndMenu() {
        isEndMenuActive = false;
        endMenu.SetActive(false);
        return;
    }

    private void moveCamera() {
        originalYPosition = _sharedMonobehaviour.mainCamera.transform.position.y;
        shouldMoveTheCamera = true;
        return;
    }

    private void slowMotion() {
        shouldSlowDownTime = true;
        return;
    }

    public void restart() {
        _objectiveScript.objectiveScore = 0;
        heightScript.currentGameMaxHeight = 0;
        //SAVE IMPLEMENTATION.
        FindObjectOfType<loadSceneScript>().loadLevel();
        return;
    }
}