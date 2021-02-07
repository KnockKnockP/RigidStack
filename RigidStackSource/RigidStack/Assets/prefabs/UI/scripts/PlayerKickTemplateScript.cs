using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKickTemplateScript : NetworkBehaviour {
    private string _playerName;
    public string playerName {
        get => _playerName;
        set {
            _playerName = value;
            playerText.text = _playerName;
            return;
        }
    }
    [SerializeField] private Text playerText = null;
    private readonly List<dragAndDropScript.PlacedObjectAndPlacer> highlightedObjects = new List<dragAndDropScript.PlacedObjectAndPlacer>();
    [NonSerialized] public GameObject parent;

    private static string selectedPlayer;
    [SerializeField] private GameObject selectButton = null, yesButton = null, noButton = null;

    private void Awake() {
        KickWindowScript.playerKickTemplateScripts.Add(this);
        return;
    }

    public void HighlightPlacedObjects() {
        foreach (PlayerKickTemplateScript playerKickTemplateScript in KickWindowScript.playerKickTemplateScripts) {
            if (playerKickTemplateScript.playerName == selectedPlayer) {
                playerKickTemplateScript.Cancel();
            }
        }
        selectedPlayer = playerName;

        highlightedObjects.Clear();
        foreach (dragAndDropScript.PlacedObjectAndPlacer placedObjectAndPlacer in dragAndDropScript.allPlacedObjects) {
            if (placedObjectAndPlacer.playerName == playerName) {
                if (placedObjectAndPlacer._objectInformation.highlightDelegate == null) {
                    placedObjectAndPlacer._objectInformation.highlightDelegate = placedObjectAndPlacer._objectInformation.Highlight;
                }
                placedObjectAndPlacer._objectInformation.highlightDelegate();
                highlightedObjects.Add(placedObjectAndPlacer);
            }
        }
        return;
    }

    private void UnHighlightPlacedObjects() {
        selectedPlayer = null;
        foreach (dragAndDropScript.PlacedObjectAndPlacer placedObjectAndPlacer in highlightedObjects) {
            if (placedObjectAndPlacer.playerName == playerName) {
                if (placedObjectAndPlacer._objectInformation.unHighlightDelegate == null) {
                    placedObjectAndPlacer._objectInformation.unHighlightDelegate = placedObjectAndPlacer._objectInformation.UnHighlight;
                }
                placedObjectAndPlacer._objectInformation.unHighlightDelegate();
            }
        }
        highlightedObjects.Clear();
        return;
    }

    public void Cancel() {
        selectButton.SetActive(true);
        yesButton.SetActive(false);
        noButton.SetActive(false);
        UnHighlightPlacedObjects();
        return;
    }

    public void Kick() {
        GetKicked(playerName);
        parent.SetActive(false);
        return;
    }

    [ClientRpc]
    private void GetKicked(string name) {
        if (LoadedPlayerData.playerData.name == name) {
            FindObjectOfType<endMenuManager>().toMainMenu();
        }
        return;
    }
}