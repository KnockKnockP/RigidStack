using Mirror;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

//TODO : Serialize NetworkIdentity component for placedGameObject.
public class dragAndDropScript : NetworkBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    //Variables for dragging and dropping the object.
    private bool isDragging, isClientThatPlacedTheObject;
    public static int index;
    //TODO : Sync this variable.
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

    #region Selecting the object from the dock.
    public virtual void OnPointerDown(PointerEventData pointerEventData) {
        if ((_dragAndDropImageScript.objectCount != 0) && (isDragging == false)) {
            isDragging = true;
            _dragAndDropScript = this;
            //Decrement objectCount here.
            isClientThatPlacedTheObject = true;
            commandSpawnObject(objectToPlace, sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), index);
            //commandUpdatePlacedGameObject(placedGameObject);
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandSpawnObject(GameObject _gameObject, Vector3 position, int _index) {
        //HACK : Replace this hack.
        if (isServer == true) {
            _gameObject = objectToPlace;
        }
        GameObject _placedGameObject = Instantiate(_gameObject, position, Quaternion.identity);
        NetworkServer.Spawn(_placedGameObject);
        uint id = _placedGameObject.GetComponent<NetworkIdentity>().netId;
        targetRPCReturnPlacedGameObject(DummyPlayerScript.dummyPlayerScript.dummyPlayerSyncList[_index].networkIdentity.connectionToClient, id);
        clientRPCUpdatePlacedGameObject(id);
        return;
    }

    [TargetRpc]
    private void targetRPCReturnPlacedGameObject(NetworkConnection networkConnection, uint id) {
        _ = networkConnection;
        placedGameObject = NetworkIdentity.spawned[id].gameObject;
        Debug.Log("Here in targetRPCReturnPlacedGameObject.");
        return;
    }

    [ClientRpc]
    private void clientRPCUpdatePlacedGameObject(uint id) {
        GameObject _gameObject = NetworkIdentity.spawned[id].gameObject;
        if (isClientThatPlacedTheObject == false) {
            Destroy(_gameObject.GetComponent<postDragAndDropScript>());
        } else {
            isClientThatPlacedTheObject = false;
        }
        _gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        _gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        _gameObject.GetComponent<SpriteRenderer>().sortingOrder = spriteOrder;
        if (_gameObject.name.Contains("television") == true) {
            _gameObject.GetComponent<televisionScript>().videoPlayerSpriteRenderer.sortingOrder = spriteOrder;
        }
        if (spriteOrder != 32766) {
            spriteOrder++;
        }
        return;
    }
    #endregion

    #region Dragging the selected object.
    public virtual void OnDrag(PointerEventData pointerEventData) {
        if ((placedGameObject != null) && (_dragAndDropImageScript.objectCount != 0) && (isDragging == true)) {
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
            enableObjectEditingPanel();
            isDragging = false;
        }
        return;
    }

    #region Placing the object.
    public void placeObject() {
        disableObjectEditingPanel();
        commandPlaceObject(placedGameObject.GetComponent<NetworkIdentity>().netId);
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandPlaceObject(uint id) {
        clientRPCPlaceObject(id);
        return;
    }

    [ClientRpc]
    private void clientRPCPlaceObject(uint id) {
        GameObject _gameObject = NetworkIdentity.spawned[id].gameObject;
        _gameObject.transform.position = new Vector3(_gameObject.transform.position.x, _gameObject.transform.position.y, 0);
        if (isClientThatPlacedTheObject == true) {
            _gameObject.GetComponent<postDragAndDropScript>().suicide();
        }

        _heightScript.placedObjects.Add(_gameObject);
        _heightScript.placedObjectsTransforms.Add(_gameObject.transform);
        _endMenuManager.allPlacedObjectsSpriteRenderers.Add(_gameObject.GetComponent<SpriteRenderer>());
        Rigidbody2D rigidbody2D = _gameObject.GetComponent<Rigidbody2D>();
        _heightScript.placedObjectsRigidbody2D.Add(rigidbody2D);

        _gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        rigidbody2D.constraints = RigidbodyConstraints2D.None;

        if ((placedGameObject != null) && (_gameObject.GetComponent<NetworkIdentity>().netId == placedGameObject.GetComponent<NetworkIdentity>().netId)) {
            placedGameObject = null;
        }
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
        commandRotate(placedGameObject, -objectEditingScript.angle);
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandRotate(GameObject _gameObject, int angle) {
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