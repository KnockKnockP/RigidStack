using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class NetworkManagerScript : MonoBehaviour {
    //Variables for determining the game mode.
    public static bool isMultiplayerGame = false, isCoop = false;

    //Variables for hosting the server.
    private static ushort usablePort;
    private NetworkManager networkManager;

    private void Awake() {
        usablePort = (ushort)(GetAvailablePort(1024));

        usablePort = 1024; Debug.LogWarning("Remove or comment this line out.");

        GetComponent<TelepathyTransport>().port = usablePort;
        networkManager = FindObjectOfType<NetworkManager>();
        if (isMultiplayerGame == true) {
        } else {
            GetComponent<NetworkManagerHUD>().enabled = false;
            networkManager.StartHost();
        }
        return;
    }

    private void OnDestroy() {
        stopServer();
        return;
    }

    private void OnApplicationQuit() {
        stopServer();
        return;
    }

    private void stopServer() {
        if ((networkManager != null) && (networkManager.isNetworkActive == true)) {
           networkManager.StopHost();
       }
       return;
    }

    //https://gist.github.com/jrusbatch/4211535#gistcomment-2754532/
    private int GetAvailablePort(int startingPort) {
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IEnumerable<int> tcpConnectionPorts = properties.GetActiveTcpConnections()
                                                                                  .Where(n => n.LocalEndPoint.Port >= startingPort)
                                                                                  .Select(n => n.LocalEndPoint.Port);
        IEnumerable<int> tcpListenerPorts = properties.GetActiveTcpListeners()
                                                                              .Where(n => n.Port >= startingPort)
                                                                              .Select(n => n.Port);
        IEnumerable<int> udpListenerPorts = properties.GetActiveUdpListeners()
                                                                              .Where(n => n.Port >= startingPort)
                                                                              .Select(n => n.Port);
        int port = Enumerable.Range(startingPort, ushort.MaxValue)
                                                                  .Where(i => !tcpConnectionPorts.Contains(i))
                                                                  .Where(i => !tcpListenerPorts.Contains(i))
                                                                  .Where(i => !udpListenerPorts.Contains(i))
                                                                  .FirstOrDefault();
        return port;
    }
}