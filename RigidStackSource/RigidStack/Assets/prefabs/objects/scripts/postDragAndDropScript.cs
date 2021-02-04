using Mirror;
using System;
using UnityEngine;

public class postDragAndDropScript : NetworkBehaviour {
    private Vector2 offset = Vector2.zero;

    //The object we are going to manipulate.
    private Transform thisTransform;
    [NonSerialized] public GameObject thisGameObject;

    private void Awake() {
        thisGameObject = gameObject;
        thisTransform = gameObject.transform;
        return;
    }

    #region Dragging the placed object.
    private void OnMouseDown() {
        offset = WindowScript.CalculateOffset(WindowScript.GetMouseToWorldPoint(), thisTransform.position);
        return;
    }

    private void OnMouseDrag() {
        commandDragObject(WindowScript.GetMouseToWorldPoint(), offset);
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandDragObject(Vector3 mouseinput, Vector3 _offset) {
        Vector3 newPosition = new Vector3((mouseinput.x - _offset.x), (mouseinput.y - _offset.y), 0f);
        thisTransform.position = newPosition;
        return;
    }
    #endregion
}