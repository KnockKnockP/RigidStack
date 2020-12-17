//using kcp2k;
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
    [SerializeField] private multiplayerLobbyScript _multiplayerLobbyScript;
    //[SerializeField] private KcpTransport kcpTransport = null;
    [SerializeField, Obsolete] private TelepathyTransport telepathyTransport = null;

    [Obsolete]
    private void OnValidate() {
#if UNITY_EDITOR
        if (_multiplayerLobbyScript == null) {
            _multiplayerLobbyScript = FindObjectOfType<multiplayerLobbyScript>();
        }
        /*
        if (kcpTransport == null) {
            kcpTransport = FindObjectOfType<KcpTransport>();
        }
        */
        if (telepathyTransport == null) {
            telepathyTransport = FindObjectOfType<TelepathyTransport>();
        }
#endif
        return;
    }

    #region Server.
    [Obsolete]
    protected override void ProcessClientRequest(DiscoveryRequest discoveryRequest, IPEndPoint _IPEndPoint) {
        base.ProcessClientRequest(discoveryRequest, _IPEndPoint);
        return;
    }

    [Obsolete]
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest discoveryRequest, IPEndPoint _IPEndPoint) {
        return new DiscoveryResponse {
            currentPlayerCount = _multiplayerLobbyScript.playerCount,
            maxPlayerCount = NetworkManager.singleton.maxConnections,
            name = _multiplayerLobbyScript.lobbyName,
            //uri = kcpTransport.ServerUri()
            uri = telepathyTransport.ServerUri()
        };
    }
    #endregion

    #region Client.
    protected override DiscoveryRequest GetRequest() {
        return new DiscoveryRequest();
    }

    protected override void ProcessResponse(DiscoveryResponse discoveryRequest, IPEndPoint __IPEndPoint) {
        discoveryRequest._IPEndPoint = __IPEndPoint;
        UriBuilder realUri = new UriBuilder(discoveryRequest.uri) {
            Host = discoveryRequest._IPEndPoint.Address.ToString()
        };
        discoveryRequest.uri = realUri.Uri;
        _multiplayerLobbyScript.addServerResponce(discoveryRequest);
        return;
    }
    #endregion
}