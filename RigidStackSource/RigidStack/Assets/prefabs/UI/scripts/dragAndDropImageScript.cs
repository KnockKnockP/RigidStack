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
        }
    }
    [SerializeField] private Text objectCounterText = null;
}