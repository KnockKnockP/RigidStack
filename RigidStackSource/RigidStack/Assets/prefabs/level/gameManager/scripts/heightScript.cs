using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : NetworkBehaviour {
    [Header("Variables for height checking.")]
    private bool _isServer;
    private readonly byte maxFrameChecking = 5;
    private byte frameCount;
    [NonSerialized] public float tolerance = 0.1f;
    /*
        PlayerData.maxHeight is the maximum score for the account's life time.
        currentGameMaxHeight is the maximum score for the single session of the game.
        currentScore is the current score that is displayed on the text.
        When the player dies, currentGameMaxHeight gets resetted.

        It looks like I marked this variable as static just to make it easier to access it from other scripts.
        Now I will have to manually reset this stupid ass fucking value.
    */
    [NonSerialized, SyncVar(hook = nameof(syncScore))] public int currentScore;
    [NonSerialized] public int currentGameMaxHeight;
    [SerializeField] private objectScript _objectScript;

    private GameObject previousFrameGameObject;
    [NonSerialized] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [NonSerialized] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    [NonSerialized] public List<GameObject> placedObjects = new List<GameObject>();

    [Header("Variables for showing the height."), SerializeField] private objectiveScript _objectiveScript;
    [SerializeField] private Text heightText = null;

    [Header("A variable for manual height checking."), SerializeField] private Button checkHeightButton = null;

    public void syncDifficulty(Difficulty syncedDifficulty) {
        //Difficulty is synced on objectiveScript.cs.
        switch (syncedDifficulty) {
            case (Difficulty.Sandbox): {
                tolerance = 0.1f;
                break;
            }
            case (Difficulty.Easy): {
                tolerance = 0.01f;
                break;
            }
            case (Difficulty.Moderate): {
                tolerance = 0.001f;
                break;
            }
            case (Difficulty.Difficult): {
                tolerance = 0.0001f;
                break;
            }
            case (Difficulty.Extreme): {
                tolerance = 0f;
                break;
            }
        }
        _isServer = isServer;
        checkHeightButton.gameObject.SetActive(LoadedPlayerData.playerData.isManualCheckingEnabled && _isServer);
        return;
    }

    private void Update() {
        if ((LoadedPlayerData.playerData.isManualCheckingEnabled == false) && (_isServer == true)) {
            checkHeight();
        }
    }

    [Server]
    public void manuallyCheckHeight() {
        frameCount = 0;
        for (byte i = 0; i < maxFrameChecking; i++) {
            checkHeight();
        }
        return;
    }

    [Server]
    public void checkHeight() {
        //yPosition <= currentFrameMaxHeight <= currentScore <= currentGameMaxHeight <= PlayerData.maxHeight.
        if (endMenuManager.isGameEnded == true) {
            return;
        }
        const int lowValue = -9999;
        int currentFrameMaxHeight = lowValue;
        GameObject maxHeightGameObject = null;
        int i = 0;
        //Getting the highest height and it's game object.
        for (; i < placedObjects.Count; i++) {
            if (checkValues(i) == true) {
                int yPosition = (int)(placedObjectsTransforms[i].position.y);
                if (yPosition > currentFrameMaxHeight) {
                    currentFrameMaxHeight = yPosition;
                    maxHeightGameObject = placedObjects[i];
                }
            }
        }
        //We check if it stays like that for maxFrameChecking frames.
        if (currentFrameMaxHeight != lowValue) {
            frameCount++;
            if (frameCount == 1) {
                previousFrameGameObject = maxHeightGameObject;
                return;
            } else if ((frameCount == maxFrameChecking) && (previousFrameGameObject == maxHeightGameObject)) {
                currentScore = currentFrameMaxHeight;
                if (currentScore > currentGameMaxHeight) {
                    currentGameMaxHeight = currentScore;
                }
                if (currentScore > LoadedPlayerData.playerData.maxHeight) {
                    LoadedPlayerData.playerData.maxHeight = currentScore;
                }
                if (currentFrameMaxHeight >= _objectiveScript.objectiveScore) {
                    _objectiveScript.clientRPCGenerateObjective(false);
                    if (isServer == true) {
                        clientRPCDisableObjectEditingPanel();
                        _objectScript.giveMoreItems();
                    }
                }
                frameCount = 0;
            } else if (previousFrameGameObject != maxHeightGameObject) {
                frameCount = 0;
            }
        }
        return;
    }

    [ClientRpc]
    private void clientRPCDisableObjectEditingPanel() {
        dragAndDropScript.staticDisableObjectEditingPanel();
        return;
    }

    private void syncScore(int oldScore, int newScore) {
        updateHeightText();
        return;
    }

    public void updateHeightText() {
        heightText.text = ("Score : " + currentScore + " / " + _objectiveScript.objectiveScore + ".");
        return;
    }

    private bool checkValues(int i) {
        Vector2 velocity = placedObjectsRigidbody2D[i].velocity;
        return ((StaticClass.isInBetweenOrEqualToTwoValues(velocity, -tolerance, tolerance)) || (velocity == Vector2.zero));
    }

    public void resetLists() {
        Rigidbody2D tempRigidbody2D = placedObjectsRigidbody2D[(placedObjectsRigidbody2D.Count - 1)];
        if (tempRigidbody2D != null) {
            tempRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        for (int i = 0; i < (placedObjectsRigidbody2D.Count - 1); i++) {
            Destroy(placedObjectsRigidbody2D[i]);
        }

        List<Transform> tempTransforms = new List<Transform> {
            placedObjectsTransforms[(placedObjectsTransforms.Count - 1)]
        };
        placedObjectsTransforms = tempTransforms;

        List<Rigidbody2D> tempRigidbody2Ds = new List<Rigidbody2D> {
            tempRigidbody2D
        };
        placedObjectsRigidbody2D = tempRigidbody2Ds;

        List<GameObject> tempGameObjects = new List<GameObject> {
            placedObjects[(placedObjects.Count - 1)]
        };
        placedObjects = tempGameObjects;
        return;
    }
}