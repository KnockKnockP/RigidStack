using UnityEngine;

public class sharedMonobehaviour : MonoBehaviour {
    //Shared variables.
    public Camera mainCamera;
    //Used for dragAndDropScript.cs.
    public GameObject dockPanel, objectEditingPanel;
    public static sharedMonobehaviour _sharedMonobehaviour = null;

    private void Awake() {
        if (_sharedMonobehaviour != null) {
            Destroy(this);
        } else {
            _sharedMonobehaviour = this;
        }
        return;
    }
}