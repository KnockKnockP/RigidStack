using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class dragAndDropScript : NetworkBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    //A variable for accessing the shared variables.
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;

    //Variables for dragging and dropping the object.
    private bool isDragging;
    //Syncvar this.
    private static short spriteOrder = 1;
    private dragAndDropImageScript _dragAndDropImageScript;
    private heightScript _heightScript;
    private endMenuManager _endMenuManager;
    public static dragAndDropScript _dragAndDropScript;
    [SerializeField] private GameObject dragAndDropImageGameobject = null;
    //The object the player can place using this drag and drop image.
    [HideInInspector] public GameObject objectToPlace, placedGameObject;

    private void Start() {
        _dragAndDropImageScript = dragAndDropImageGameobject.GetComponent<dragAndDropImageScript>();
        _heightScript = FindObjectOfType<heightScript>();
        _endMenuManager = FindObjectOfType<endMenuManager>();
        disableObjectEditingPanel();
        return;
    }

    //Selecting the object from the dock.
    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if (_dragAndDropImageScript.objectCount != 0) {
            if (isDragging == false) {
                isDragging = true;
                _dragAndDropScript = this;
                placedGameObject = Instantiate(objectToPlace, _sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity, _sharedMonobehaviour.towerObjects.transform);
                placedGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                placedGameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
                placedGameObject.GetComponent<SpriteRenderer>().sortingOrder = spriteOrder;
                if (placedGameObject.name.Contains("television") == true) {
                    placedGameObject.GetComponent<televisionScript>().videoPlayerSpriteRenderer.sortingOrder = spriteOrder;
                }
                if (NetworkManagerScript.isMultiplayerGame == true) {
                    NetworkServer.Spawn(placedGameObject, connectionToServer);
                }
                if (spriteOrder != 32766) {
                    spriteOrder++;
                }
            }
        }
        return;
    }

    public virtual void OnDrag(PointerEventData pointerEventData) {
        if (placedGameObject == null) {
            return;
        }
        if (_dragAndDropImageScript.objectCount != 0) {
            if (isDragging == true) {
                placedGameObject.transform.position = _sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            }
        }
        return;
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData) {
        if (_dragAndDropImageScript.objectCount != 0) {
            if (isDragging == true) {
                if (placedGameObject == null) {
                    isDragging = false;
                    return;
                }
                postDragAndDropScript placedObjectPostDragAndDropScript = placedGameObject.GetComponent<postDragAndDropScript>();
                placedObjectPostDragAndDropScript._sharedMonobehaviour = _sharedMonobehaviour;
                placedObjectPostDragAndDropScript.placedGameObject = placedGameObject;
                enableObjectEditingPanel();
                isDragging = false;
            }
        }
        return;
    }

    public void placeObject() {
        placedGameObject.transform.position = new Vector3(placedGameObject.transform.position.x, placedGameObject.transform.position.y, 0);
        _dragAndDropImageScript.objectCount--;
        disableObjectEditingPanel();
        placedGameObject.GetComponent<postDragAndDropScript>().suicide();

        _heightScript.placedObjects.Add(placedGameObject);
        _heightScript.placedObjectsTransforms.Add(placedGameObject.transform);
        _endMenuManager.allPlacedObjectsSpriteRenderers.Add(placedGameObject.GetComponent<SpriteRenderer>());
        Rigidbody2D rigidbody2D = placedGameObject.GetComponent<Rigidbody2D>();
        _heightScript.placedObjectsRigidbody2D.Add(rigidbody2D);

        placedGameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;

        placedGameObject = null;
        return;
    }

    public void cancelPlacingObject() {
        Destroy(placedGameObject);
        disableObjectEditingPanel();
        return;
    }

    #region Rotating the object.
    public void rotateLeft() {
        placedGameObject.transform.Rotate(0f, 0f, objectEditingScript.angle);
        return;
    }

    public void rotateRight() {
        placedGameObject.transform.Rotate(0f, 0f, -objectEditingScript.angle);
        return;
    }
    #endregion

    #region Toggling the object editing panel.
    private void enableObjectEditingPanel() {
        _sharedMonobehaviour.dockPanel.SetActive(false);
        _sharedMonobehaviour.objectEditingPanel.SetActive(true);
        FindObjectOfType<objectEditingScript>().updateAngleValue();
        return;
    }

    private void disableObjectEditingPanel() {
        _sharedMonobehaviour.dockPanel.SetActive(true);
        _sharedMonobehaviour.objectEditingPanel.SetActive(false);
        return;
    }
    #endregion
}