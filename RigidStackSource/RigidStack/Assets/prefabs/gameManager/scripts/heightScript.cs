using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : MonoBehaviour {
    private byte _frameCount;
    private byte frameCount {
        get {
            return _frameCount;
        }
        set {
            _frameCount = value;
            Debug.Log("Tallest object stayed on Vector2.zero (With the tolerance of " + tolerance + "f.) for " + frameCount + " frame(s).");
        }
    }
    [HideInInspector] public int _maxHeight;
    [HideInInspector] public float tolerance = -0.01f;
    private objectiveScript _objectiveScript;
    [SerializeField] private Text heightText = null;
    [HideInInspector] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [HideInInspector] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    private GameObject previousFrameGameObject;
    [HideInInspector] public List<GameObject> placedObjects = new List<GameObject>();

    private void Awake() {
        _objectiveScript = FindObjectOfType<objectiveScript>();
        return;
    }

    private void LateUpdate() {
        updateHeight();
        return;
    }

    private void updateHeight() {
        int maxHeight = -9999;
        GameObject currentFrameGameObject;
        for (short i = 0; i < placedObjectsTransforms.Count; i++) {
            if ((placedObjectsRigidbody2D[i].velocity.x >= tolerance) && (placedObjectsRigidbody2D[i].velocity.y >= tolerance)) {
                int yPosition = (int)(placedObjectsTransforms[i].position.y);
                if (yPosition > maxHeight) {
                    maxHeight = yPosition;
                    heightText.text = ("Score : " + maxHeight.ToString());
                    _maxHeight = maxHeight;
                    if (_maxHeight >= StaticClass.objectiveScore) {
                        frameCount++;
                        if (frameCount == 1) {
                            previousFrameGameObject = placedObjects[i];
                            continue;
                        } else if (frameCount == 2) {
                            currentFrameGameObject = placedObjects[i];
                            if (previousFrameGameObject == currentFrameGameObject) {
                                Debug.Log("Generating new objective.");
                                _objectiveScript.generateObjective(false);
                                resetLists();
                                FindObjectOfType<objectScript>().giveMoreItems();
                            }
                            /*
                                I have no fucking idea why but putting two Debug.Log(object message) here fixes the bug.
                                Just don't touch this dark magic.
                            */
                            Debug.Log("Here 1 : " + frameCount);
                            frameCount = 0;
                            Debug.Log("Here 2 : " + frameCount);
                        }
                    }
                }
            }
        }
        return;
    }

    private void resetLists() {
        Transform tempTransform = placedObjectsTransforms[(placedObjectsTransforms.Count - 1)];
        List<Transform> tempTransforms = new List<Transform>();
        Rigidbody2D tempRigidbody2D = placedObjectsRigidbody2D[(placedObjectsRigidbody2D.Count - 1)];
        List<Rigidbody2D> tempRigidbody2Ds = new List<Rigidbody2D>();
        GameObject tempGameObject = placedObjects[(placedObjects.Count - 1)];
        List<GameObject> tempGameObjects = new List<GameObject>();
        tempRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        for (int i = 0; i < (placedObjectsRigidbody2D.Count - 1); i++) {
            Destroy(placedObjectsRigidbody2D[i]);
        }
        tempTransforms.Add(tempTransform);
        placedObjectsTransforms = tempTransforms;
        tempRigidbody2Ds.Add(tempRigidbody2D);
        placedObjectsRigidbody2D = tempRigidbody2Ds;
        tempGameObjects.Add(tempGameObject);
        placedObjects = tempGameObjects;
        return;
    }
}