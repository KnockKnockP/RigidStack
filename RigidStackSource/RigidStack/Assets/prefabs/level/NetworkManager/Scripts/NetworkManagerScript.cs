using Mirror;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerScript : MonoBehaviour {
    //A variable for determining the game mode.
    public static bool isMultiplayerGame = false;

    //A variable for setting up the main menu.
    private static bool hasAddedAnEvent = false;
    [SerializeField] private GameObject scriptCrammer = null;

    //A variable for finding the available port.
    private static readonly IPEndPoint DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, port: 0);

    private void Awake() {
        if (hasAddedAnEvent == false) {
            SceneManager.sceneLoaded += sceneChanged;
            hasAddedAnEvent = true;
        }
        return;
    }

    private void Start() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == SceneNames.MainMenu) {
            scriptCrammer.SetActive(true);
        } else if (sceneName != SceneNames.Level) {
            scriptCrammer.SetActive(true);
            Destroy(gameObject);
        }
        return;
    }

    private void sceneChanged(Scene scene, LoadSceneMode loadSceneMode) {
        if ((scene.name == SceneNames.Level) && (isMultiplayerGame == false)) {
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager.isNetworkActive == true) {
                networkManager.StopHost();
            }
            networkManager.GetComponent<TelepathyTransport>().port = getAvailablePort();
            networkManager.StartHost();
        }
        return;
    }

    //https://www.stackoverflow.com/a/49408267/
    public static ushort getAvailablePort() {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(DefaultLoopbackEndpoint);
        return (ushort)(((IPEndPoint)socket.LocalEndPoint).Port);
    }
}