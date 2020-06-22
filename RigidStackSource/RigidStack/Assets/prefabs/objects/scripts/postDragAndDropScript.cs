using UnityEngine;

public class postDragAndDropScript : MonoBehaviour {
    private Camera mainCamera;
    [HideInInspector] public GameObject placedGameObject;

    private void Awake() {
        mainCamera = Camera.main;
        return;
    }

    private void OnMouseDrag() {
        placedGameObject.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        return;
    }

    public void suicide() {
        placedGameObject = null;
        Destroy(this);
        return;
    }
}