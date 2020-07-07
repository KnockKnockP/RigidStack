using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class heightScript : MonoBehaviour {
    [SerializeField] private Text heightText;
    [HideInInspector] public List<Transform> placedObjectsTransforms = new List<Transform>();

    private void Update() {
        short maxHeight = -9999;
        for (short i = 0; i < placedObjectsTransforms.Count; i++) {
            short yPosition = (short)(placedObjectsTransforms[i].position.y);
            if (yPosition > maxHeight) {
                maxHeight = yPosition;
                heightText.text = maxHeight.ToString();
            }
        }
        return;
    }
}