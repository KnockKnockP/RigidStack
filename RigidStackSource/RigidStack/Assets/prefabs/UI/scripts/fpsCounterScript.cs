using UnityEngine;
using UnityEngine.UI;

public class fpsCounterScript : MonoBehaviour {
    private Text fpsCounterText;

    private void Awake() {
        fpsCounterText = gameObject.GetComponent<Text>();
        return;
    }

    //https://forum.unity.com/threads/fps-counter.505495/
    private void Update() {
        fpsCounterText.text = ("FPS : " + (int)(1f / Time.unscaledDeltaTime) + ".");
    }
}