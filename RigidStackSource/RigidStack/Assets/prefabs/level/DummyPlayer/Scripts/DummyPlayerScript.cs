using Mirror;
using UnityEngine;

public class DummyPlayerScript : NetworkBehaviour {
    public static bool didNameChecking = false;
    public static DummyPlayerScript dummyPlayerScript = null;
    public static NetworkIdentity networkIdentity;
    public static GameObject player;

    private void Start() {
        if (dummyPlayerScript == null) {
            dummyPlayerScript = this;
        }
        NetworkIdentity _networkIdentity = GetComponent<NetworkIdentity>();
        if (_networkIdentity.isLocalPlayer == true) {
            player = gameObject;
            networkIdentity = _networkIdentity;
            player.name = (LoadedPlayerData.playerData.name + "'s player.");
            if (isClientOnly == true) {
                FindObjectOfType<multiplayerLobbyScript>().commandNameCheck(LoadedPlayerData.playerData.name, networkIdentity);
                didNameChecking = true;
            }
        }
        DontDestroyOnLoad(gameObject);
        return;
    }
}