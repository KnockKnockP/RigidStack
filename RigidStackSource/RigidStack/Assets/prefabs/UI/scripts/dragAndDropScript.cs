using UnityEngine;
using UnityEngine.EventSystems;

public class dragAndDropScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    private objectClass objectImageGameObjectObjectClass;
    private heightScript _heightScript;
    public static dragAndDropScript _dragAndDropScript;
    private Camera mainCamera;
    private GameObject placedGameObject;
    [SerializeField] private GameObject objectImageGameObject = null, towerObjects = null, dockPanel = null, objectEditingPanel = null;
    [HideInInspector] public GameObject objectToPlace;

    private void Awake() {
        objectImageGameObjectObjectClass = objectImageGameObject.GetComponent<objectClass>();
        _heightScript = FindObjectOfType<heightScript>();
        mainCamera = Camera.main;
        return;
    }

    private void Start() {
        disableObjectEditingPanel();
        return;
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticClass.isDragging == false) {
                StaticClass.isDragging = true;
                _dragAndDropScript = this;
                placedGameObject = Instantiate(objectToPlace, mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity, towerObjects.transform);
                placedGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                placedGameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
            }
        }
        return;
    }

    public virtual void OnDrag(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticClass.isDragging == true) {
                placedGameObject.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            }
        }
        return;
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticClass.isDragging == true) {
                placedGameObject.GetComponent<postDragAndDropScript>().placedGameObject = placedGameObject;
                enableObjectEditingPanel();
                StaticClass.isDragging = false;
            }
        }
        return;
    }

    public void placeObject() {
        Rigidbody2D rigidbody2D = placedGameObject.GetComponent<Rigidbody2D>();
        placedGameObject.transform.position = new Vector3(placedGameObject.transform.position.x, placedGameObject.transform.position.y, 0);
        objectImageGameObjectObjectClass.objectCount--;
        disableObjectEditingPanel();
        placedGameObject.GetComponent<postDragAndDropScript>().suicide();
        _heightScript.placedObjectsTransforms.Add(placedGameObject.transform);
        placedGameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;
        return;
    }

    public void cancelPlacingObject() {
        Destroy(placedGameObject);
        disableObjectEditingPanel();
        return;
    }

    public void rotateLeft() {
        placedGameObject.transform.Rotate(0f, 0f, StaticClass.angle);
        return;
    }

    public void rotateRight() {
        placedGameObject.transform.Rotate(0f, 0f, -StaticClass.angle);
        return;
    }

    private void enableObjectEditingPanel() {
        dockPanel.SetActive(false);
        objectEditingPanel.SetActive(true);
        FindObjectOfType<objectEditingScript>().updateAngleValue();
        return;
    }

    private void disableObjectEditingPanel() {
        dockPanel.SetActive(true);
        objectEditingPanel.SetActive(false);
        return;
    }
}