using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveScript : MonoBehaviour {
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;


    private int second, newSecond = 15, windSustainTime = 2, windGenerationHeight = 50;
    public static int newObjectiveScoreAddition = 10;
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
    private float cannonShootingDelay = 3f;
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


    public static int minimumDifferenceForEachWinds = 5;
    [SerializeField] private GameObject windPrefab = null, windsEmptyObject = null;
    private List<GameObject> winds = new List<GameObject>();
    private readonly List<GameObject> tempWinds = new List<GameObject>();
    private IEnumerator toggleWindsFunction;

    private void Awake() {
        //Debug.LogWarning("Remove this line when done testing."); newSecond = 6;
        return;
    }

    private void Start() {
        switch (LoadedPlayerData.playerData.difficulty) {
            case (Difficulty.Easy) : {
                newObjectiveScoreAddition = 10;
                newSecond = 15;
                windSustainTime = 2;
                minimumDifferenceForEachWinds = 5;
                windGenerationHeight = 50;
                cannonShootingDelay = 3f;
                break;
            }
            case (Difficulty.Moderate) : {
                newObjectiveScoreAddition = 15;
                newSecond = 15;
                windSustainTime = 3;
                minimumDifferenceForEachWinds = 3;
                windGenerationHeight = 40;
                cannonShootingDelay = 2f;
                break;
            }
            case (Difficulty.Difficult) : {
                newObjectiveScoreAddition = 20;
                newSecond = 13;
                windSustainTime = 3;
                minimumDifferenceForEachWinds = 3;
                windGenerationHeight = 30;
                cannonShootingDelay = 1f;
                break;
            }
            case (Difficulty.Extreme) : {
                newObjectiveScoreAddition = 20;
                newSecond = 10;
                windSustainTime = 3;
                minimumDifferenceForEachWinds = 2;
                windGenerationHeight = 25;
                cannonShootingDelay = 0.5f;
                break;
            }
        }

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
            removeWinds();
        }

        if (toggleWindsFunction != null) {
            StopCoroutine(toggleWindsFunction);
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

        generateWinds(isFromAwake);
        toggleWindsFunction = toggleWinds(isFromAwake);
        StartCoroutine(toggleWindsFunction);
        return;
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

    private void generateCannons(int height, bool isFromAwake) {
        float leftSide = _sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x;
        Vector3 position = new Vector3((leftSide - 1f), height, 0f);
        GameObject generatedCannon = Instantiate(cannon, position, Quaternion.identity, cannonGrouperObject.transform);
        generatedCannon.name = (height.ToString() + " cannon");

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
            yield return null;
            for (int i = 0; i < cannonList.Count; i++) {
                Vector2 tipPosition = cannonInformationHolders[i].cannonTip.transform.position;
                Vector3 position = new Vector3(tipPosition.x, tipPosition.y, 0);
                Instantiate(cannonInformationHolders[i].cannonShell, position, cannonInformationHolders[i].cannonTip.transform.rotation, cannonList[i].transform).GetComponent<Rigidbody2D>().velocity = new Vector2(15, 0);
            }
            yield return new WaitForSeconds(waitSecond);
        }
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

    private void generateWinds(bool isFromAwake) {
        if (heightScript.currentGameMaxHeight >= windGenerationHeight) {
            for (int i = (objectiveScore - newObjectiveScoreAddition + 1); i <= objectiveScore; i = (i + Random.Range(minimumDifferenceForEachWinds, (newObjectiveScoreAddition + 1)))) {
                float leftSide = (_sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x / 2.5f);
                Vector3 windPosition = new Vector3(leftSide, i, 0);
                GameObject generatedWind = Instantiate(windPrefab, windPosition, Quaternion.identity, windsEmptyObject.transform);

                generatedWind.SetActive(false);
                generatedWind.name = (i + " wind");

                BoxCollider2D generatedWindBoxCollider2D = generatedWind.GetComponent<BoxCollider2D>();
                float width = ((2f * _sharedMonobehaviour.mainCamera.orthographicSize) * _sharedMonobehaviour.mainCamera.aspect);
                generatedWindBoxCollider2D.size = new Vector2(width, generatedWindBoxCollider2D.size.y);
                generatedWindBoxCollider2D.offset = new Vector2((generatedWindBoxCollider2D.size.x / 2), generatedWindBoxCollider2D.offset.y);

                if (isFromAwake == true) {
                    winds.Add(generatedWind);
                } else {
                    tempWinds.Add(generatedWind);
                }
            }
        }
        return;
    }

    private IEnumerator toggleWinds(bool isFromAwake) {
        List<GameObject> _winds;
        if (isFromAwake == true) {
            _winds = winds;
        } else {
            _winds = tempWinds;
        }
        while (true) {
            yield return null;
            if (_winds.Count > 0) {
                GameObject selectedWind = _winds[Random.Range(0, _winds.Count)];
                if (selectedWind != null) {
                    int randomDelay = Random.Range(0, newSecond);
                    StartCoroutine(actuallyToggleWinds(selectedWind, randomDelay));
                    StartCoroutine(actuallyToggleWinds(selectedWind, (randomDelay + windSustainTime)));
                }
                yield return new WaitForSeconds(Random.Range((newSecond - 5), (newSecond + 5)));
            }
        }
    }

    private IEnumerator actuallyToggleWinds(GameObject _wind, int delay) {
        yield return new WaitForSeconds(delay);
        if ((_wind != null) && (_wind.activeInHierarchy == false)) {
            _wind.SetActive(true);
        } else {
            _wind.SetActive(false);
        }
        yield return null;
    }

    private void removeWinds() {
        foreach (GameObject _wind in winds) {
            if (_wind != null) {
                Destroy(_wind);
            }
        }

        winds = tempWinds;
        tempWinds.Clear();
        return;
    }

    private IEnumerator countDown() {
        bool needsToStopCoroutine = false;
        IEnumerator shootCannonsFunction = shootCannons(cannonShootingDelay);
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
}