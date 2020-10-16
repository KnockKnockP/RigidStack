using Mirror;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

//http://www.babbacom.com/?p=311/
namespace System.Net.NetworkInformation {
    class DroidIPGlobalProperties {
        public const string ProcDir = "/proc", CompatProcDir = "/usr/compat/linux/proc";
        public readonly string StatisticsFile, StatisticsFileIPv6, TcpFile, Tcp6File, UdpFile, Udp6File;

        public DroidIPGlobalProperties(string procDir = ProcDir) {
            StatisticsFile = Path.Combine(procDir, "net/snmp");
            StatisticsFileIPv6 = Path.Combine(procDir, "net/snmp6");
            TcpFile = Path.Combine(procDir, "net/tcp");
            Tcp6File = Path.Combine(procDir, "net/tcp6");
            UdpFile = Path.Combine(procDir, "net/udp");
            Udp6File = Path.Combine(procDir, "net/udp6");
        }

        private static readonly char[] wsChars = new char[] {' ', '\t'};

        private Exception CreateException(string file, string msg) {
            return new InvalidOperationException(string.Format("Unsupported (unexpected) '{0}' file format. ", file) + msg);
        }

        private IPEndPoint[] GetLocalAddresses(List<string[]> list) {
            IPEndPoint[] ret = new IPEndPoint[list.Count];
            for (int i = 0; i < ret.Length; i++) {
                ret[i] = ToEndpoint(list[i][1]);
            }
            return ret;
        }

        private IPEndPoint ToEndpoint(string s) {
            int idx = s.IndexOf(':'), port = int.Parse(s.Substring(idx + 1), NumberStyles.HexNumber);
            if (s.Length == 13) {
                return new IPEndPoint(long.Parse(s.Substring(0, idx), NumberStyles.HexNumber), port);
            } else {
                byte[] bytes = new byte[16];
                for (int i = 0; (i << 1) < idx; i++)
                    bytes[i] = byte.Parse(s.Substring(i << 1, 2), NumberStyles.HexNumber);
                return new IPEndPoint(new IPAddress(bytes), port);
            }
        }

        private void GetRows(string file, List<string[]> list) {
            if (!File.Exists(file)) {
                return;
            }
            using (StreamReader sr = new StreamReader(file, Encoding.ASCII)) {
                sr.ReadLine();
                while (!sr.EndOfStream) {
                    string[] item = sr.ReadLine().Split(wsChars, StringSplitOptions.RemoveEmptyEntries);
                    if (item.Length < 4) {
                        throw CreateException(file, null);
                    }
                    list.Add(item);
                }
            }
        }

        public TcpConnectionInformation[] GetActiveTcpConnections() {
            List<string[]> list = new List<string[]>();
            GetRows(TcpFile, list);
            GetRows(Tcp6File, list);

            TcpConnectionInformation[] ret = new TcpConnectionInformation[list.Count];
            for (int i = 0; i < ret.Length; i++) {
                IPEndPoint local = ToEndpoint(list[i][1]);
                IPEndPoint remote = ToEndpoint(list[i][2]);
                TcpState state = (TcpState)int.Parse(list[i][3], NumberStyles.HexNumber);
                ret[i] = new SystemTcpConnectionInformation(local, remote, state);
            }
            return ret;
        }

        private class SystemTcpConnectionInformation : TcpConnectionInformation {
            public override IPEndPoint LocalEndPoint {
                get;
            }
            public override IPEndPoint RemoteEndPoint {
                get;
            }
            public override TcpState State {
                get;
            }

            public SystemTcpConnectionInformation(IPEndPoint local, IPEndPoint remote, TcpState state) {
                LocalEndPoint = local;
                RemoteEndPoint = remote;
                State = state;
            }
        }
    }
}

public class NetworkManagerScript : NetworkBehaviour {
    //Variables for determining the game mode.
    public static bool isMultiplayerGame = false, isCoop = false;

    //Variables for hosting the server.
    private static ushort usablePort;
    private NetworkManager networkManager;

    private void Awake() {
        usablePort = (ushort)(GetAvailablePort(1024));
        Debug.Log("Found usuable port : " + usablePort);
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
    private static int GetAvailablePort(int startingPort) {
        #if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IEnumerable<int> tcpConnectionPorts = ipGlobalProperties.GetActiveTcpConnections()
                                                                                              .Where(n => n.LocalEndPoint.Port >= startingPort)
                                                                                              .Select(n => n.LocalEndPoint.Port);
            IEnumerable<int> tcpListenerPorts = ipGlobalProperties.GetActiveTcpListeners()
                                                                                          .Where(n => n.Port >= startingPort)
                                                                                          .Select(n => n.Port);
            IEnumerable<int> udpListenerPorts = ipGlobalProperties.GetActiveUdpListeners()
                                                                                          .Where(n => n.Port >= startingPort)
                                                                                          .Select(n => n.Port);
            int port = Enumerable.Range(startingPort, ushort.MaxValue)
                                                                      .Where(i => !tcpConnectionPorts.Contains(i))
                                                                      .Where(i => !tcpListenerPorts.Contains(i))
                                                                      .Where(i => !udpListenerPorts.Contains(i))
                                                                      .FirstOrDefault();
            return port;
        #else
            /*
            DroidIPGlobalProperties droidIPGlobalProperties = new DroidIPGlobalProperties();
            TcpConnectionInformation[] tcpConnectionInformations = droidIPGlobalProperties.GetActiveTcpConnections();
            for (int i = startingPort; i <= ushort.MaxValue; i++) {
                bool isUsed = false;
                for (int j = 0; j < tcpConnectionInformations.Length; j++) {
                    if (i == tcpConnectionInformations[j].LocalEndPoint.Port) {
                        isUsed = true;
                        break;
                    }
                }
                if (isUsed == false) {
                    return i;
                }
            }
            return 0;
            */
        #endif
    }
}