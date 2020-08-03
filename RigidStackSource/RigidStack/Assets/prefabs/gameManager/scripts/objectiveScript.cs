using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveScript : MonoBehaviour {
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;


    private int second;
    //DIFFICULTY IMPLEMENTATION
    public static int newObjectiveScoreAddition = 10, newSecond = 15;
    private int _objectiveScore;
    public int objectiveScore {
        get {
            return _objectiveScore;
        }
        set {
            _objectiveScore = value;
            Debug.Log("Objective score has been set to " + objectiveScore);
        }
    }


    bool hasCannonsEntered = false;
    [SerializeField] private Text timerText = null;
    [SerializeField] private Transform heightTextsParent = null;
    [SerializeField] private Canvas textCanvasTemplate = null;
    private readonly List<Canvas> textCanvases = new List<Canvas>();

    private readonly List<cannonInformationHolder> tempCannonInformationHolders = new List<cannonInformationHolder>();
    private List<cannonInformationHolder> cannonInformationHolders = new List<cannonInformationHolder>();

    private readonly List<Animator> tempCannonsAnimator = new List<Animator>();
    private List<Animator> cannonsAnimator = new List<Animator>();

    private readonly List<GameObject> tempCannonList = new List<GameObject>();
    private List<GameObject> cannonList = new List<GameObject>();

    [SerializeField] private GameObject cannonGrouperObject = null, cannon = null;

    private void Awake() {
        //Debug.LogWarning("Remove this line when done testing."); newSecond = 6;

        second = newSecond;
        generateObjective(true);
        StartCoroutine(countDown());
        return;
    }

    public void generateObjective(bool isFromAwake) {
        if (isFromAwake == false) {
            reset();
            freezeAll();
            removeCannons();
        }

        second = newSecond;

        int newScore = (objectiveScore + newObjectiveScoreAddition);
        float leftSide = (_sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x + textCanvasTemplate.GetComponent<RectTransform>().rect.width - textCanvasTemplate.transform.localScale.x);
        for (int i = objectiveScore; i <= newScore; i = (i + 5)) {
            Canvas newCanvas = Instantiate(textCanvasTemplate, new Vector2(leftSide, i), Quaternion.identity, heightTextsParent);
            newCanvas.GetComponentInChildren<Text>().text = (newCanvas.transform.position.y.ToString() + ".");
            newCanvas.gameObject.SetActive(true);
            textCanvases.Add(newCanvas);

            generateCannons(i, isFromAwake);
        }
        objectiveScore = newScore;
        return;
    }

    private void generateCannons(int height, bool isFromAwake) {
        float leftSide = _sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x;
        Vector3 position = new Vector3((leftSide - 1f), height, 0f);
        GameObject generatedCannon = Instantiate(cannon, position, Quaternion.identity, cannonGrouperObject.transform);
        generatedCannon.name = (height.ToString() + " cannon.");

        cannonInformationHolder generatedCannonInformationHolder = generatedCannon.GetComponent<cannonInformationHolder>();
        Animator generatedCannonAnimator = generatedCannon.GetComponent<Animator>();

        if (isFromAwake == true) {
            cannonList.Add(generatedCannon);
            cannonInformationHolders.Add(generatedCannonInformationHolder);
            cannonsAnimator.Add(generatedCannonAnimator);
        } else {
            tempCannonList.Add(generatedCannon);
            tempCannonInformationHolders.Add(generatedCannonInformationHolder);
            tempCannonsAnimator.Add(generatedCannonAnimator);
        }
        return;
    }

    private IEnumerator countDown() {
        //DIFFICULTY IMPLEMENTATION
        bool needsToStopCoroutine = false;
        IEnumerator shootCannonsFunction = shootCannons(3f);
        while (true) {
            yield return new WaitForSeconds(1f);
            timerText.text = ("Time left : " + second + ".");
            if (second > -1) {
                if (needsToStopCoroutine == true) {
                    needsToStopCoroutine = false;
                    StopCoroutine(shootCannonsFunction);
                }
                if ((second == newSecond) && (hasCannonsEntered == true)) {
                    exitCannons();
                } else if ((second < 6) && (hasCannonsEntered == false)) {
                    enterCannons();
                }
            } else {
                if (needsToStopCoroutine == false) {
                    needsToStopCoroutine = true;
                    StartCoroutine(shootCannonsFunction);
                }
            }
            second--;
            yield return null;
        }
    }

    private void enterCannons() {
        hasCannonsEntered = true;
        foreach (Animator cannonAnimator in cannonsAnimator) {
            cannonAnimator.SetBool("enterScene", true);
        }
        return;
    }

    private void exitCannons() {
        hasCannonsEntered = false;
        foreach (Animator cannonAnimator in cannonsAnimator) {
            cannonAnimator.SetBool("enterScene", false);
        }
        return;
    }

    private IEnumerator shootCannons(float waitSecond) {
        while (true) {
            for (int i = 0; i < cannonList.Count; i++) {
                Vector2 tipPosition = cannonInformationHolders[i].cannonTip.transform.position;
                Vector3 position = new Vector3(tipPosition.x, tipPosition.y, 0);
                Instantiate(cannonInformationHolders[i].cannonShell, position, cannonInformationHolders[i].cannonTip.transform.rotation, cannonList[i].transform).GetComponent<Rigidbody2D>().velocity = new Vector2(15, 0);
            }
            yield return new WaitForSeconds(waitSecond);
        }
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

    private void removeCannons() {
        foreach (GameObject _cannon in cannonList) {
            if (_cannon != null) {
                Destroy(_cannon);
            }
        }


        cannonList.Clear();
        cannonsAnimator.Clear();
        cannonInformationHolders.Clear();


        cannonList = tempCannonList;
        tempCannonList.Clear();

        cannonsAnimator = tempCannonsAnimator;
        tempCannonsAnimator.Clear();

        cannonInformationHolders = tempCannonInformationHolders;
        tempCannonInformationHolders.Clear();
        return;
    }
}