using Mirror;
using System;
using UnityEngine;

public class postDragAndDropScript : NetworkBehaviour {
    //Variables for manipulating the object.
    public static bool isCollisionFishy;
    private objectEditingScript _objectEditingScript;

    //The object we are going to manipulate.
    [NonSerialized] public GameObject thisGameObject;

    private void Awake() {
        _objectEditingScript = FindObjectOfType<objectEditingScript>();
        thisGameObject = gameObject;
        return;
    }

    #region Dragging the placed object.
    private void OnMouseDrag() {
        commandDragObject(thisGameObject, sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)));
        if (isCollisionFishy == true) {
            _objectEditingScript.confirmButton.interactable = false;
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandDragObject(GameObject _gameObject, Vector3 position) {
        _gameObject.transform.position = position;
        return;
    }
    #endregion

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
}