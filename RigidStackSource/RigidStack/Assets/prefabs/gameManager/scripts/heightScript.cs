using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : MonoBehaviour {
    private byte frameCount;
    //DIFFICULTY IMPLEMENTATION
    [HideInInspector] public float tolerance = -0.01f;
    /*
        maxHeight is the maximum score for the whole account, it is also equal to the old objective score.
        currentGameMaxHeight is the maximum score for this only game.
        When the player dies, currentGameMaxHeight gets resetted.
    */
    [HideInInspector] public static int maxHeight, currentGameMaxHeight;


    private GameObject previousFrameGameObject;
    [HideInInspector] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [HideInInspector] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    [HideInInspector] public List<GameObject> placedObjects = new List<GameObject>();


    private objectiveScript _objectiveScript;
    [SerializeField] private Text heightText = null;

    private void Awake() {
        _objectiveScript = FindObjectOfType<objectiveScript>();
        return;
    }

    private void Start() {
        heightText.text = ("Score : 0 / " + _objectiveScript.objectiveScore.ToString() + ".");
        return;
    }

    private void FixedUpdate() {
        updateHeight();
        return;
    }

    private void updateHeight() {
        int currentFrameMaxHeight = -9999;
        GameObject currentFrameGameObject;
        for (short i = 0; i < placedObjectsTransforms.Count; i++) {
            _ = placedObjectsRigidbody2D[i].velocity;
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
                                maxHeight = (_objectiveScript.objectiveScore - objectiveScript.newObjectiveScoreAddition);
                            }
                            _ = frameCount;
                            frameCount = 0;
                            _ = frameCount;
                        }
                    }
                }
            }
        }
        return;
    }

    private bool checkValues(int i) {
        return StaticClass.isInBetweenOrEqualToTwoValues(placedObjectsRigidbody2D[i].velocity, -0.01f, 0.01f);
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