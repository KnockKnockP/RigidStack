using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class multiplayerLobbyScript : NetworkBehaviour {
    //Variables for maintaining the lobby.
    private ushort usuablePort;
    private TelepathyTransport telepathyTransport;
    private NetworkManager networkManager;

    //Variables for maintaining the lobby user interface.
    [SerializeField] private Text roomCodeText = null, statusText = null;
    [SerializeField] private Button startButton = null;
    [SerializeField] private GameObject lobbyPanel = null;

    private void Awake() {
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
    #endregion

    #region Joining the multiplayer lobby.
    public void joinMultiplayerLobby(InputField inputField) {
        usuablePort = ushort.Parse(inputField.text);
        telepathyTransport.port = usuablePort;
        networkManager.StartClient();
        return;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        commandUpdateTexts(false);
        if (isServer) {
            startButton.gameObject.SetActive(true);
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