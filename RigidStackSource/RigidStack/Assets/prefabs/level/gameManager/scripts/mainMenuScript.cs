using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuScript : MonoBehaviour {
    //Variables for preparing the pre main menu scene.
    private string exceptionMessage1, exceptionMessage2;
    [SerializeField] private Text currentVersionText = null, updateText = null, noticeText = null;
    [SerializeField] private GameObject inGameConsole = null;

    private void Awake() {
        currentVersionText.text = ("v" + Application.version + ".");
        if (SceneManager.GetActiveScene().name == SceneNames.MainMenu) {
            limitFPS();
            disableDebugging();
            checkInternetConnection();
            if (Debug.isDebugBuild) {
                inGameConsole.SetActive(true);
            } else {
                inGameConsole.SetActive(false);
            }
        }
        return;
    }

    private void limitFPS() {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
#else
        Application.targetFrameRate = (Screen.currentResolution.refreshRate * 2);
#endif
        return;
    }

    private void disableDebugging() {
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        return;
    }

    private void checkInternetConnection() {
        noticeText.text = "Checking internet connection.";
        if (checkInternetConnectability() == true) {
            noticeText.text = "Connected to the internet";
            checkForUpdate();
            checkNoticeText();
        } else {
            noticeText.color = Color.red;
            noticeText.text = ("Failed to connect to the internet (Two attempts.).\r\n" +
                               exceptionMessage1 + ",\r\n" +
                               exceptionMessage2);
        }
        return;
    }

    //https://www.stackoverflow.com/a/2031831/
    private bool checkInternetConnectability() {
        try {
            WebClient webClient = new WebClient();
            webClient.OpenRead("https://www.google.com/generate_204");
            //webClient.OpenRead("https://www.goog975679576947965976567579le.com/6497659859497646756generate_204");
            return true;
        } catch (Exception exception) {
            exceptionMessage1 = exception.Message;
            exceptionMessage1.TrimEnd('.');
        }
        try {
            WebClient webClient = new WebClient();
            webClient.OpenRead("https://www.baidu.com/");
            //webClient.OpenRead("https://www.baidu65765768567596756735636596756978567.co576569759656759675m/");
            return true;
        } catch (Exception exception) {
            exceptionMessage2 = exception.Message;
            if (exceptionMessage2.EndsWith(".") == false) {
                exceptionMessage2 = (exceptionMessage2 + ".");
            }
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
            updateText.color = new Color32(255, 255, 0, 255);
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