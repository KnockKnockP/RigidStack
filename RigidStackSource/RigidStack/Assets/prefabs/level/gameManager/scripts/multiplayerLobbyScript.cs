/*
    Behold... the new contestant of the new spaghetti code competition!
*/

using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class multiplayerLobbyScript : NetworkBehaviour {
    [NonSerialized, SyncVar(hook = nameof(syncPlayerCount))] public int playerCount;
    private string disconnectedReason = "No reason.";
    [NonSerialized] public string lobbyName = "Unnamed.";
    private readonly List<DiscoveryResponse> discoveredServers = new List<DiscoveryResponse>();
    private KcpTransport kcpTransport;
    private NetworkManager networkManager;
    private CustomNetworkDiscovery customNetworkDiscovery;

    [Header("Select multiplayer panel."), SerializeField] private Text selectMultiplayerText = null;

    [Header("Join multiplayer lobby panel."), SerializeField] private GameObject multiplayerLobbyListScrollViewViewport = null, multiplayerLobbyListTemplate = null;

    [Header("Multiplayer lobby."), SerializeField] private Text statusText = null;
    [SerializeField] private Button startButton = null, kickButton = null;
    [SerializeField] private InputField lobbyNameInputField = null;
    [SerializeField] private Dropdown selectPlayerDropdownMenu = null;
    [SerializeField] private GameObject joinMultiplayerLobbyPanel = null, lobbyPanel = null;

    private void Start() {
        NetworkManagerScript.playerNames.Clear();
        StartCoroutine(waitForSingleton());
        return;
    }

    private IEnumerator waitForSingleton() {
        while (true) {
            if (NetworkManager.singleton != null) {
                networkManager = NetworkManager.singleton;
                kcpTransport = networkManager.GetComponent<KcpTransport>();
                customNetworkDiscovery = networkManager.GetComponent<CustomNetworkDiscovery>();
                yield break;
            }
            yield return null;
        }
    }

    public void createLobby() {
        playerCount = 0;
        kcpTransport.Port = NetworkManagerScript.getAvailablePort();
        lobbyName = (LoadedPlayerData.playerData.name + "'s lobby.");
        networkManager.maxConnections = 5;
        try {
            networkManager.StartHost();
            StartCoroutine(nameof(checkPort));
        } catch (Exception exception) {
            Debug.LogError(exception.Message);
            customNetworkDiscovery.StopDiscovery();
            networkManager.StopHost();
            //TODO : Show error shits.
        }
        return;
    }

    private IEnumerator checkPort() {
        const int timeOut = 5;
        for (int i = timeOut; i >= 0; i--) {
            selectMultiplayerText.text = ("Please wait for " + i + ((i == 1) ? " second" : " seconds") + ".");
            yield return new WaitForSeconds(1);
        }
        if (kcpTransport.ServerActive() == false) {
            Debug.LogWarning("Something went wrong with creation of the multiplayer lobby.\r\n");
            //TODO : Show error shits.
            customNetworkDiscovery.StopDiscovery();
            networkManager.StopHost();
        } else {
            selectMultiplayerText.text = "";
            lobbyPanel.SetActive(true);
            customNetworkDiscovery.AdvertiseServer();
            Debug.Log("Multiplayer lobby created successfully.");
        }
        yield break;
    }

    #region Updating lobby texts.
    [Command(ignoreAuthority = true)]
    private void commandIncrementPlayerCount() {
        playerCount++;
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandDecrementPlayerCount() {
        playerCount--;
        return;
    }

    private void syncPlayerCount(int oldPlayerCount, int newPlayerCount) {
        if (playerCount > 1) {
            startButton.interactable = true;
            statusText.text = ("Ready to start. (" + playerCount + " / " + networkManager.maxConnections + ".).");
        } else {
            startButton.interactable = false;
            statusText.text = ("Waiting for players. (" + playerCount + " / " + networkManager.maxConnections + ".).");
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandUpdatePlayerList(string playerName) {
        //Removing all null names. (Player names that have disconnected.).
        List<string> temp = new List<string>();
        foreach (string name in NetworkManagerScript.playerNames) {
            if (name != null) {
                temp.Add(name);
            }
        }
        //Adding the new player name if it is not null.
        if (playerName != null) {
            temp.Add(playerName);
        }
        NetworkManagerScript.playerNames.Clear();
        NetworkManagerScript.playerNames = temp;
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

    public void renameLobby() {
        lobbyName = lobbyNameInputField.text;
        return;
    }

    #region Fetching servers.
    public void addServerResponce(DiscoveryResponse discoveryResponse) {
        discoveredServers.Add(discoveryResponse);
        MultiplayerLobbyListScript multiplayerLobbyListScript = Instantiate(multiplayerLobbyListTemplate, multiplayerLobbyListScrollViewViewport.transform).GetComponent<MultiplayerLobbyListScript>();
        multiplayerLobbyListScript.index = (discoveredServers.Count - 1);
        multiplayerLobbyListScript.multiplayerLobbyNameText.text = discoveryResponse.name;
        multiplayerLobbyListScript.multiplayerLobbyPlayerCountText.text = (discoveryResponse.currentPlayerCount + " / " + discoveryResponse.maxPlayerCount + ".");
        return;
    }

    public void refreshServers() {
        foreach (Transform _transform in multiplayerLobbyListScrollViewViewport.transform) {
            Destroy(_transform.gameObject);
        }
        discoveredServers.Clear();
        customNetworkDiscovery.StartDiscovery();
        return;
    }
    #endregion

    #region Joining the multiplayer lobby.
    public void joinMultiplayerLobby(int index) {
        NetworkIdentity[] networkIdentities = FindObjectsOfType<NetworkIdentity>();
        foreach (NetworkIdentity networkIdentity in networkIdentities) {
            networkIdentity.gameObject.SetActive(false);
        }
        kcpTransport.Port = (ushort)(discoveredServers[index].uri.Port);
        networkManager.StartClient(discoveredServers[index].uri);
        lobbyPanel.SetActive(true);
        return;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        bool enableOrDisable = false;
        if (isServer == true) {
            enableOrDisable = true;
            NetworkManagerScript.playerNames.Add(LoadedPlayerData.playerData.name);
        }
        startButton.gameObject.SetActive(enableOrDisable);
        kickButton.gameObject.SetActive(enableOrDisable);
        lobbyNameInputField.gameObject.SetActive(enableOrDisable);
        selectPlayerDropdownMenu.gameObject.SetActive(enableOrDisable);
        if (isClientOnly == true) {
            StartCoroutine(waitForValidationsToEnd());
        }
        commandIncrementPlayerCount();
        //We only disable joinMultiplayerLobbyPanel when we successfully connect to the multiplayer lobby.
        joinMultiplayerLobbyPanel.SetActive(false);
        return;
    }

    private IEnumerator waitForValidationsToEnd() {
        while (true) {
            if (DummyPlayerScript.loadedAllThings == true) {
                commandNameCheck(LoadedPlayerData.playerData.name, DummyPlayerScript.networkIdentity);
                commandUpdatePlayerList(LoadedPlayerData.playerData.name);
                yield break;
            }
            yield return null;
        }
    }

    [Command(ignoreAuthority = true)]
    public void commandNameCheck(string playerName, NetworkIdentity networkIdentity) {
        foreach (string name in NetworkManagerScript.playerNames) {
            if (playerName == name) {
                targetRPCDisconnect(networkIdentity.connectionToClient, "Duplicate name found in the lobby.");
                return;
            }
        }
        return;
    }
    #endregion

    #region Exiting the multiplayer lobby.
    [TargetRpc]
    private void targetRPCDisconnect(NetworkConnection networkConnection, string reason) {
        disconnectedReason = reason;
        exitMultiplayerLobby();
        lobbyPanel.SetActive(false);
        Debug.Log("Disconnected from the server.\r\n" +
                  "Reason : " + disconnectedReason);
        //TODO : Make an error popup.
        return;
    }

    public void exitMultiplayerLobby() {
        if (isServer == true) {
            cancelMultiplayerLobby();
        } else if (isClientOnly == true) {
            commandDecrementPlayerCount();
            commandRemoveName(LoadedPlayerData.playerData.name);
            lobbyPanel.SetActive(false);
            networkManager.StopClient();
            refreshServers();
        }
        NetworkManagerScript.activateScriptCrammer();
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandRemoveName(string name) {
        for (int i = 0; i < NetworkManagerScript.playerNames.Count; i++) {
            if (name == NetworkManagerScript.playerNames[i]) {
                NetworkManagerScript.playerNames[i] = null;
                break;
            }
        }
        commandUpdatePlayerList(null);
        return;
    }
    #endregion

    #region Cancelling the multiplayer lobby.
    public void cancelMakingTheMultiplayerLobby() {
        if (isServer == true) {
            actuallyCancelMakingTheMultiplayerLobby();
        }
        return;
    }

    [Server]
    private void actuallyCancelMakingTheMultiplayerLobby() {
        StopCoroutine(nameof(checkPort));
        selectMultiplayerText.text = "";
        cancelMultiplayerLobby();
        return;
    }

    [Server]
    private void cancelMultiplayerLobby() {
        clientRPCCancelMultiplayerLobby();
        StartCoroutine(waitUntilAllClientsDisconnect());
        return;
    }

    [ClientRpc]
    private void clientRPCCancelMultiplayerLobby() {
        if (isServer == true) {
            return;
        }
        exitMultiplayerLobby();
        return;
    }

    [Server]
    private IEnumerator waitUntilAllClientsDisconnect() {
        while (playerCount != 1) {
            /*
                We wait until all players disconnect except for the host.
                We are doing in a coroutine because the server needs to accept all commands.
            */
            yield return null;
        }
        networkManager.StopHost();
        yield break;
    }
    #endregion

    #region Kicking a player.
    [Server]
    public void kickPlayer() {
        getKicked(selectPlayerDropdownMenu.captionText.text);
        return;
    }

    [ClientRpc]
    public void getKicked(string playerNameToKick) {
        if (LoadedPlayerData.playerData.name == playerNameToKick) {
            disconnectedReason = "Kicked by the host.";
            exitMultiplayerLobby();
        }
        return;
    }
    #endregion

    #region Starting the multiplayer lobby.
    public void startMultiplayer() {
        customNetworkDiscovery.StopDiscovery();
        clientRPCChangeScene();
        networkManager.ServerChangeScene(SceneNames.Level);
        return;
    }

    [ClientRpc]
    private void clientRPCChangeScene() {
        NetworkManagerScript.isSingleplayerGame = false;
        NetworkManagerScript.isMultiplayerGame = true;
        return;
    }
    #endregion
}