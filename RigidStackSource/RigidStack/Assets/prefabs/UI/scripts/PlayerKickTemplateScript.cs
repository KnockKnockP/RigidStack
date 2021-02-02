using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKickTemplateScript : NetworkBehaviour {
    private string _playerName;
    public string playerName {
        get => _playerName;
        set {
            _playerName = value;
            playerText.text = _playerName;
            return;
        }
    }
    [SerializeField] private Text playerText = null;
    [NonSerialized] public GameObject parent;

    public void Kick() {
        GetKicked(playerName);
        parent.SetActive(false);
        return;
    }

    [ClientRpc]
    private void GetKicked(string name) {
        if (LoadedPlayerData.playerData.name == name) {
            FindObjectOfType<endMenuManager>().toMainMenu();
        }
        return;
    }
}