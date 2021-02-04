using UnityEngine;
using UnityEngine.EventSystems;

public class WindowScript : MonoBehaviour, IPointerDownHandler, IDragHandler {
    private Vector2 offset;
    private Transform _transform;

    private void Awake() {
        _transform = transform;
    }

    public void OnPointerDown(PointerEventData pointerEventData) {
        offset = CalculateOffset(pointerEventData.position, _transform.position);
        return;
    }

    public void OnDrag(PointerEventData pointerEventData) {
        _transform.position = (pointerEventData.position - offset);
        return;
    }

    public static Vector2 CalculateOffset(Vector3 mousePosition, Vector3 gameObjectPosition) {
        Vector2 offset = new Vector2((mousePosition.x - gameObjectPosition.x), (mousePosition.y - gameObjectPosition.y));
        return offset;
    }

    public static Vector3 GetMouseToWorldPoint() {
        Vector3 mouseInput = Input.mousePosition;
        return sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));
    }
}