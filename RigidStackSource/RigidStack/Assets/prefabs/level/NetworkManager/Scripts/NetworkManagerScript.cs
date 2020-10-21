using Mirror;
using System.Net;
using System.Net.NetworkInformation;
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
            networkManager.GetComponent<TelepathyTransport>().port = getAvailablePort();
            networkManager.StartHost();
        }
        return;
    }

    //https://gamedev.stackexchange.com/a/161776/
    public static string getIP() {
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces()) {
            if ((networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) || (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)) {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses) {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork) {
                        string ipString = unicastIPAddressInformation.Address.ToString();
                        if (ipString.StartsWith("192.168")) {
                            return ipString;
                        }
                    }
                }
            }
        }
        return null;
    }

    //https://gist.github.com/jrusbatch/4211535#gistcomment-3437695
    public static ushort getAvailablePort() {
        return (ushort)(((IPEndPoint)(new UdpClient(0, AddressFamily.InterNetwork).Client.LocalEndPoint)).Port);
    }
}