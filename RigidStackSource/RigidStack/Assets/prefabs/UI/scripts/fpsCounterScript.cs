#region Using tags.
using UnityEngine;
using UnityEngine.UI;
#endregion

#region "MonoBehaviour" inherited "fpsCounterScript" class.
public class fpsCounterScript : MonoBehaviour {
    #region A variable for shwoing the fps.
    private Text fpsCounterText = null;
    #endregion

    #region Awake function.
    private void Awake() {
        fpsCounterText = gameObject.GetComponent<Text>();
        return;
    }
    #endregion

    #region Update function.
    //https://forum.unity.com/threads/fps-counter.505495/
    private void Update() {
        fpsCounterText.text = ("FPS : " + (int)(1f / Time.unscaledDeltaTime) + ".");
    }
    #endregion
}
#endregion