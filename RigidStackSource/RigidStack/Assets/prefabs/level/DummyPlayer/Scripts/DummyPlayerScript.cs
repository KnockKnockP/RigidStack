using Mirror;
using UnityEngine;

public class DummyPlayerScript : NetworkBehaviour {
    public static bool loadedAllThings = false;
    public static DummyPlayerScript dummyPlayerScript = null;
    public static NetworkIdentity networkIdentity;
    public static GameObject player;

    private void Start() {
        loadedAllThings = false;
        DontDestroyOnLoad(gameObject);
        NetworkIdentity _networkIdentity = GetComponent<NetworkIdentity>();
        if (_networkIdentity.isLocalPlayer == true) {
            dummyPlayerScript = this;
            player = gameObject;
            player.name = LoadedPlayerData.playerData.name;
            networkIdentity = _networkIdentity;
            loadedAllThings = true;
        }
        return;
    }

    public override void OnStopClient() {
        base.OnStopClient();
        loadedAllThings = false;
        return;
    }
}