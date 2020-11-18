using UnityEngine;

public class settingsMenuScript : MonoBehaviour {
    private settingsScript _settingsScript;

    private void OnEnable() {
        if (_settingsScript == null) {
            _settingsScript = FindObjectOfType<settingsScript>();
        }
        _settingsScript.updateAll();
        return;
    }
}