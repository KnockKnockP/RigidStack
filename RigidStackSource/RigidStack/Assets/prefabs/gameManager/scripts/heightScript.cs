using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : MonoBehaviour {
    /*
        Rigidbody2D's velocity is Vector2.zero at the first frame of the object's existence.
        We will be safe and make sure it stays at Vector2.zero for atleast 2 frames.
        We are going to use frameCount to track the frames.
    */
    private byte frameCount;
    [HideInInspector] public long _maxHeight;
    [SerializeField] private Text heightText = null;
    [HideInInspector] public List<Transform> placedObjectsTransforms = new List<Transform>();
    [HideInInspector] public List<Rigidbody2D> placedObjectsRigidbody2D = new List<Rigidbody2D>();
    private GameObject previousFrameGameObject;
    [HideInInspector] public List<GameObject> placedObjects = new List<GameObject>();

    //TODO : Figure out the tolerance for the Vector2's velocity.
    private void Update() {
        int maxHeight = -9999;
        GameObject currentFrameGameObject;
        for (short i = 0; i < placedObjectsTransforms.Count; i++) {
            if (placedObjectsRigidbody2D[i].velocity == Vector2.zero) {
                int yPosition = (int)(placedObjectsTransforms[i].position.y);
                if (yPosition > maxHeight) {
                    maxHeight = yPosition;
                    heightText.text = maxHeight.ToString();
                    _maxHeight = maxHeight;
                    if (_maxHeight >= StaticClass.objectiveScore) {
                        if (frameCount == 1) {
                            previousFrameGameObject = placedObjects[i];
                            addFrameCount();
                        } else if (frameCount == 2) {
                            currentFrameGameObject = placedObjects[i];
                            //frameCount = 0;
                            if (previousFrameGameObject == currentFrameGameObject) {
                                frameCount = 0;
                                //Remove this after testing.
                                StaticClass.objectiveScore = 100;
                                //Debug.Log("Yay!");
                            } else {
                                frameCount = 0;
                                //Debug.Log("Nope.");
                            }
                        } else {
                            addFrameCount();
                        }
                    }
                }
            }
        }
        return;
    }

    private void addFrameCount() {
        frameCount++;
        Debug.Log("Tallest object stayed on Vector2.zero for " + frameCount + " frame(s).");
        return;
    }
}