using Mirror;
using System.Collections;
using UnityEngine;

public class DummyPlayerScript : NetworkBehaviour {
    public struct DummyPlayer {
        public NetworkIdentity networkIdentity;
        public int index;
    }

    public class DummyPlayerSyncList : SyncList<DummyPlayer> {}

    //Variables for target rpc functions.
    public DummyPlayerSyncList dummyPlayerSyncList = new DummyPlayerSyncList();
    public static DummyPlayerScript dummyPlayerScript;

    private void Start() {
        if (isServer == true) {
            if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer) {
                dummyPlayerScript = this;
            }
            serverStart(gameObject);
        }
        return;
    }

    [Server]
    private void serverStart(GameObject _gameObject) {
        DummyPlayer dummyPlayer = new DummyPlayer {
            networkIdentity = _gameObject.GetComponent<NetworkIdentity>()
        };
        dummyPlayer.index = dummyPlayerSyncList.Count;
        dummyPlayerSyncList.Add(dummyPlayer);
        if (dummyPlayerScript != _gameObject.GetComponent<DummyPlayerScript>()) {
            clientRPCSuicide(_gameObject);
        }
        targetRPCReturnIndex(dummyPlayer.networkIdentity.connectionToClient, dummyPlayer.index);
        return;
    }

    [ClientRpc]
    private void clientRPCSuicide(GameObject _gameObject) {
        Destroy(_gameObject.GetComponent<DummyPlayerScript>());
        return;
    }

    [TargetRpc]
    private void targetRPCReturnIndex(NetworkConnection networkConnection, int _index) {
        _ = networkConnection;
        dragAndDropScript.index = _index;
        return;
    }
}