using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : MonoBehaviour {
    /*
        Rigidbody2D's velocity is Vector2.zero at the first frame of the object's existence.
        We will be safe and make sure it stays at Vector2.zero for atleast 3 frames.
        We are going to use frameCount to track the frames.
    */
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

    private void FixedUpdate() {
        int maxHeight = -9999;
        GameObject currentFrameGameObject;
        for (short i = 0; i < placedObjectsTransforms.Count; i++) {
            if ((placedObjectsRigidbody2D[i].velocity.x >= tolerance) && (placedObjectsRigidbody2D[i].velocity.y >= tolerance)) {
                int yPosition = (int)(placedObjectsTransforms[i].position.y);
                if (yPosition > maxHeight) {
                    maxHeight = yPosition;
                    heightText.text = maxHeight.ToString();
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
                            }
                            frameCount = 0;
                        }
                    }
                }
            }
        }
        return;
    }
}