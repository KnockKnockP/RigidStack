using kcp2k;
using Mirror;
using Mirror.Discovery;
using System;
using System.Net;
using UnityEngine;

public class DiscoveryRequest : NetworkMessage {
    //What the client sends.
}

public class DiscoveryResponse : NetworkMessage {
    //What the server sends.
    public int currentPlayerCount, maxPlayerCount;
    public string name;
    public IPEndPoint _IPEndPoint {
        get; set;
    }
    public Uri uri;
}

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse> {
    private multiplayerLobbyScript _multiplayerLobbyScript = null;
    [SerializeField] private KcpTransport kcpTransport = null;

    private void OnValidate() {
#if UNITY_EDITOR
        if (kcpTransport == null) {
            kcpTransport = FindObjectOfType<KcpTransport>();
        }
#endif
        return;
    }

    private void Awake() {
        _multiplayerLobbyScript = FindObjectOfType<multiplayerLobbyScript>();
        return;
    }

    #region Server.
    protected override void ProcessClientRequest(DiscoveryRequest discoveryRequest, IPEndPoint _IPEndPoint) {
        base.ProcessClientRequest(discoveryRequest, _IPEndPoint);
        return;
    }

    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest discoveryRequest, IPEndPoint _IPEndPoint) {
        return new DiscoveryResponse {
            currentPlayerCount = _multiplayerLobbyScript.playerCount,
            maxPlayerCount = NetworkManager.singleton.maxConnections,
            name = _multiplayerLobbyScript.lobbyName,
            uri = kcpTransport.ServerUri()
        };
    }
    #endregion

    #region Client.
    protected override DiscoveryRequest GetRequest() {
        return new DiscoveryRequest();
    }

    protected override void ProcessResponse(DiscoveryResponse discoveryRequest, IPEndPoint _IPEndPoint) {
        discoveryRequest._IPEndPoint = _IPEndPoint;
        UriBuilder realUri = new UriBuilder(discoveryRequest.uri) {
            Host = discoveryRequest._IPEndPoint.Address.ToString()
        };
        discoveryRequest.uri = realUri.Uri;
        _multiplayerLobbyScript.addServerResponce(discoveryRequest);
        return;
    }
    #endregion
}