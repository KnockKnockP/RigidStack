using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class multiplayerLobbyScript : NetworkBehaviour {
    //Variables for maintaining the lobby.
    private ushort usuablePort;
    private List<string> playerNames = new List<string>();
    private TelepathyTransport telepathyTransport;
    private NetworkManager networkManager;

    //Variables for maintaining the lobby user interface.
    [SerializeField] private Text roomCodeText = null, statusText = null;
    [SerializeField] private Button startButton = null, kickButton = null;
    [SerializeField] private Dropdown selectPlayerDropdownMenu = null;
    [SerializeField] private GameObject lobbyPanel = null;

    private void Start() {
        telepathyTransport = FindObjectOfType<TelepathyTransport>();
        networkManager = FindObjectOfType<NetworkManager>();
        return;
    }

    public void createLobby() {
        ushort usablePort = NetworkManagerScript.GetAvailablePort();
        telepathyTransport.port = usablePort;
        networkManager.StartHost();
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
        updateTexts(joinedPlayers);
        return;
    }

    private void updateTexts(byte joinedPlayers) {
        roomCodeText.text = ("Room code : " + telepathyTransport.port);
        if (joinedPlayers > 1) {
            statusText.text = ("Ready to start. (" + joinedPlayers + " / " + networkManager.maxConnections + ".).");
        } else {
            statusText.text = ("Waiting for players. (" + joinedPlayers + " / " + networkManager.maxConnections + ".).");
        }
        return;
    }

    [Command(ignoreAuthority = true)]
    private void commandUpdatePlayerList(string playerName) {
        List<string> temp = new List<string>();
        foreach (string _playerName in playerNames) {
            if (_playerName != null) {
                temp.Add(_playerName);
            }
        }
        playerNames = temp;
        if (playerName != null) {
            playerNames.Add(playerName);
        }
        selectPlayerDropdownMenu.ClearOptions();
        selectPlayerDropdownMenu.AddOptions(playerNames);
        return;
    }
    #endregion

    #region Joining the multiplayer lobby.
    public void joinMultiplayerLobby(InputField inputField) {
        usuablePort = ushort.Parse(inputField.text);
        telepathyTransport.port = usuablePort;
        NetworkIdentity[] networkIdentities = FindObjectsOfType<NetworkIdentity>();
        foreach (NetworkIdentity networkIdentity in networkIdentities) {
            networkIdentity.gameObject.SetActive(false);
        }
        networkManager.StartClient();
        return;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        commandUpdateTexts(false);
        if (isServer == true) {
            startButton.gameObject.SetActive(true);
            kickButton.gameObject.SetActive(true);
            selectPlayerDropdownMenu.gameObject.SetActive(true);
        } else if (isClientOnly == true) {
            commandUpdatePlayerList(LoadedPlayerData.playerData.name);
        }
        lobbyPanel.SetActive(true);
        return;
    }
    #endregion

    #region Exiting the joined multiplayer lobby.
    public void exitMultiplayerLobby() {
        if (isServer == true) {
            commandCancelMultiplayerLobby();
        } else if (isClientOnly == true) {
            commandUpdateTexts(true);
            networkManager.StopClient();
        }
        return;
    }
    #endregion

    #region Cancelling the multiplayer lobby.
    [Command(ignoreAuthority = true)]
    private void commandCancelMultiplayerLobby() {
        clientRPCCancelMultiplayerLobby();
        networkManager.StopHost();
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
        getKicked(playerNames[selectPlayerDropdownMenu.value]);
        return;
    }

    [ClientRpc]
    private void getKicked(string playerNameToKick) {
        if (LoadedPlayerData.playerData.name == playerNameToKick) {
            commandRemoveName(playerNameToKick);
            commandUpdateTexts(true);
            lobbyPanel.SetActive(false);
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

    #region Starting the multiplayer lobby.
    public void startMultiplayer() {
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