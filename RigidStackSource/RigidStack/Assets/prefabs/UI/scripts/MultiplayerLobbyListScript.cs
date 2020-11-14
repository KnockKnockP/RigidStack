using System;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobbyListScript : MonoBehaviour {
    [NonSerialized] public int index;
    public Text multiplayerLobbyNameText, multiplayerLobbyPlayerCountText;

    public void joinMultiplayerLobby() {
        FindObjectOfType<multiplayerLobbyScript>().joinMultiplayerLobby(index);
        return;
    }
}