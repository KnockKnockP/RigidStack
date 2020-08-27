using UnityEngine;
using UnityEngine.UI;

public class inputManager : MonoBehaviour {
    /*
        W, up arrow key : Move camera up,
        S, down arrow key : Move camera down,
        A, left arrow key : Rotate to left,
        D, right arrow key : Rotate to right,
        Left shift, right shift : Place the object,
        Escape : Cancel object editing, pause,
        Left control, right control : Manually check height.
    */
    [SerializeField] private pauseScript _pauseScript = null;
    [SerializeField] private cameraScript _cameraScript = null;
    [SerializeField] private heightScript _heightScript = null;
    [SerializeField] private Button confirmButton = null;

    private void Update() {
        manageCameraMovement();
        manageDragAndDrop();
        if ((Input.GetKeyDown(KeyCode.LeftControl) == true) || (Input.GetKeyDown(KeyCode.RightControl) == true)) {
            _heightScript.manuallyCheckHeight();
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

    private void manageDragAndDrop() {
        if ((Input.GetKeyDown(KeyCode.LeftShift) == true) || (Input.GetKeyDown(KeyCode.RightShift) == true)) {
            if ((dragAndDropScript._dragAndDropScript.placedGameObject != null) && (confirmButton.interactable == true)) {
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
        if ((Input.GetKeyDown(KeyCode.D) == true) || (Input.GetKeyDown(KeyCode.RightArrow) == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.rotateRight();
            }
        }
        if ((Input.GetKeyDown(KeyCode.A) == true) || (Input.GetKeyDown(KeyCode.LeftArrow) == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.rotateLeft();
            }
        }
        return;
    }
}