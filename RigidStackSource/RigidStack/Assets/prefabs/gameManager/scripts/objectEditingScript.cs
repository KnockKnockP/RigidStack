using UnityEngine;

public class objectEditingScript : MonoBehaviour {
    public void confirmPlacement() {
        dragAndDropScript._dragAndDropScript.placeObject();
        return;
    }

    public void cancelPlacement() {
        dragAndDropScript._dragAndDropScript.cancelPlacingObject();
        return;
    }

    public void rotateLeft() {
        dragAndDropScript._dragAndDropScript.rotateLeft();
        return;
    }

    public void rotateRight() {
        dragAndDropScript._dragAndDropScript.rotateRight();
        return;
    }
}