using UnityEngine;
using UnityEngine.UI;

public class cameraScript : MonoBehaviour {
    private bool shouldMoveCameraUp, shouldMoveCameraDown;
    [SerializeField] private Transform platformTransform, girdTransform;
    [SerializeField] private Text templateText;
    private Camera mainCamera;

    private void Awake() {
        Vector3 position = platformTransform.position;
        mainCamera = Camera.main;
        position.y = (position.y + girdTransform.position.y);
        position.z = mainCamera.transform.position.z;
        mainCamera.transform.position = position;
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
        if (newPosition.y >= -2) {
            mainCamera.transform.position = newPosition;
        }
        return;
    }
}