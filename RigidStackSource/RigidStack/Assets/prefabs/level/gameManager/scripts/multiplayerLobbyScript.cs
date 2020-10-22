using Mirror;
using Mirror.Discovery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class multiplayerLobbyScript : NetworkBehaviour {
    private class MultiplayerLobby {
        public int index = -1;
        public string serverName = "Unnamed.";
        public ServerResponse serverResponse = null;
    }

    //Variables for maintaining the lobby.
    private bool lobbyStarted;
    private string disconnectedReason = "No reason.";
    private List<string> playerNames = new List<string>();
    private readonly Dictionary<long, MultiplayerLobby> discoveredServers = new Dictionary<long, MultiplayerLobby>();
    private TelepathyTransport telepathyTransport;
    private NetworkManager networkManager;
    private NetworkDiscovery networkDiscovery;

    //Variables for maintaining the lobby user interface.
    [SerializeField] private Text statusText = null, IPText = null;
    [SerializeField] private Button startButton = null, kickButton = null;
    [SerializeField] private Dropdown selectPlayerDropdownMenu = null, serverDropdownMenu = null;
    [SerializeField] private GameObject lobbyPanel = null;

    private void Start() {
        telepathyTransport = FindObjectOfType<TelepathyTransport>();
        networkManager = FindObjectOfType<NetworkManager>();
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        return;
    }

    public void createLobby() {
        telepathyTransport.port = NetworkManagerScript.getAvailablePort();
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
        return;
    }

    #region Updating lobby texts.
    [Command(ignoreAuthority = true)]
    private void commandUpdateTexts(bool isDisconnecting) {
        byte playerCount = getPlayerCount(isDisconnecting);
        if (playerCount > 1) {
            startButton.interactable = true;
        } else {
            startButton.interactable = false;
        }
        clientRPCUpdateTexts(playerCount);
        return;
    }

    [Server]
    private byte getPlayerCount(bool isDisconnecting) {
        byte playerCount = 0;
        for (byte i = 0; i < NetworkServer.connections.Count; i++) {
            playerCount++;
        }
        if (isDisconnecting == true) {
            playerCount--;
        }
        return playerCount;
    }

    [ClientRpc]
    private void clientRPCUpdateTexts(byte joinedPlayers) {
        if (joinedPlayers > 1) {
            statusText.text = ("Ready to start. (" + joinedPlayers + " / " + networkManager.maxConnections + ".).");
        } else {
            statusText.text = ("Waiting for players. (" + joinedPlayers + " / " + networkManager.maxConnections + ".).");
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandUpdatePlayerList(string playerName) {
        //Removing all null names. (Player names that have disconnected.).
        List<string> temp = new List<string>();
        foreach (string name in playerNames) {
            if (name != null) {
                temp.Add(name);
            }
        }
        //Adding the new player name if it is not null.
        if (playerName != null) {
            temp.Add(playerName);
        }
        playerNames.Clear();
        playerNames = temp;
        //List to put it on the dropdown menu. (Excluding the host.).
        List<string> temp2 = new List<string>();
        foreach (string name in temp) {
            if (name != LoadedPlayerData.playerData.name) {
                temp2.Add(name);
            }
        }
        selectPlayerDropdownMenu.ClearOptions();
        selectPlayerDropdownMenu.AddOptions(temp2);
        return;
    }
    #endregion

    #region Joining the multiplayer lobby.
    public void addServerResponce(ServerResponse serverResponse) {
        string name = serverResponse.EndPoint.Address.ToString();
        discoveredServers[serverResponse.serverId] = new MultiplayerLobby() {
            index = discoveredServers.Count,
            serverName = name,
            serverResponse = serverResponse
        };
        List<string> temp = new List<string> {
            name
        };
        serverDropdownMenu.AddOptions(temp);
        return;
    }

    public void refreshServers() {
        serverDropdownMenu.ClearOptions();
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        return;
    }

    public void joinMultiplayerLobby() {
        int index = serverDropdownMenu.value;
        Uri uri = null;
        foreach (MultiplayerLobby multiplayerLobby in discoveredServers.Values) {
            if (multiplayerLobby.index == index) {
                uri = multiplayerLobby.serverResponse.uri;
                break;
            }
        }
        if (uri == null) {
            //TODO : Make an error popup.
            return;
        }
        NetworkIdentity[] networkIdentities = FindObjectsOfType<NetworkIdentity>();
        foreach (NetworkIdentity networkIdentity in networkIdentities) {
            networkIdentity.gameObject.SetActive(false);
        }
        networkManager.StartClient(uri);
        return;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        commandUpdateTexts(false);
        bool enableOrDisable = false;
        if (isServer == true) {
            enableOrDisable = true;
            playerNames.Add(LoadedPlayerData.playerData.name);
        }
        startButton.gameObject.SetActive(enableOrDisable);
        kickButton.gameObject.SetActive(enableOrDisable);
        selectPlayerDropdownMenu.gameObject.SetActive(enableOrDisable);
        if (isClientOnly == true) {
            StartCoroutine(waitForValidationsToEnd());
        }
        IPText.text = networkManager.networkAddress;
        lobbyPanel.SetActive(true);
        return;
    }

    private IEnumerator waitForValidationsToEnd() {
        while (true) {
            if (DummyPlayerScript.loadedAllThings == true) {
                commandNameCheck(LoadedPlayerData.playerData.name, DummyPlayerScript.networkIdentity);
                commandCheckLobbyHasStarted(DummyPlayerScript.networkIdentity);
                commandUpdatePlayerList(LoadedPlayerData.playerData.name);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    [Command(ignoreAuthority = true)]
    public void commandNameCheck(string playerName, NetworkIdentity networkIdentity) {
        foreach (string name in playerNames) {
            if (playerName == name) {
                targetRPCDisconnect(networkIdentity.connectionToClient, "Duplicate name found in the lobby.");
                return;
            }
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandCheckLobbyHasStarted(NetworkIdentity networkIdentity) {
        if (lobbyStarted == true) {
            targetRPCDisconnect(networkIdentity.connectionToClient, "The multiplayer lobby has already started.");
        }
        return;
    }

    [TargetRpc]
    private void targetRPCDisconnect(NetworkConnection networkConnection, string reason) {
        _ = networkConnection;
        disconnectedReason = reason;
        exitMultiplayerLobby();
        lobbyPanel.SetActive(false);
        //TODO : Make an error popup.
        return;
    }
    #endregion

    #region Exiting the multiplayer lobby.
    public void exitMultiplayerLobby() {
        if (isServer == true) {
            cancelMultiplayerLobby();
        } else if (isClientOnly == true) {
            commandUpdateTexts(true);
            commandRemoveName(LoadedPlayerData.playerData.name);
            networkManager.StopClient();
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandRemoveName(string name) {
        for (int i = 0; i < playerNames.Count; i++) {
            if (name == playerNames[i]) {
                playerNames[i] = null;
                break;
            }
        }
        commandUpdatePlayerList(null);
        return;
    }
    #endregion

    #region Cancelling the multiplayer lobby.
    [Server]
    private void cancelMultiplayerLobby() {
        clientRPCCancelMultiplayerLobby();
        networkManager.StopHost();
        lobbyPanel.SetActive(false);
        return;
    }

    [ClientRpc(excludeOwner = true)]
    private void clientRPCCancelMultiplayerLobby() {
        networkManager.StopClient();
        lobbyPanel.SetActive(false);
        return;
    }
    #endregion

    #region Kicking a player.
    [Server]
    public void kickPlayer() {
        getKicked(selectPlayerDropdownMenu.captionText.text);
        return;
    }

    [ClientRpc]
    private void getKicked(string playerNameToKick) {
        if (LoadedPlayerData.playerData.name == playerNameToKick) {
            commandRemoveName(playerNameToKick);
            commandUpdateTexts(true);
            lobbyPanel.SetActive(false);
            disconnectedReason = "Kicked by the host.";
            exitMultiplayerLobby();
        }
        return;
    }
    #endregion

    #region Starting the multiplayer lobby.
    public void startMultiplayer() {
        lobbyStarted = true;
        clientRPCChangeScene();
        networkManager.ServerChangeScene(SceneNames.Level);
        return;
    }

    [ClientRpc]
    private void clientRPCChangeScene() {
        NetworkManagerScript.isMultiplayerGame = true;
        return;
    }
    #endregion
}