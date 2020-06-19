using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public static class StaticVariables {
    public static bool isDragging;
}

public class dragAndDropScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    private Camera mainCamera;
    private GameObject placeHolderGameObject, placedGameObject;
    [HideInInspector] public GameObject objectToPlace;

    private void Awake() {
        mainCamera = Camera.main;
        return;
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if (StaticVariables.isDragging == false) {
            StaticVariables.isDragging = true;
            placeHolderGameObject = objectToPlace;
            placeHolderGameObject.GetComponent<PolygonCollider2D>().enabled = false;
            placeHolderGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            placedGameObject = Instantiate(placeHolderGameObject, mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity);
        }
        return;
    }

    public virtual void OnDrag(PointerEventData pointerEventData) {
        if (StaticVariables.isDragging == true) {
            placedGameObject.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
        return;
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData) {
        if (StaticVariables.isDragging == true) {
            Rigidbody2D rigidbody2D = placedGameObject.GetComponent<Rigidbody2D>();
            placedGameObject.GetComponent<PolygonCollider2D>().enabled = true;
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
            StaticVariables.isDragging = false;
        }
        return;
    }
}