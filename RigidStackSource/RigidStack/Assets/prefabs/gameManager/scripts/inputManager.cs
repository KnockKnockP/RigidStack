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
    [SerializeField] private cameraScript _cameraScript = null;
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
            _cameraScript.toggleCameraUp();
            yield return null;
        }
        if ((Input.GetKeyUp(KeyCode.W) == true) || (Input.GetKeyUp(KeyCode.UpArrow) == true)) {
            _cameraScript.toggleCameraUp();
            yield return null;
        }
        if ((Input.GetKeyDown(KeyCode.S) == true) || (Input.GetKeyDown(KeyCode.DownArrow) == true)) {
            _cameraScript.toggleCameraDown();
            yield return null;
        }
        if ((Input.GetKeyUp(KeyCode.S) == true) || (Input.GetKeyUp(KeyCode.DownArrow) == true)) {
            _cameraScript.toggleCameraDown();
            yield return null;
        }
        yield return null;
    }
    #endregion

    #region Managing drag and drop inputs.
    private IEnumerator manageDragAndDrop() {
        yield return null;
        if ((Input.GetKeyDown(KeyCode.LeftShift) == true) || (Input.GetKeyDown(KeyCode.RightShift) == true)) {
            if ((dragAndDropScript._dragAndDropScript.placedGameObject != null) && (confirmButton.interactable == true)) {
                dragAndDropScript._dragAndDropScript.placeObject();
            }
            yield return null;
        }
        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.cancelPlacingObject();
            } else {
                _pauseScript.pauseOrResume();
            }
            yield return null;
        }
        if ((Input.GetKeyDown(KeyCode.D) == true) || (Input.GetKeyDown(KeyCode.RightArrow) == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.rotateRight();
            }
            yield return null;
        }
        if ((Input.GetKeyDown(KeyCode.A) == true) || (Input.GetKeyDown(KeyCode.LeftArrow) == true)) {
            if (dragAndDropScript._dragAndDropScript.placedGameObject != null) {
                dragAndDropScript._dragAndDropScript.rotateLeft();
            }
            yield return null;
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