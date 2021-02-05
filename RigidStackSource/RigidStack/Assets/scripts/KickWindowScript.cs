using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class KickWindowScript : NetworkBehaviour {
    [SerializeField] private Transform scrollTransform = null;
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

    private void Start() {
        _gameObject.SetActive(false);
        return;
    }

    private void OnEnable() {
        if (NetworkManager.singleton.isNetworkActive == false) {
            return;
        }

#if UNITY_EDITOR
        if (NetworkManagerScript.playerNames.Count == 0) {
            NetworkManagerScript.playerNames.Add("Profile 1.");
            NetworkManagerScript.playerNames.Add("Profile 2.");
        }
#endif
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
}