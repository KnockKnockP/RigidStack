using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class KickWindowScript : NetworkBehaviour {
    [SerializeField] private Transform scrollTransform = null;
    public static readonly List<PlayerKickTemplateScript> playerKickTemplateScripts = new List<PlayerKickTemplateScript>();
    private GameObject _gameObject;
    [SerializeField] private GameObject template = null;
    private readonly List<GameObject> generatedTemplates = new List<GameObject>();

    private void Awake() {
        if (isClientOnly == true) {
            Destroy(gameObject);
        }
        _gameObject = gameObject;
        return;
    }

    private void OnEnable() {
        if (NetworkManager.singleton.isNetworkActive == false) {
            return;
        }

        playerKickTemplateScripts.Clear();
        foreach (GameObject _template in generatedTemplates) {
            NetworkServer.Destroy(_template);
        }
        generatedTemplates.Clear();

        foreach (string name in NetworkManagerScript.playerNames) {
            if (name != LoadedPlayerData.playerData.name) {
                GameObject clone = Instantiate(template, scrollTransform);
                NetworkServer.Spawn(clone);
                PlayerKickTemplateScript playerKickTemplateScript = clone.GetComponent<PlayerKickTemplateScript>();
                playerKickTemplateScript.parent = _gameObject;
                playerKickTemplateScript.playerName = name;
                clone.SetActive(true);
                generatedTemplates.Add(clone);
            }
        }
        return;
    }

    private void Start() {
        _gameObject.SetActive(false);
        return;
    }

    private void OnDisable() {
        foreach (PlayerKickTemplateScript playerKickTemplateScript in playerKickTemplateScripts) {
            playerKickTemplateScript.Cancel();
        }
        return;
    }
}