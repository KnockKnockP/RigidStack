using Mirror;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerScript : MonoBehaviour {
    //A variable for determining the game mode.
    public static bool isMultiplayerGame = false;

    //A variable for setting up the main menu.
    [SerializeField] private GameObject scriptCrammer = null;

    //A variable for managing scenes.
    private static bool hasAddedAnEvent = false;

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
            foreach (NetworkManager networkManager in FindObjectsOfType<NetworkManager>()) {
                Destroy(networkManager.gameObject);
            }
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
            networkManager.GetComponent<TelepathyTransport>().port = GetAvailablePort();
            networkManager.StartHost();
        }
        return;
    }

    //https://gist.github.com/jrusbatch/4211535#gistcomment-3437695
    public static ushort GetAvailablePort() {
        return (ushort)(((IPEndPoint)(new UdpClient(0, AddressFamily.InterNetwork).Client.LocalEndPoint)).Port);
    }
}