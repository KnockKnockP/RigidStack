/*
    I fucking hate working on this script and I want to stop working on it.
    Hopefully this is the last time I would ever touch this script and see no bugs from it.
    2020-10-08 02:10 PM.

    Well, fuck.
    I need to touch this stupid script again to implement multiplayer.
    This is going to take some while to get it right.
    2020-10-10 05:47 PM.

    Fucking hell.
    This motherfucking script just wont fucking stupid ass sucking work for fucks sake.
    How many times do I have to stare at this stupid script just to make this motherfucking simple height checking working?
    This is madness, there are fuck ton of bugs that I need to fix because it does not happen in the editor.
    Every time I try to debug this shit it magically fixes it self.
    Jesus fucking chirst.
    This script is fucking cursed.
    2020-10-11 03:31 PM.
*/

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : NetworkBehaviour {
    //Variables for height counting.
    private readonly byte maxFrameChecking = 5;
    private byte frameCount;
    [NonSerialized] public float tolerance = 0.1f;
    /*
        PlayerData.maxHeight is the maximum score for the account's life time, it is also equal to the old objective score.
        currentGameMaxHeight is the maximum score for the single session of the game.
        When the player dies, currentGameMaxHeight gets resetted.

        It looks like I marked this variable as static just to make it easier to access it from other scripts.
        Now I will have to manually reset this stupid ass fucking value.
    */
    [NonSerialized] public int currentGameMaxHeight = 0;
    private objectScript _objectScript;

    //Variables for tracking placed objects.
    private GameObject previousFrameGameObject;
    [NonSerialized] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [NonSerialized] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    [NonSerialized] public List<GameObject> placedObjects = new List<GameObject>();

    //Variables for showing the height.
    private objectiveScript _objectiveScript;
    [SerializeField] private Text heightText = null;

    //A variable for manual height checking.
    [SerializeField] private Button checkHeightButton = null;

    private void Start() {
        _objectScript = FindObjectOfType<objectScript>();
        _objectiveScript = FindObjectOfType<objectiveScript>();
        switch (LoadedPlayerData.playerData.difficulty) {
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
        checkHeightButton.gameObject.SetActive(LoadedPlayerData.playerData.isManualCheckingEnabled);
        if (isServer == true) {
            Invoke(nameof(updateScoreTextOnStart), 1f);
        }
        if ((LoadedPlayerData.playerData.isManualCheckingEnabled == false) && (isServer == true)) {
            StartCoroutine(updateHeight());
        }
        if (isClientOnly == true) {
            checkHeightButton.gameObject.SetActive(false);
        }
        return;
    }

    [Server]
    private void updateScoreTextOnStart() {
        heightText.text = ("Score : 0 / " + _objectiveScript.objectiveScore + ".");
        return;
    }

    private IEnumerator updateHeight() {
        while (true) {
            checkHeight();
            yield return null;
        }
    }

    public void manuallyCheckHeight() {
        frameCount = 0;
        for (byte i = 0; i < maxFrameChecking; i++) {
            checkHeight();
        }
        return;
    }

    [Server]
    public void checkHeight() {
        if (endMenuManager.isGameEnded == true) {
            return;
        }
        const int shit = -9999;
        int currentFrameMaxHeight = shit;
        GameObject maxHeightGameObject = null;
        int i = 0;
        for (; i < placedObjects.Count; i++) {
            if (checkValues(i) == true) {
                int yPosition = (int)(placedObjectsTransforms[i].position.y);
                if (yPosition > currentFrameMaxHeight) {
                    currentFrameMaxHeight = yPosition;
                    currentGameMaxHeight = currentFrameMaxHeight;
                    maxHeightGameObject = placedObjects[i];
                }
            }
        }
        if (currentFrameMaxHeight != shit) {
            frameCount++;
            if (frameCount == 1) {
                previousFrameGameObject = maxHeightGameObject;
                return;
            } else if (frameCount == maxFrameChecking) {
                if (previousFrameGameObject == maxHeightGameObject) {
                    heightText.text = ("Score : " + currentFrameMaxHeight + " / " + _objectiveScript.objectiveScore + ".");
                    if (currentFrameMaxHeight >= _objectiveScript.objectiveScore) {
                        _objectiveScript.generateObjective(false);
                        resetLists();
                        if (isServer == true) {
                            _objectScript.giveMoreItems();
                        }
                        LoadedPlayerData.playerData.maxHeight = (_objectiveScript.objectiveScore - objectiveScript.newObjectiveScoreAddition);
                    }
                }
                frameCount = 0;
            } else {
                if (previousFrameGameObject != maxHeightGameObject) {
                    return;
                }
            }
        }
        return;
    }

    private bool checkValues(int i) {
        return ((StaticClass.isInBetweenOrEqualToTwoValues(placedObjectsRigidbody2D[i].velocity, -tolerance, tolerance)) || (placedObjectsRigidbody2D[i].velocity == Vector2.zero));
    }

    private void resetLists() {
        Rigidbody2D tempRigidbody2D = placedObjectsRigidbody2D[(placedObjectsRigidbody2D.Count - 1)];
        tempRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

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