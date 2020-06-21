using UnityEngine;
using UnityEngine.EventSystems;

public class dragAndDropScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    private objectClass objectImageGameObjectObjectClass;
    public static dragAndDropScript _dragAndDropScript;
    private Camera mainCamera;
    private GameObject placeHolderGameObject, placedGameObject;
    [SerializeField] private GameObject objectImageGameObject = null, towerObjects = null, dockPanel = null, objectEditingPanel = null;
    [HideInInspector] public GameObject objectToPlace;

    private void Awake() {
        objectImageGameObjectObjectClass = objectImageGameObject.GetComponent<objectClass>();
        mainCamera = Camera.main;
        return;
    }

    private void Start() {
        disableObjectEditingPanel();
        return;
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticVariables.isDragging == false) {
                StaticVariables.isDragging = true;
                _dragAndDropScript = this;
                placeHolderGameObject = objectToPlace;
                placeHolderGameObject.GetComponent<PolygonCollider2D>().enabled = false;
                placeHolderGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                placedGameObject = Instantiate(placeHolderGameObject, mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity, towerObjects.transform);
            }
        }
        return;
    }

    public virtual void OnDrag(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticVariables.isDragging == true) {
                placedGameObject.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            }
        }
        return;
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticVariables.isDragging == true) {
                enableObjectEditingPanel();
                StaticVariables.isDragging = false;
            }
        }
        return;
    }

    public void placeObject() {
        Rigidbody2D rigidbody2D = placedGameObject.GetComponent<Rigidbody2D>();
        disableObjectEditingPanel();
        objectImageGameObjectObjectClass.objectCount--;
        placedGameObject.GetComponent<PolygonCollider2D>().enabled = true;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;
        return;
    }

    public void cancelPlacingObject() {
        Destroy(placedGameObject);
        disableObjectEditingPanel();
        return;
    }

    public void rotateLeft() {
        placedGameObject.transform.Rotate(0f, 0f, 5f);
        return;
    }

    public void rotateRight() {
        placedGameObject.transform.Rotate(0f, 0f, -5f);
        return;
    }

    private void enableObjectEditingPanel() {
        dockPanel.SetActive(false);
        objectEditingPanel.SetActive(true);
        return;
    }

    private void disableObjectEditingPanel() {
        dockPanel.SetActive(true);
        objectEditingPanel.SetActive(false);
        return;
    }
}