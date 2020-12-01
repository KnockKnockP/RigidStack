using kcp2k;
using Mirror;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerScript : MonoBehaviour {
    private static bool hasAddedAnEvent = false;
    public static bool isMultiplayerGame = false;

    //A variable for finding the available port.
    private static readonly IPEndPoint defaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, 0);

    private void Awake() {
        if (hasAddedAnEvent == false) {
            SceneManager.sceneLoaded += sceneChanged;
            hasAddedAnEvent = true;
        }
        return;
    }

    private void Start() {
        activateScriptCrammer();
        return;
    }

    public static void activateScriptCrammer() {
        if (SceneManager.GetActiveScene().name != SceneNames.Level) {
            foreach (Transform _transform in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (_transform.gameObject.name == "scriptCrammer") {
                    _transform.gameObject.SetActive(true);
                }
            }
        }
        return;
    }

    private void sceneChanged(Scene scene, LoadSceneMode loadSceneMode) {
        if ((scene.name == SceneNames.Level) && (isMultiplayerGame == false)) {
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager.isNetworkActive == true) {
                networkManager.StopHost();
            }
            networkManager.GetComponent<NetworkManager>().maxConnections = 1;
            networkManager.GetComponent<KcpTransport>().Port = getAvailablePort();
            networkManager.StartHost();
        }
        return;
    }

    //https://www.stackoverflow.com/a/49408267/
    public static ushort getAvailablePort() {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(defaultLoopbackEndpoint);
        return (ushort)(((IPEndPoint)socket.LocalEndPoint).Port);
    }
}