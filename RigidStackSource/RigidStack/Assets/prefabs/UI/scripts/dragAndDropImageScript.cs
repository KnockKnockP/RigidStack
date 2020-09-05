#region Using tags.
using UnityEngine;
using UnityEngine.UI;
#endregion

#region "MonoBehaviour" inherited "dragAndDropImageScript" class.
public class dragAndDropImageScript : MonoBehaviour {
    #region Variables for tracking the object count.
    private short _objectCount = 0;
    [HideInInspector] public short objectCount {
        get {
            return _objectCount;
        }
        set {
            _objectCount = value;
            objectCounterText.text = _objectCount.ToString();
            if (_objectCount == 0) {
                Debug.Log("You can not use this object.");
            }
        }
    }
    [SerializeField] private Text objectCounterText = null;
    #endregion
}
#endregion