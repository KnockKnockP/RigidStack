using UnityEngine;

public class postDragAndDropScript : MonoBehaviour {
    //A variable for accessing the main camera.
    [HideInInspector] public sharedMonobehaviour _sharedMonobehaviour;

    //Variables for manipulating the object.
    public static bool isCollisionFishy;
    private objectEditingScript _objectEditingScript;
    //The object we are going to manipulate.
    [HideInInspector] public GameObject placedGameObject;

    private void Awake() {
        _objectEditingScript = FindObjectOfType<objectEditingScript>();
        return;
    }

    private void OnMouseDrag() {
        placedGameObject.transform.position = _sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
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
        placedGameObject = null;
        Destroy(this);
        return;
    }
}