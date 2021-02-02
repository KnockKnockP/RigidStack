using UnityEngine;

public class sharedMonobehaviour : MonoBehaviour {
    public static float leftSide, orthographicSize;
    public Camera mainCamera;
    public GameObject dockPanel, objectEditingPanel;
    public static sharedMonobehaviour _sharedMonobehaviour = null;

    private void Awake() {
        if (_sharedMonobehaviour != null) {
            Destroy(this);
        } else {
            _sharedMonobehaviour = this;
        }
        leftSide = mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x;
        orthographicSize = mainCamera.orthographicSize;
        FindObjectOfType<dragAndDropScript>()._Start();
        return;
    }
}