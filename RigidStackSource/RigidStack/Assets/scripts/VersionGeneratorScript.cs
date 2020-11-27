#if UNITY_EDITOR
using System;
using System.Globalization;
using UnityEditor;
#endif
using UnityEngine;

public class VersionGeneratorScript : MonoBehaviour {
    public bool isDevelopmentBuild, upgradeMajor, upgradeMinor, upgradePatch;
}

#if UNITY_EDITOR
[CustomEditor(typeof(VersionGeneratorScript))]
public class VersionGeneratorScriptOverride : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate new version number.") == true) {
            generateNewVersionNumber();
        }
        return;
    }

    private void generateNewVersionNumber() {
        string newVersion = Application.version;
        VersionGeneratorScript versionGeneratorScript = (VersionGeneratorScript)(target);
        if (versionGeneratorScript.isDevelopmentBuild == true) {
            newVersion = ("TESTBUILD-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "T" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture) + "+0900");
        } else {
            string[] split = newVersion.Split('.');
            string majorVersion = split[0], minorVersion = split[1], patchVersion = split[2];
            if (versionGeneratorScript.upgradeMajor == true) {
                majorVersion = (int.Parse(majorVersion) + 1).ToString();
            }
            if (versionGeneratorScript.upgradeMinor == true) {
                minorVersion = (int.Parse(minorVersion) + 1).ToString();
            }
            if (versionGeneratorScript.upgradePatch == true) {
                patchVersion = (int.Parse(patchVersion) + 1).ToString();
            }
            newVersion = (majorVersion + "." + minorVersion + "." + patchVersion);
        }
        int androidBundleVersionCode = PlayerSettings.Android.bundleVersionCode;
        Debug.Log("New version : " + newVersion + "\r\n" +
                  "Android .NET bundle version : " + ((versionGeneratorScript.isDevelopmentBuild == true) ? androidBundleVersionCode : ++androidBundleVersionCode) + "\r\n" +
                  "Android IL2CPP bundle version : " + ((versionGeneratorScript.isDevelopmentBuild == true) ? androidBundleVersionCode : ++androidBundleVersionCode));
        Debug.LogError("Do not forget to enter password.");
        GC.Collect();
        return;
    }
}
#endif