#region A using tag.
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "postDragAndDropScript" class.
public class postDragAndDropScript : MonoBehaviour {
    #region Variables.
    #region A variable for accessing the main camera.
    [HideInInspector] public sharedMonobehaviour _sharedMonobehaviour;
    #endregion

    #region Variables for manipulating the object.
    public static bool isCollisionFishy;
    private objectEditingScript _objectEditingScript;
    //The object we are going to manipulate.
    [HideInInspector] public GameObject placedGameObject;
    #endregion
    #endregion

    #region Awake function.
    private void Awake() {
        _objectEditingScript = FindObjectOfType<objectEditingScript>();
        return;
    }
    #endregion

    #region Moving the object.
    private void OnMouseDrag() {
        placedGameObject.transform.position = _sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        if (isCollisionFishy == true) {
            _objectEditingScript.confirmButton.interactable = false;
        }
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

    #region Destroying this script.
    public void suicide() {
        placedGameObject = null;
        Destroy(this);
        return;
    }
    #endregion
}
#endregion