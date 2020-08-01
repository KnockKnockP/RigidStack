using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : MonoBehaviour {
    private byte frameCount;
    //DIFFICULTY IMPLEMENTATION
    [HideInInspector] public float tolerance = -0.01f;
    [HideInInspector] public int maxHeight;


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

    private void LateUpdate() {
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
                    heightText.text = ("Score : " + currentFrameMaxHeight.ToString() + " / " + objectiveScript.objectiveScore.ToString() + ".");
                    maxHeight = currentFrameMaxHeight;
                    if (currentFrameMaxHeight >= objectiveScript.objectiveScore) {
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

        List<Transform> tempTransforms = new List<Transform>();
        tempTransforms.Add(placedObjectsTransforms[(placedObjectsTransforms.Count - 1)]);
        placedObjectsTransforms = tempTransforms;

        List<Rigidbody2D> tempRigidbody2Ds = new List<Rigidbody2D>();
        tempRigidbody2Ds.Add(tempRigidbody2D);
        placedObjectsRigidbody2D = tempRigidbody2Ds;

        List<GameObject> tempGameObjects = new List<GameObject>();
        tempGameObjects.Add(placedObjects[(placedObjects.Count - 1)]);
        placedObjects = tempGameObjects;
        return;
    }
}