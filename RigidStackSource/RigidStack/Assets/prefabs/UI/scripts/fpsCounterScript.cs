using UnityEngine;
using UnityEngine.UI;

public class fpsCounterScript : MonoBehaviour {
    private int oldValue;
    private Text fpsCounterText;

    private void Awake() {
        fpsCounterText = gameObject.GetComponent<Text>();
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