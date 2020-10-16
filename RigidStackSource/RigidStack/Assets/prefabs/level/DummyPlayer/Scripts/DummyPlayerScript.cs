using Mirror;
using UnityEngine;

public class DummyPlayerScript : NetworkBehaviour {
    public static NetworkIdentity networkIdentity;
    public static GameObject player;

    private void Start() {
        if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer == true) {
            player = gameObject;
            networkIdentity = player.GetComponent<NetworkIdentity>();
        }
        return;
    }
}