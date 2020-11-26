using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenuScript : MonoBehaviour {
    [SerializeField] private Text currentVersionText = null, updateText = null, noticeText = null;
    [SerializeField] private GameObject inGameConsole = null;

    private void Awake() {
        currentVersionText.text = ("v" + Application.version + ".");
        if (SceneManager.GetActiveScene().name == SceneNames.MainMenu) {
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;
            try {
                checkForUpdate();
                checkNoticeText();
            } catch (Exception exception) {
                //We log it as a warning since it's nothing so serious about not being able to check the version.
                Debug.LogWarning(exception.Message);
            }
            inGameConsole.SetActive(Debug.isDebugBuild);
        }
        return;
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
        streamReader.Close();
        stream.Close();
        return;
    }

    private void checkNoticeText() {
        WebClient webClient = new WebClient();
        Stream stream = webClient.OpenRead("https://knockknockp.github.io/RigidStack/notice.txt");
        StreamReader streamReader = new StreamReader(stream);
        noticeText.text = streamReader.ReadToEnd();
        streamReader.Close();
        stream.Close();
        return;
    }
}