using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveScript : MonoBehaviour {
    private readonly int newObjectiveScoreAddition = 10, newSecond = 15;
    private int second;
    [SerializeField] private Text timerText = null;
    [SerializeField] private Transform heightTextsParent = null;
    [SerializeField] private Canvas textCanvasTemplate = null;
    private readonly List<Canvas> textCanvases = new List<Canvas>();

    private void Awake() {
        second = newSecond;
        generateObjective(true);
        StartCoroutine(countDown());
        return;
    }

    public void generateObjective(bool isFromAwake) {
        if (isFromAwake == false) {
            reset();
            freezeAll();
        }
        second = newSecond;
        for (int i = StaticClass.objectiveScore; i <= (StaticClass.objectiveScore + 10); i = (i + 5)) {
            Canvas newCanvas = Instantiate(textCanvasTemplate, new Vector2(0, i), Quaternion.identity, heightTextsParent);
            newCanvas.GetComponentInChildren<Text>().text = newCanvas.transform.position.y.ToString();
            newCanvas.gameObject.SetActive(true);
            textCanvases.Add(newCanvas);
        }
        StaticClass.objectiveScore = (StaticClass.objectiveScore + newObjectiveScoreAddition);
        return;
    }

    private IEnumerator countDown() {
        for (; second >= 0; second--) {
            yield return new WaitForSeconds(1);
            timerText.text = ("Time left : " + second);
        }
        Debug.Log("Time is up!\r\n" +
                  "Cannons will be trying to knock the tower down.");
        yield return null;
    }

    private void freezeAll() {
        heightScript _heightScript = FindObjectOfType<heightScript>();
        foreach (Transform placedObjectsTransform in _heightScript.placedObjectsTransforms) {
            GameObject placedObject = placedObjectsTransform.gameObject;
            placedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        StaticClass.objectiveScore = _heightScript._maxHeight;
        return;
    }

    private void reset() {
        foreach (Canvas _textCanvas in textCanvases) {
            Destroy(_textCanvas.gameObject);
        }
        textCanvases.Clear();
        return;
    }
}