using Mirror;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerScript : MonoBehaviour {
    //A variable for determining the game mode.
    public static bool isMultiplayerGame = false;

    private void Awake() {
        SceneManager.sceneLoaded += sceneChanged;
        return;
    }

    private void sceneChanged(Scene scene, LoadSceneMode loadSceneMode) {
        if ((scene.name == SceneNames.Level) && (isMultiplayerGame == false)) {
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager.isNetworkActive == true) {
                /*
                    Should I stop the host or the client?
                    I have no fucking idea.
                 */
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