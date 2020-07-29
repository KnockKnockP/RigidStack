using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveScript : MonoBehaviour {
    //DIFFICULTY IMPLEMENTATION
    [HideInInspector] public int newObjectiveScoreAddition = 10, newSecond = 15;
    private int second;
    [SerializeField] private Text timerText = null;
    [SerializeField] private Transform heightTextsParent = null;
    [SerializeField] private Canvas textCanvasTemplate = null;
    private readonly List<Canvas> textCanvases = new List<Canvas>();
    private Camera mainCamera;
    [SerializeField] private GameObject cannons = null, cannon = null/*, cannonShell = null*/;
    private readonly List<GameObject> cannonList = new List<GameObject>();

    private void Awake() {
        second = newSecond;
        mainCamera = Camera.main;
        return;
    }

    private void Start() {
        generateObjective(true);
        StartCoroutine(countDown());
    }

    public void generateObjective(bool isFromAwake) {
        int newScore = (StaticClass.objectiveScore + newObjectiveScoreAddition);
        if (isFromAwake == false) {
            reset();
            freezeAll();
        }
        second = newSecond;
        for (int i = StaticClass.objectiveScore; i <= newScore; i = (i + 5)) {
            Canvas newCanvas = Instantiate(textCanvasTemplate, new Vector2(0, i), Quaternion.identity, heightTextsParent);
            newCanvas.GetComponentInChildren<Text>().text = newCanvas.transform.position.y.ToString();
            newCanvas.gameObject.SetActive(true);
            textCanvases.Add(newCanvas);
            generateCannons(i);
        }
        StaticClass.objectiveScore = newScore;
        return;
    }

    private void generateCannons(int height) {
        float leftSide = mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x;
        Vector3 position = new Vector3((leftSide - 1f), height, 0f);
        cannonList.Add(Instantiate(cannon, position, Quaternion.identity, cannons.transform));
        return;
    }

    private IEnumerator countDown() {
        for (; second >= 0; second--) {
            yield return new WaitForSeconds(1);
            timerText.text = ("Time left : " + second);
        }
        Debug.Log("Time is up!\r\n" +
                  "Cannons will be trying to knock the tower(s) down.");
        yield return null;
    }

    private void freezeAll() {
        heightScript _heightScript = FindObjectOfType<heightScript>();
        foreach (GameObject placedObject in _heightScript.placedObjects) {
            placedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            placedObject.GetComponent<SpriteRenderer>().color = new Color32(50, 50, 50, 255);
        }
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