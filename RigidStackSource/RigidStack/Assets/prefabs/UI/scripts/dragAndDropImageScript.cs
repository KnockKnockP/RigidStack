using UnityEngine;
using UnityEngine.UI;

public class dragAndDropImageScript : MonoBehaviour {
    //Variables for tracking the object count.
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
}