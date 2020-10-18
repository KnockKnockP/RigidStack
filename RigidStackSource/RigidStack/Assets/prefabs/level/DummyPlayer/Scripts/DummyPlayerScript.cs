using Mirror;
using UnityEngine;

public class DummyPlayerScript : NetworkBehaviour {
    public static NetworkIdentity networkIdentity;
    public static GameObject player;

    private void Start() {
        NetworkIdentity _networkIdentity = GetComponent<NetworkIdentity>();
        if (_networkIdentity.isLocalPlayer == true) {
            player = gameObject;
            networkIdentity = _networkIdentity;
            gameObject.name = (LoadedPlayerData.playerData.name + "'s player.");
        }
        DontDestroyOnLoad(gameObject);
        return;
    }
}