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

    private void OnEnable() {
#if UNITY_EDITOR
        if (NetworkManagerScript.playerNames.Count == 0) {
            NetworkManagerScript.playerNames.Add("Profile");
        }
#endif

        foreach (GameObject _template in generatedTemplates) {
            Destroy(_template);
        }
        generatedTemplates.Clear();

        foreach (string name in NetworkManagerScript.playerNames) {
            if (name != LoadedPlayerData.playerData.name) {
                GameObject clone = Instantiate(template, scrollTransform);
                PlayerKickTemplateScript playerKickTemplateScript = clone.GetComponent<PlayerKickTemplateScript>();
                playerKickTemplateScript.parent = _gameObject;
                playerKickTemplateScript.playerName = name;
                NetworkServer.Spawn(clone);
                clone.SetActive(true);
                generatedTemplates.Add(clone);
            }
        }
        return;
    }
}