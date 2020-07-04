using UnityEngine;

public class postDragAndDropScript : MonoBehaviour {
    private objectEditingScript _objectEditingScript;
    private Camera mainCamera;
    [HideInInspector] public GameObject placedGameObject;

    private void Awake() {
        _objectEditingScript = FindObjectOfType<objectEditingScript>();
        mainCamera = Camera.main;
        return;
    }

    private void OnMouseDrag() {
        placedGameObject.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        if (StaticClass.isCollisionFishy == true) {
            _objectEditingScript.confirmButton.interactable = false;
        }
        return;
    }

    private void OnTriggerEnter2D() {
        _objectEditingScript.confirmButton.interactable = false;
        return;
    }

    private void OnTriggerExit2D() {
        _objectEditingScript.confirmButton.interactable = true;
        return;
    }

    public void suicide() {
        placedGameObject = null;
        Destroy(this);
        return;
    }
}