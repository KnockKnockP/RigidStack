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
        offset = CalculateOffset(GetMouseToWorldPoint(), thisTransform.position);
        return;
    }

    private void OnMouseDrag() {
        commandDragObject(GetMouseToWorldPoint(), offset);
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandDragObject(Vector3 mouseinput, Vector3 _offset) {
        Vector3 newPosition = new Vector3((mouseinput.x - _offset.x), (mouseinput.y - _offset.y), 0f);
        thisTransform.position = newPosition;
        return;
    }
    #endregion

    private static Vector3 GetMouseToWorldPoint() {
        Vector3 mouseInput = Input.mousePosition;
        return sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));
    }

    private static Vector2 CalculateOffset(Vector3 mousePosition, Vector3 gameObjectPosition) {
        Vector2 offset = new Vector2((mousePosition.x - gameObjectPosition.x), (mousePosition.y - gameObjectPosition.y));
        return offset;
    }
}