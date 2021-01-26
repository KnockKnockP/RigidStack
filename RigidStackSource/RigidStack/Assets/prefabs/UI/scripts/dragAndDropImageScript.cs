using UnityEngine;
using UnityEngine.UI;

public class dragAndDropImageScript : MonoBehaviour {
    private short _objectCount = 0;
    public short objectCount {
        get => _objectCount;
        set {
            if (_objectCount != value) {
                _objectCount = value;
                objectCounterText.text = _objectCount.ToString();
            }
            return;
        }
    }
    [SerializeField] private Text objectCounterText = null;
}