using UnityEngine;

public class NetworkManagerScript : MonoBehaviour {
    //A variable for enabling and disabling the actual network manager.
    [SerializeField] private bool isParentObject = false;
    [SerializeField] private GameObject actualNetworkManager = null;

    //Variables for determining the game mode.
    public static bool isMultiplayerGame = false, isCoop = false;

    private void Awake() {
        if (isParentObject == true) {
            actualNetworkManager.SetActive(isMultiplayerGame);
            Destroy(this);
        } else {
            Debug.Log("Hello, world!");
        }
        return;
    }
}