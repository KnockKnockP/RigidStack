#region A using tag.
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "cameraScript" class.
public class cameraScript : MonoBehaviour {
    #region Variables.
    #region A variable for accessing the main camera.
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;
    #endregion

    #region Variables for moving the camera.
    public static bool shouldMoveCameraUp, shouldMoveCameraDown;
    public static float cameraMovementSpeed = 10f;
    [SerializeField] private endMenuManager _endMenuManager = null;
    [HideInInspector] public Vector3 originalCameraPosition;
    [SerializeField] private Transform platformTransform = null, girdTransform = null;
    #endregion
    #endregion

    #region Awake function.
    private void Awake() {
        Vector3 position = platformTransform.position;
        position.y = (position.y + girdTransform.position.y);
        position.z = _sharedMonobehaviour.mainCamera.transform.position.z;
        _sharedMonobehaviour.mainCamera.transform.position = position;
        originalCameraPosition = position;
        return;
    }
    #endregion

    #region Update function.
    private void Update() {
        if (shouldMoveCameraUp == true) {
            moveCameraUp();
        } else if (shouldMoveCameraDown == true) {
            moveCameraDown();
        }
        return;
    }
    #endregion

    #region Toggling the camera.
    public void setBooleanToMoveTheCameraUp(bool _value) {
        shouldMoveCameraUp = _value;
        return;
    }

    public void setBooleanToMoveTheCameraDown(bool _value) {
        shouldMoveCameraDown = _value;
        return;
    }
    #endregion

    #region Moving the camera.
    private void moveCameraUp() {
        _endMenuManager.shouldMoveTheCamera = false;
        Vector3 newPosition = _sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y + (cameraMovementSpeed * Time.unscaledDeltaTime));
        _sharedMonobehaviour.mainCamera.transform.position = newPosition;
        return;
    }

    private void moveCameraDown() {
        _endMenuManager.shouldMoveTheCamera = false;
        Vector3 newPosition = _sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y - (cameraMovementSpeed * Time.unscaledDeltaTime));
        if (newPosition.y >= -2) {
            _sharedMonobehaviour.mainCamera.transform.position = newPosition;
        }
        return;
    }
    #endregion
}
#endregion