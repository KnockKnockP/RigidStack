#region Using tags.
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#endregion

#region "MonoBehaviour" inherited "inputManager" class.
public class inputManager : MonoBehaviour {
    #region Controls comment.
    /*
        W, up arrow key : Move camera up,
        S, down arrow key : Move camera down,
        A, left arrow key : Rotate to left,
        D, right arrow key : Rotate to right,
        Left shift, right shift : Place the object,
        Escape : Cancel object editing, pause,
        Left control, right control : Manually check height.
    */
    #endregion

    #region Variables for input management.
    [SerializeField] private pauseScript _pauseScript = null;
    [SerializeField] private heightScript _heightScript = null;
    [SerializeField] private Button confirmButton = null;
    #endregion

    #region Update function.
    private void Update() {
        StartCoroutine(manageCameraMovement());
        StartCoroutine(manageDragAndDrop());
        StartCoroutine(manageOthers());
        return;
    }
    #endregion

    #region Managing the camera movement inputs.
    private IEnumerator manageCameraMovement() {
        yield return null;
        if ((Input.GetKeyDown(KeyCode.W) == true) || (Input.GetKeyDown(KeyCode.UpArrow) == true)) {
            cameraScript.shouldMoveCameraUp = true;
            cameraScript.shouldMoveCameraDown = false;
            yield return null;
        }
        if ((Input.GetKeyUp(KeyCode.W) == true) || (Input.GetKeyUp(KeyCode.UpArrow) == true)) {
            cameraScript.shouldMoveCameraUp = false;
            yield return null;
        }
        if ((Input.GetKeyDown(KeyCode.S) == true) || (Input.GetKeyDown(KeyCode.DownArrow) == true)) {
            cameraScript.shouldMoveCameraDown = true;
            cameraScript.shouldMoveCameraUp = false;
            yield return null;
        }
        if ((Input.GetKeyUp(KeyCode.S) == true) || (Input.GetKeyUp(KeyCode.DownArrow) == true)) {
            cameraScript.shouldMoveCameraDown = false;
            yield return null;
        }
        yield return null;
    }
    #endregion

    #region Managing drag and drop inputs.
    private IEnumerator manageDragAndDrop() {
        yield return null;
        if ((confirmButton.interactable == true) && (confirmButton.gameObject.activeInHierarchy == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                if ((Input.GetKeyDown(KeyCode.LeftShift) == true) || (Input.GetKeyDown(KeyCode.RightShift) == true)) {
                    if (confirmButton.interactable == true) {
                        dragAndDropScript._dragAndDropScript.placeObject();
                    }
                    yield return null;
                }
                if ((Input.GetKeyDown(KeyCode.D) == true) || (Input.GetKeyDown(KeyCode.RightArrow) == true)) {
                    dragAndDropScript._dragAndDropScript.rotateRight();
                    yield return null;
                }
                if ((Input.GetKeyDown(KeyCode.A) == true) || (Input.GetKeyDown(KeyCode.LeftArrow) == true)) {
                    dragAndDropScript._dragAndDropScript.rotateLeft();
                    yield return null;
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
        yield return null;
    }
    #endregion

    #region Managing other inputs.
    private IEnumerator manageOthers() {
        yield return null;
        if ((Input.GetKeyDown(KeyCode.LeftControl) == true) || (Input.GetKeyDown(KeyCode.RightControl) == true)) {
            _heightScript.manuallyCheckHeight();
            yield return null;
        }
        yield return null;
    }
    #endregion
}
#endregion