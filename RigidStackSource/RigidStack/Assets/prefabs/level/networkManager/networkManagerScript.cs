using UnityEngine;

public class networkManagerScript : MonoBehaviour {
    [SerializeField] private bool isParentObject = false;
    [SerializeField] private GameObject childObject = null;

    private void Awake() {
        if (isParentObject == true) {
            childObject.SetActive(loadSceneScript.isMultiplayer);
            Destroy(this);
        }
        return;
    }
}