#region A using tag.
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "objectInformation" class.
public class objectInformation : MonoBehaviour {
    #region Variables.
    #region Variables for determining how many objects the player should get.
    public int minimumAmount = 0, maximumAmount = 0;
    #endregion

    #region A variable to check if the object has been placed down.
    [SerializeField] private postDragAndDropScript _postDragAndDropScript = null;
    #endregion
    #endregion

    #region Update function.
    private void Update() {
        if ((_postDragAndDropScript == null) && (transform.position.y < -1f)) {
            FindObjectOfType<endMenuManager>().endGame();
        }
        return;
    }
    #endregion
}
#endregion