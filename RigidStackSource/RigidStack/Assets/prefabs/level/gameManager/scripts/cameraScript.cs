using System;
using UnityEngine;

public class cameraScript : MonoBehaviour {
    //Variables for moving the camera.
    public static bool shouldMoveCameraUp, shouldMoveCameraDown;
    public static float cameraMovementSpeed = 10f;
    [SerializeField] private endMenuManager _endMenuManager = null;
    [NonSerialized] public Vector3 originalCameraPosition;
    [SerializeField] private Transform platformTransform = null, girdTransform = null;

    private void Start() {
        Vector3 position = platformTransform.position;
        position.y = (position.y + girdTransform.position.y);
        position.z = sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position.z;
        sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position = position;
        originalCameraPosition = position;
        return;
    }

    private void Update() {
        if (shouldMoveCameraUp == true) {
            moveCameraUp();
        } else if (shouldMoveCameraDown == true) {
            moveCameraDown();
        }
        return;
    }

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
        Vector3 newPosition = sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y + (cameraMovementSpeed * Time.unscaledDeltaTime));
        sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position = newPosition;
        return;
    }

    private void moveCameraDown() {
        _endMenuManager.shouldMoveTheCamera = false;
        Vector3 newPosition = sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y - (cameraMovementSpeed * Time.unscaledDeltaTime));
        if (newPosition.y >= -2) {
            sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position = newPosition;
        }
        return;
    }
    #endregion
}