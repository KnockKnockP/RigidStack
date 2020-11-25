using Mirror;
using System;
using UnityEngine;

public class postDragAndDropScript : NetworkBehaviour {
    //The object we are going to manipulate.
    [NonSerialized] public GameObject thisGameObject;

    private void Awake() {
        thisGameObject = gameObject;
        return;
    }

    #region Dragging the placed object.
    private void OnMouseDrag() {
        commandDragObject(thisGameObject, sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)));
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandDragObject(GameObject _gameObject, Vector3 position) {
        _gameObject.transform.position = position;
        return;
    }
    #endregion
}