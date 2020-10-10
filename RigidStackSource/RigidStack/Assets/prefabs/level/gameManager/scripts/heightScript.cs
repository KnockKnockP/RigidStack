/*
    I fucking hate working on this script and I want to stop working on it.
    Hopefully this is the last time I would ever touch this script and see no bugs from it.
    2020-10-08 02:10 PM.

    Well, fuck.
    I need to touch this stupid script again to implement multiplayer.
    This is going to take some while to get it right.
    2020-10-10 05:47 PM.
*/

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : NetworkBehaviour {
    //Variables for height counting.
    private byte frameCount;
    [HideInInspector] public float tolerance = 0.1f;
    /*
        PlayerData.maxHeight is the maximum score for the account's life time, it is also equal to the old objective score.
        currentGameMaxHeight is the maximum score for the single session of the game.
        When the player dies, currentGameMaxHeight gets resetted.

        It looks like I marked this variable as static just to make it easier to access it from other scripts.
        Now I will have to manually reset this stupid ass fucking value.
    */
    [HideInInspector] public int currentGameMaxHeight = 0;
    private objectScript _objectScript;

    //Variables for tracking placed objects.
    private GameObject previousFrameGameObject;
    [HideInInspector] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [HideInInspector] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    [HideInInspector] public List<GameObject> placedObjects = new List<GameObject>();

    //Variables for showing the height.
    private objectiveScript _objectiveScript;
    [SerializeField] private Text heightText = null;

    //A variable for manual height checking.
    [SerializeField] private Button checkHeightButton = null;

    private void Awake() {
        _objectScript = FindObjectOfType<objectScript>();
        _objectiveScript = FindObjectOfType<objectiveScript>();
        return;
    }

    private void Start() {
        switch (LoadedPlayerData.playerData.difficulty) {
            case (Difficulty.Sandbox) : {
                tolerance = 0.1f;
                break;
            }
            case (Difficulty.Easy) : {
                tolerance = 0.01f;
                break;
            }
            case (Difficulty.Moderate) : {
                tolerance = 0.001f;
                break;
            }
            case (Difficulty.Difficult) : {
                tolerance = 0.0001f;
                break;
            }
            case (Difficulty.Extreme) : {
                tolerance = 0f;
                break;
            }
        }
        heightText.text = ("Score : 0 / " + _objectiveScript.objectiveScore.ToString() + ".");
        checkHeightButton.gameObject.SetActive(LoadedPlayerData.playerData.isManualCheckingEnabled);
        if (LoadedPlayerData.playerData.isManualCheckingEnabled == false) {
            StartCoroutine(updateHeight());
        }
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
        checkHeight();
        checkHeight();
        return;
    }

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
            } else if (frameCount == 2) {
                if (previousFrameGameObject == maxHeightGameObject) {
                    heightText.text = ("Score : " + currentFrameMaxHeight + " / " + _objectiveScript.objectiveScore + ".");
                    if (currentFrameMaxHeight >= _objectiveScript.objectiveScore) {
                        _objectiveScript.generateObjective(false);
                        resetLists();
                        if (isServer) {
                            _objectScript.giveMoreItems();
                        }
                        LoadedPlayerData.playerData.maxHeight = (_objectiveScript.objectiveScore - objectiveScript.newObjectiveScoreAddition);
                    }
                }
                frameCount = 0;
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