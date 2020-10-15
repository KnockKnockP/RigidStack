using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class dragAndDropScript : NetworkBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    //Variables for dragging and dropping the object.
    private bool isDragging;
    //Syncvar this.
    private static short spriteOrder = 1;
    private dragAndDropImageScript _dragAndDropImageScript;
    private heightScript _heightScript;
    private endMenuManager _endMenuManager;
    public static dragAndDropScript _dragAndDropScript;
    [SyncVar(hook = nameof(syncPlacedGameObjectClient))] private GameObject placedGameObjectSyncClient = null;
    [SyncVar(hook = nameof(syncPlacedGameObjectServer))] private GameObject placedGameObjectSyncServer = null;
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

    #region Selecting the object from the dock.
    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if ((_dragAndDropImageScript.objectCount != 0) && (isDragging == false)) {
            isDragging = true;
            _dragAndDropScript = this;
            commandSpawnObject(objectToPlace, sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), isClientOnly);
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandSpawnObject(GameObject _gameObject, Vector3 position, bool _isClientOnly) {
        if (isServer == true) {
            _gameObject = objectToPlace;
        }
        GameObject _placedGameObject = Instantiate(_gameObject, position, Quaternion.identity);
        NetworkServer.Spawn(_placedGameObject);
        if (_isClientOnly == true) {
            placedGameObjectSyncClient = _placedGameObject;
        } else {
            placedGameObjectSyncServer = _placedGameObject;
        }
        return;
    }

    private void syncPlacedGameObjectClient(GameObject oldGameObject, GameObject newGameObject) {
        _ = oldGameObject;
        placedGameObject = newGameObject;
        updatePlacedGameObject();
        return;
    }

    private void syncPlacedGameObjectServer(GameObject oldGameObject, GameObject newGameObject) {
        _ = oldGameObject;
        placedGameObject = newGameObject;
        updatePlacedGameObject();
        return;
    }

    private void updatePlacedGameObject() {
        placedGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        placedGameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        placedGameObject.GetComponent<SpriteRenderer>().sortingOrder = spriteOrder;
        if (placedGameObject.name.Contains("television") == true) {
            placedGameObject.GetComponent<televisionScript>().videoPlayerSpriteRenderer.sortingOrder = spriteOrder;
        }
        if (spriteOrder != 32766) {
            spriteOrder++;
        }
        return;
    }
    #endregion

    #region Dragging the selected object.
    public virtual void OnDrag(PointerEventData pointerEventData) {
        if (placedGameObject == null) {
            return;
        }
        if ((_dragAndDropImageScript.objectCount != 0) && (isDragging == true)) {
            commandDragObject(placedGameObject, sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)));
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandDragObject(GameObject _gameObject, Vector3 position) {
        _gameObject.transform.position = position;
        return;
    }
    #endregion

    public virtual void OnPointerUp(PointerEventData pointerEventData) {
        if ((_dragAndDropImageScript.objectCount != 0) && (isDragging == true)) {
            if (placedGameObject == null) {
                isDragging = false;
            }
            enableObjectEditingPanel();
            isDragging = false;
        }
        return;
    }

    #region Placing the object.
    public void placeObject() {
        disableObjectEditingPanel();
        commandPlaceObject(isClientOnly);
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandPlaceObject(bool _isClientOnly) {
        GameObject _gameObject;
        if (_isClientOnly == true) {
            _gameObject = placedGameObjectSyncClient;
        } else {
            _gameObject = placedGameObjectSyncServer;
        }
        _gameObject.transform.position = new Vector3(_gameObject.transform.position.x, _gameObject.transform.position.y, 0);
        _gameObject.GetComponent<postDragAndDropScript>().suicide();

        _heightScript.placedObjects.Add(_gameObject);
        _heightScript.placedObjectsTransforms.Add(_gameObject.transform);
        _endMenuManager.allPlacedObjectsSpriteRenderers.Add(_gameObject.GetComponent<SpriteRenderer>());
        Rigidbody2D rigidbody2D = _gameObject.GetComponent<Rigidbody2D>();
        _heightScript.placedObjectsRigidbody2D.Add(rigidbody2D);

        _gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;

        placedGameObject = null;
        return;
    }
    #endregion

    #region Cancelling placing the object.
    public void cancelPlacingObject() {
        commandCancelPlacingObject(placedGameObject);
        disableObjectEditingPanel();
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandCancelPlacingObject(GameObject _gameObject) {
        if (_gameObject != null) {
            NetworkServer.UnSpawn(_gameObject);
            Destroy(_gameObject);
            clientRPCIncrementObjectCount();
        }
        return;
    }
    
    [ClientRpc]
    private void clientRPCIncrementObjectCount() {
        _dragAndDropImageScript.objectCount++;
        return;
    }
    #endregion

    #region Rotating the object.
    public void rotateLeft() {
        commandRotate(placedGameObject, objectEditingScript.angle);
        return;
    }

    public void rotateRight() {
        commandRotate(placedGameObject, (byte)(-objectEditingScript.angle));
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandRotate(GameObject _gameObject, byte angle) {
        _gameObject.transform.Rotate(0f, 0f, angle);
        return;
    }
    #endregion

    #region Toggling the object editing panel.
    private void enableObjectEditingPanel() {
        sharedMonobehaviour._sharedMonobehaviour.dockPanel.SetActive(false);
        sharedMonobehaviour._sharedMonobehaviour.objectEditingPanel.SetActive(true);
        FindObjectOfType<objectEditingScript>().updateAngleValue();
        return;
    }

    public void disableObjectEditingPanel() {
        sharedMonobehaviour._sharedMonobehaviour.dockPanel.SetActive(true);
        sharedMonobehaviour._sharedMonobehaviour.objectEditingPanel.SetActive(false);
        return;
    }
    #endregion
}