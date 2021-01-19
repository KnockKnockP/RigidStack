using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class DebuggerPanelScript : MonoBehaviour {
    [SerializeField] private Text pingText = null;

    private void Update() {
        pingText.text = ((int)(NetworkTime.rtt * 1000)).ToString();
    }
}