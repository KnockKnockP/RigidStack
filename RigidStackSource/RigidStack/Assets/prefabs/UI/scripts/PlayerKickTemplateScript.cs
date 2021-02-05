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

    public void HighlightPlacedObjects() {
        highlightedObjects.Clear();
        Debug.LogWarning("Here 1.");
        foreach (dragAndDropScript.PlacedObjectAndPlacer placedObjectAndPlacer in FindObjectOfType<dragAndDropScript>().allPlacedObjects) {
            if (placedObjectAndPlacer.playerName == playerName) {
                Debug.LogWarning("Here 2.");
                if (placedObjectAndPlacer._objectInformation.highlightDelegate == null) {
                    Debug.LogWarning("Here 3.");
                    placedObjectAndPlacer._objectInformation.highlightDelegate = placedObjectAndPlacer._objectInformation.Highlight;
                }
                placedObjectAndPlacer._objectInformation.highlightDelegate();
                highlightedObjects.Add(placedObjectAndPlacer);
            }
        }
        return;
    }

    public void UnHighlightPlacedObjects() {
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