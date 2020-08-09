using UnityEngine;

public class inputManager : MonoBehaviour {
    [SerializeField] private pauseScript _pauseScript = null;
    [SerializeField] private cameraScript _cameraScript = null;

    private void Update() {
        manageCameraMovement();
        if ((Input.GetKeyDown(KeyCode.Return) == true) || (Input.GetKeyDown(KeyCode.KeypadEnter) == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.placeObject();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.cancelPlacingObject();
            } else {
                _pauseScript.pauseOrResume();
            }
        }
        return;
    }

    private void manageCameraMovement() {
        if ((Input.GetKeyDown(KeyCode.W) == true) || (Input.GetKeyDown(KeyCode.UpArrow) == true)) {
            _cameraScript.toggleCameraUp();
        }
        if ((Input.GetKeyUp(KeyCode.W) == true) || (Input.GetKeyUp(KeyCode.UpArrow) == true)) {
            _cameraScript.toggleCameraUp();
        }
        if ((Input.GetKeyDown(KeyCode.S) == true) || (Input.GetKeyDown(KeyCode.DownArrow) == true)) {
            _cameraScript.toggleCameraDown();
        }
        if ((Input.GetKeyUp(KeyCode.S) == true) || (Input.GetKeyUp(KeyCode.DownArrow) == true)) {
            _cameraScript.toggleCameraDown();
        }
        return;
    }
}