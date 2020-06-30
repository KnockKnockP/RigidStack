using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class preMainMenuScript : MonoBehaviour {
    [SerializeField] private Text currentVersionText = null, updateText = null, noticeText = null;

    private void Awake() {
        noticeText.text = "Checking internet.";
        if (checkInternetConnection() == true) {
            noticeText.text = "Internet connected.";
            checkForUpdate();
            checkNoticeText();
        } else {
            noticeText.text = "Internet not connected.";
        }
        return;
    }

    //https://stackoverflow.com/a/2031831.
    public static bool checkInternetConnection() {
        try {
            WebClient webClient = new WebClient();
            webClient.OpenRead("http://google.com/generate_204");
            return true;
        } catch {
            return false;
        }
    }

    private void checkForUpdate() {
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

    private void checkNoticeText() {
        WebClient webClient = new WebClient();
        Stream stream = webClient.OpenRead("https://knockknockp.github.io/RigidStack/notice.txt");
        StreamReader streamReader = new StreamReader(stream);
        noticeText.text = streamReader.ReadToEnd();
        stream.Close();
        streamReader.Close();
        return;
    }
}