using UnityEngine;

public class cameraScript : MonoBehaviour {
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;


    [SerializeField] private endMenuManager _endMenuManager = null;
    private bool shouldMoveCameraUp, shouldMoveCameraDown;


    public static float cameraMovementSpeed = 0.1f;


    [HideInInspector] public Vector3 originalCameraPosition;
    [SerializeField] private Transform platformTransform = null, girdTransform = null;

    private void Awake() {
        Vector3 position = platformTransform.position;
        position.y = (position.y + girdTransform.position.y);
        position.z = _sharedMonobehaviour.mainCamera.transform.position.z;
        _sharedMonobehaviour.mainCamera.transform.position = position;
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

    public void toggleCameraUp() {
        shouldMoveCameraUp = (!shouldMoveCameraUp);
        return;
    }

    public void toggleCameraDown() {
        shouldMoveCameraDown = (!shouldMoveCameraDown);
        return;
    }

    private void moveCameraUp() {
        _endMenuManager.shouldMoveTheCamera = false;
        Vector3 newPosition = _sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y + cameraMovementSpeed);
        _sharedMonobehaviour.mainCamera.transform.position = newPosition;
        return;
    }

    private void moveCameraDown() {
        _endMenuManager.shouldMoveTheCamera = false;
        Vector3 newPosition = _sharedMonobehaviour.mainCamera.transform.position;
        newPosition.y = (newPosition.y - cameraMovementSpeed);
        if (newPosition.y >= -2) {
            _sharedMonobehaviour.mainCamera.transform.position = newPosition;
        }
        return;
    }
}