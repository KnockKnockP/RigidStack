using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class preMainMenuScript : MonoBehaviour {
    private string exceptionMessage;
    [SerializeField] private Text currentVersionText = null, updateText = null, noticeText = null;

    private void Awake() {
        limitFPS();
        noticeText.text = "Checking internet connection.";
        if (checkInternetConnection() == true) {
            noticeText.text = "Connected to the internet";
            checkForUpdate();
            checkNoticeText();
        } else {
            noticeText.color = Color.red;
            noticeText.text = "Not connected to the internet. (" + exceptionMessage + ".).";
        }
        return;
    }

    private void limitFPS() {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        return;
    }

    //https://stackoverflow.com/a/2031831.
    public bool checkInternetConnection() {
        try {
            WebClient webClient = new WebClient();
            webClient.OpenRead("http://google.com/generate_204");
            return true;
        } catch (Exception exception) {
            exceptionMessage = exception.Message;
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