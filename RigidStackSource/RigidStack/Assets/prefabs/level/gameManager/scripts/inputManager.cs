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
    [SerializeField] private heightScript _heightScript = null;
    [SerializeField] private Button confirmButton = null;

    private void Update() {
        manageCameraMovement();
        manageDragAndDrop();
        manageOthers();
        return;
    }

    private void manageCameraMovement() {
        if ((Input.GetKeyDown(KeyCode.W) == true) || (Input.GetKeyDown(KeyCode.UpArrow) == true)) {
            cameraScript.shouldMoveCameraUp = true;
            cameraScript.shouldMoveCameraDown = false;
        }
        if ((Input.GetKeyUp(KeyCode.W) == true) || (Input.GetKeyUp(KeyCode.UpArrow) == true)) {
            cameraScript.shouldMoveCameraUp = false;
        }
        if ((Input.GetKeyDown(KeyCode.S) == true) || (Input.GetKeyDown(KeyCode.DownArrow) == true)) {
            cameraScript.shouldMoveCameraDown = true;
            cameraScript.shouldMoveCameraUp = false;
        }
        if ((Input.GetKeyUp(KeyCode.S) == true) || (Input.GetKeyUp(KeyCode.DownArrow) == true)) {
            cameraScript.shouldMoveCameraDown = false;
        }
        return;
    }

    private void manageDragAndDrop() {
        if ((confirmButton.interactable == true) && (confirmButton.gameObject.activeInHierarchy == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                if ((Input.GetKeyDown(KeyCode.LeftShift) == true) || (Input.GetKeyDown(KeyCode.RightShift) == true)) {
                    if (confirmButton.interactable == true) {
                        dragAndDropScript._dragAndDropScript.placeObject();
                    }
                }
                if ((Input.GetKeyDown(KeyCode.D) == true) || (Input.GetKeyDown(KeyCode.RightArrow) == true)) {
                    dragAndDropScript._dragAndDropScript.rotateRight();
                }
                if ((Input.GetKeyDown(KeyCode.A) == true) || (Input.GetKeyDown(KeyCode.LeftArrow) == true)) {
                    dragAndDropScript._dragAndDropScript.rotateLeft();
                }
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

    private void manageOthers() {
        if ((Input.GetKeyDown(KeyCode.LeftControl) == true) || (Input.GetKeyDown(KeyCode.RightControl) == true)) {
            _heightScript.manuallyCheckHeight();
        }
        return;
    }
}