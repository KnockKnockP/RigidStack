using UnityEngine;
using UnityEngine.UI;

public class dragAndDropImageScript : MonoBehaviour {
    private short _objectCount = 0;
    public short objectCount {
        get {
            return _objectCount;
        }
        set {
            if (_objectCount != value) {
                _objectCount = value;
                objectCounterText.text = _objectCount.ToString();
            }
        }
    }
    [SerializeField] private Text objectCounterText = null;
}