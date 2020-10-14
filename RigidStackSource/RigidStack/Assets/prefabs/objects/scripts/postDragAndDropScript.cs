using Mirror;
using UnityEngine;

public class postDragAndDropScript : NetworkBehaviour {
    //Variables for manipulating the object.
    public static bool isCollisionFishy;
    private objectEditingScript _objectEditingScript;
    //The object we are going to manipulate.
    [HideInInspector] public GameObject placedGameObject;

    private void Awake() {
        _objectEditingScript = FindObjectOfType<objectEditingScript>();
        placedGameObject = gameObject;
        return;
    }

    private void OnMouseDrag() {
        placedGameObject.transform.position = sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        if (isCollisionFishy == true) {
            _objectEditingScript.confirmButton.interactable = false;
        }
        return;
    }

    #region Detecting the collision.
    private void OnTriggerEnter2D() {
        _objectEditingScript.confirmButton.interactable = false;
        return;
    }

    private void OnTriggerExit2D() {
        _objectEditingScript.confirmButton.interactable = true;
        return;
    }
    #endregion

    public void suicide() {
        commandSuicide();
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandSuicide() {
        clientRpcSuicide();
        return;
    }

    [ClientRpc]
    private void clientRpcSuicide() {
        actuallySuicide();
        return;
    }

    private void actuallySuicide() {
        placedGameObject = null;
        Destroy(this);
        return;
    }
}