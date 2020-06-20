using UnityEngine;
using UnityEngine.EventSystems;

public class dragAndDropScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    private objectClass objectImageGameObjectObjectClass;
    private Camera mainCamera;
    private GameObject placeHolderGameObject, placedGameObject, towerObjects, dockPanel, objectEditingPanel;
    [SerializeField] private GameObject objectImageGameObject = null;
    [HideInInspector] public GameObject objectToPlace;

    private void Awake() {
        objectImageGameObjectObjectClass = objectImageGameObject.GetComponent<objectClass>();
        mainCamera = Camera.main;
        towerObjects = GameObject.Find("towerObjects");
        dockPanel = GameObject.Find("dockPanel");
        objectEditingPanel = GameObject.Find("objectEditingPanel");
        return;
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if (objectImageGameObjectObjectClass.objectCount != 0) {
            if (StaticVariables.isDragging == false) {
                StaticVariables.isDragging = true;
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
            }
        }
        return;
    }

    private void enableObjectEditingPanel() {
        dockPanel.SetActive(false);
        objectEditingPanel.SetActive(true);
        return;
    }

    public void placeObject() {
        disableObjectEditingPanel();
        Rigidbody2D rigidbody2D = placedGameObject.GetComponent<Rigidbody2D>();
        objectImageGameObjectObjectClass.objectCount--;
        placedGameObject.GetComponent<PolygonCollider2D>().enabled = true;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;
        StaticVariables.isDragging = false;
        return;
    }

    private void disableObjectEditingPanel() {
        dockPanel.SetActive(true);
        objectEditingPanel.SetActive(false);
        return;
    }
}