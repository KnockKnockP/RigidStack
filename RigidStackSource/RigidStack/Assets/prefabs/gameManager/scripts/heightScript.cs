#region Using tags.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#endregion

#region "MonoBehaviour" inherited "heightScript" class.
public class heightScript : MonoBehaviour {
    #region Variables.
    #region Variables for height counting.
    private byte frameCount;
    [HideInInspector] public float tolerance = 0.1f;
    /*
        PlayerData.maxHeight is the maximum score for the whole account, it is also equal to the old objective score.
        currentGameMaxHeight is the maximum score for this only game.
        When the player dies, currentGameMaxHeight gets resetted.
    */
    [HideInInspector] public static int currentGameMaxHeight;
    #endregion

    #region Variables for tracking placed objects.
    private GameObject previousFrameGameObject;
    [HideInInspector] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [HideInInspector] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    [HideInInspector] public List<GameObject> placedObjects = new List<GameObject>();
    #endregion

    #region Variables for showing the height.
    private objectiveScript _objectiveScript;
    [SerializeField] private Text heightText = null;
    #endregion

    #region A variable for manual height checking.
    [SerializeField] private Button checkHeightButton = null;
    #endregion
    #endregion

    #region Awake function.
    private void Awake() {
        _objectiveScript = FindObjectOfType<objectiveScript>();
        return;
    }
    #endregion

    #region Start function.
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
    #endregion

    #region Automatically checking height.
    private IEnumerator updateHeight() {
        while (true) {
            checkHeight();
            yield return null;
        }
    }
    #endregion

    #region Manually checking height.
    public void manuallyCheckHeight() {
        frameCount = 0;
        checkHeight();
        checkHeight();
        return;
    }
    #endregion

    #region Checking height.
    public void checkHeight() {
        int currentFrameMaxHeight = -9999;
        GameObject currentFrameGameObject;
        for (short i = 0; i < placedObjectsTransforms.Count; i++) {
            if (checkValues(i) == true) {
                int yPosition = (int)(placedObjectsTransforms[i].position.y);
                if (yPosition > currentFrameMaxHeight) {
                    currentFrameMaxHeight = yPosition;
                    currentGameMaxHeight = currentFrameMaxHeight;
                    heightText.text = ("Score : " + currentFrameMaxHeight.ToString() + " / " + _objectiveScript.objectiveScore.ToString() + ".");
                    if (currentFrameMaxHeight >= _objectiveScript.objectiveScore) {
                        frameCount++;
                        if (frameCount == 1) {
                            previousFrameGameObject = placedObjects[i];
                            continue;
                        } else if (frameCount == 2) {
                            currentFrameGameObject = placedObjects[i];
                            if (previousFrameGameObject == currentFrameGameObject) {
                                _objectiveScript.generateObjective(false);
                                resetLists();
                                FindObjectOfType<objectScript>().giveMoreItems();
                                LoadedPlayerData.playerData.maxHeight = (_objectiveScript.objectiveScore - objectiveScript.newObjectiveScoreAddition);
                            }
                            frameCount = 0;
                        }
                    }
                }
            }
        }
        return;
    }
    #endregion

    #region Checking velocity values.
    private bool checkValues(int i) {
        return ((StaticClass.isInBetweenOrEqualToTwoValues(placedObjectsRigidbody2D[i].velocity, -tolerance, tolerance)) || (placedObjectsRigidbody2D[i].velocity == Vector2.zero));
    }
    #endregion

    #region Resetting all lists.
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
    #endregion
}
#endregion