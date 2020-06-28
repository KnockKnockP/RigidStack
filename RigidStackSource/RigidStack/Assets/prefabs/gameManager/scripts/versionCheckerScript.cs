using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class versionCheckerScript : MonoBehaviour {
    [SerializeField] private Text currentVersionText = null, updateText = null;

    private void Awake() {
        string currentVersion = currentVersionText.text, newVersion;
        WebClient webClient = new WebClient();
        Stream stream = webClient.OpenRead("https://knockknockp.github.io/RigidStack/latestVersion.txt");
        StreamReader streamReader = new StreamReader(stream);
        newVersion = streamReader.ReadToEnd();
        if (currentVersion != newVersion) {
            updateText.text = "New update avaliable!\r\n" +
                              "Current version : " + currentVersion + "\r\n" +
                              "New version : " + newVersion;
            updateText.gameObject.SetActive(true);
        }
        stream.Close();
        streamReader.Close();
        return;
    }
}