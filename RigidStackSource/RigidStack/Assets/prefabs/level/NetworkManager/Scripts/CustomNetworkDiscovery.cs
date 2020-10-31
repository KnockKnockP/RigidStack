using Mirror;
using Mirror.Discovery;
using System;
using System.Net;
using UnityEngine;

/*
    Discovery Guide : https://mirror-networking.com/docs/Guides/NetworkDiscovery.html
    Documentation : https://mirror-networking.com/docs/Components/NetworkDiscovery.html
    API Reference : https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

public class DiscoveryRequest : MessageBase {
    //What the client sends.
}

public class DiscoveryResponse : MessageBase {
    //What the server sends.
    public string name;
    public IPEndPoint _IPEndPoint {
        get; set;
    }
    public Uri uri;
}

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse> {
    [SerializeField] private multiplayerLobbyScript _multiplayerLobbyScript;
    [SerializeField] private TelepathyTransport telepathyTransport;

    #region Server.
    protected override void ProcessClientRequest(DiscoveryRequest discoveryRequest, IPEndPoint _IPEndPoint) {
        base.ProcessClientRequest(discoveryRequest, _IPEndPoint);
        return;
    }

    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest discoveryRequest, IPEndPoint _IPEndPoint) {
        return new DiscoveryResponse {
            name = _multiplayerLobbyScript.lobbyName,
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