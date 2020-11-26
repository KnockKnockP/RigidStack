using UnityEngine;
using UnityEngine.UI;

public class fpsCounterScript : MonoBehaviour {
    private int oldValue;
    [SerializeField] private Text fpsCounterText;

    private void OnValidate() {
#if UNITY_EDITOR
        if (fpsCounterText == null) {
            fpsCounterText = gameObject.GetComponent<Text>();
        }
#endif
        return;
    }

    //https://forum.unity.com/threads/fps-counter.505495/
    private void Update() {
        int newValue = (int)(1f / Time.unscaledDeltaTime);
        if (oldValue != newValue) {
            oldValue = newValue;
            fpsCounterText.text = ("FPS : " + newValue + ".");
        }
    }
}