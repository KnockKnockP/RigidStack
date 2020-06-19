using UnityEngine;
using UnityEngine.UI;

public class objectClass : MonoBehaviour {
    private short _objectCount = 0;
    [HideInInspector] public short objectCount {
        get {
            return _objectCount;
        }
        set {
            _objectCount = value;
            counterText.text = _objectCount.ToString();
        }
    }
    [SerializeField] private Text counterText = null;
    //[SerializeField] private Image _objectSprite = null;
}