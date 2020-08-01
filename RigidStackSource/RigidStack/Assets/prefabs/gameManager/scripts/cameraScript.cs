using UnityEngine;

public class cameraScript : MonoBehaviour {
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;


    private bool shouldMoveCameraUp, shouldMoveCameraDown;


    public static float cameraMovementSpeed = 0.1f;


    [SerializeField] private Transform platformTransform = null, girdTransform = null;

    private void Awake() {
        Vector3 position = platformTransform.position;
        position.y = (position.y + girdTransform.position.y);
        position.z = _sharedMonobehaviour.mainCamera.transform.position.z;
        _sharedMonobehaviour.mainCamera.transform.position = position;
        return;
    }

    private void FixedUpdate() {
        if (shouldMoveCameraUp == true) {
            moveCameraUp();
        } else if (shouldMoveCameraDown == true) {
            moveCameraDown();
        }
        return;
    }

    public void toggleCameraUp() {
        shouldMoveCameraUp = (!shouldMoveCameraUp);
        return;
    }

    public void toggleCameraDown() {
        shouldMoveCameraDown = (!shouldMoveCameraDown);
        return;
    }

    private void moveCameraUp() {
        Vector3 newPosition = _sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y + cameraMovementSpeed);
        _sharedMonobehaviour.mainCamera.transform.position = newPosition;
        return;
    }

    private void moveCameraDown() {
        Vector3 newPosition = _sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y - cameraMovementSpeed);
        if (newPosition.y >= -2) {
            _sharedMonobehaviour.mainCamera.transform.position = newPosition;
        }
        return;
    }
}