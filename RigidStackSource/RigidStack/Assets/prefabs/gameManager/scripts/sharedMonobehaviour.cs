#region A using tag.
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "sharedMonobehaviour" class.
public class sharedMonobehaviour : MonoBehaviour {
    #region Shared variables.
    public Camera mainCamera;
    //Used for dragAndDropScript.cs.
    public GameObject towerObjects, dockPanel, objectEditingPanel;
    #endregion
}
#endregion