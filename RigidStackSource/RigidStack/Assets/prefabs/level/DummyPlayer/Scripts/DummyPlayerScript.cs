using Mirror;
using UnityEngine;

public class DummyPlayerScript : NetworkBehaviour {
    public static bool loadedAllThings = false;
    public static DummyPlayerScript dummyPlayerScript = null;
    public static NetworkIdentity networkIdentity;
    public static GameObject player;

    private void Awake() {
        loadedAllThings = false;
        return;
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
        NetworkIdentity _networkIdentity = GetComponent<NetworkIdentity>();
        if (_networkIdentity.isLocalPlayer == true) {
            dummyPlayerScript = this;
            player = gameObject;
            player.name = (LoadedPlayerData.playerData.name + "'s player.");
            networkIdentity = _networkIdentity;
            loadedAllThings = true;
        }
        return;
    }
}