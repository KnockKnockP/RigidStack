using UnityEngine;

public class cameraScript : MonoBehaviour {
    private bool shouldMoveCameraUp, shouldMoveCameraDown;
    private Camera mainCamera;

    private void Awake() {
        mainCamera = Camera.main;
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
        Vector3 newPosition = mainCamera.transform.position;
        newPosition.y = (newPosition.y + StaticClass.cameraMovementSpeed);
        mainCamera.transform.position = newPosition;
        return;
    }

    private void moveCameraDown() {
        Vector3 newPosition = mainCamera.transform.position;
        newPosition.y = (newPosition.y - StaticClass.cameraMovementSpeed);
        if (newPosition.y >= -3) {
            mainCamera.transform.position = newPosition;
        }
        return;
    }
}