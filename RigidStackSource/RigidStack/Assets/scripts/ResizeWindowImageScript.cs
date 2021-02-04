using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeWindowImageScript : MonoBehaviour, IPointerDownHandler, IDragHandler {
    private Vector2 originalPosition, originalScale;
    [SerializeField] private RectTransform canvasRectTransform = null, targetRectTransform = null;

    public void OnPointerDown(PointerEventData pointerEventData) {
        originalPosition = targetRectTransform.position;
        originalScale = targetRectTransform.sizeDelta;
        return;
    }

    public void OnDrag(PointerEventData pointerEventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pointerEventData.position, null, out Vector2 point);
        targetRectTransform.position = new Vector2(originalPosition.x + (point.x * 0.5f), originalPosition.y + (point.y * 0.5f));
        targetRectTransform.sizeDelta = new Vector2((originalScale.x + point.x), (originalScale.y - point.y));
        return;
    }
}