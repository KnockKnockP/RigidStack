using UnityEngine;

public class sharedMonobehaviour : MonoBehaviour {
    //Used for all scripts that requires camera to operate.
    public Camera mainCamera;
    //Used for dragAndDropScript.cs.
    public GameObject towerObjects, dockPanel, objectEditingPanel;
}