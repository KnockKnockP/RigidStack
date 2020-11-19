using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveScript : NetworkBehaviour {
    //Variables for the basic gameplay.
    [SyncVar(hook = nameof(syncSecond))] private int second;
    [SyncVar(hook = nameof(syncNewSecond))] private int newSecond = 60;
    private int windSustainTime = 1, windGenerationHeight = 50;
    [NonSerialized] public int newObjectiveScoreAddition = 10;
    [NonSerialized] public int objectiveScore = 0;
    [SyncVar(hook = nameof(syncDifficulty))] private Difficulty difficulty;
    private heightScript _heightScript;

    //Variables for height canvases.
    private float heightCanvasPivotX;
    private RectTransform heightCanvasRectTransform;

    //Variables for cannons.
    bool hasCannonsEntered = false;
    [SyncVar] private float cannonShootingDelay = 5f;
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

    //Variables for winds.
    [SyncVar] public int minimumDifferenceForEachWinds = 5;
    [SerializeField] private GameObject windPrefab = null, windsEmptyObject = null;
    private List<GameObject> winds = new List<GameObject>();
    private readonly List<GameObject> tempWinds = new List<GameObject>();
    private IEnumerator toggleWindsFunction;

    private void Start() {
        if (isServer == true) {
            difficulty = LoadedPlayerData.playerData.difficulty;
        }
        return;
    }

    /*
        This is where actual initialization of this script happenes.
        It also links to heightScript since both scripts works very closely together.
        Therefore, no seperate initialization methods such as Awake and Start on heightScript is needed.

        Note :
            Any methods that require networking does not work when doing initialization.
    */
    private void syncDifficulty(Difficulty oldDifficulty, Difficulty newDifficulty) {
        _ = oldDifficulty;
        difficulty = newDifficulty;
        _heightScript = FindObjectOfType<heightScript>();
        heightCanvasRectTransform = textCanvasTemplate.GetComponent<RectTransform>();
        heightCanvasPivotX = heightCanvasRectTransform.pivot.x;
        switch (difficulty) {
            case (Difficulty.Sandbox): {
                newObjectiveScoreAddition = 10;
                newSecond = 60;
                windSustainTime = 1;
                minimumDifferenceForEachWinds = 5;
                windGenerationHeight = 50;
                cannonShootingDelay = 5f;
                break;
            }
            case (Difficulty.Easy): {
                newObjectiveScoreAddition = 10;
                newSecond = 15;
                windSustainTime = 2;
                minimumDifferenceForEachWinds = 5;
                windGenerationHeight = 50;
                cannonShootingDelay = 3f;
                break;
            }
            case (Difficulty.Moderate): {
                newObjectiveScoreAddition = 15;
                newSecond = 15;
                windSustainTime = 3;
                minimumDifferenceForEachWinds = 3;
                windGenerationHeight = 40;
                cannonShootingDelay = 2f;
                break;
            }
            case (Difficulty.Difficult): {
                newObjectiveScoreAddition = 20;
                newSecond = 13;
                windSustainTime = 3;
                minimumDifferenceForEachWinds = 3;
                windGenerationHeight = 30;
                cannonShootingDelay = 1f;
                break;
            }
            case (Difficulty.Extreme): {
                newObjectiveScoreAddition = 20;
                newSecond = 10;
                windSustainTime = 3;
                minimumDifferenceForEachWinds = 2;
                windGenerationHeight = 25;
                cannonShootingDelay = 0.5f;
                break;
            }
        }

        _heightScript.syncDifficulty(difficulty);
        generateObjective(true);
        if (isServer == true) {
            StartCoroutine(countDown());
        }
        return;
    }

    [ClientRpc]
    public void clientRPCGenerateObjective(bool isFromAwake) {
        generateObjective(isFromAwake);
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
        float leftSide = (sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x
                          + heightCanvasPivotX);
        for (int i = objectiveScore; i <= newScore; i = (i + 5)) {
            Canvas newCanvas = Instantiate(textCanvasTemplate, new Vector2(leftSide, i), Quaternion.identity, heightTextsParent);
            newCanvas.GetComponentInChildren<Text>().text = (newCanvas.transform.position.y.ToString() + ".");
            newCanvas.gameObject.SetActive(true);
            textCanvases.Add(newCanvas);

            generateCannons(i, isFromAwake);
        }
        objectiveScore = newScore;
        _heightScript.updateHeightText();

        generateWinds(isFromAwake);
        if (isServer == true) {
            toggleWindsFunction = toggleWinds(isFromAwake);
            StartCoroutine(toggleWindsFunction);
        }
        return;
    }

    private void freezeAll() {
        Color32 dimmedColor = new Color32(50, 50, 50, 255);
        for (int i = 0; i < (_heightScript.placedObjects.Count - 1); i++) {
            GameObject placedObject = _heightScript.placedObjects[i];
            Rigidbody2D rigidbody2D = _heightScript.placedObjectsRigidbody2D[i];
            if (rigidbody2D != null) {
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            placedObject.GetComponent<SpriteRenderer>().color = dimmedColor;
            if (placedObject.name.Contains("television") == true) {
                televisionScript _televisionScript = placedObject.GetComponent<televisionScript>();
                _televisionScript.videoPlayerSpriteRenderer.color = dimmedColor;
                _televisionScript.videoPlayer.Pause();
            }
        }
        _heightScript.resetLists();
        return;
    }

    private void reset() {
        foreach (Canvas _textCanvas in textCanvases) {
            Destroy(_textCanvas.gameObject);
        }
        textCanvases.Clear();
        return;
    }

    #region Cannons.
    private void generateCannons(int height, bool isFromAwake) {
        float leftSide = sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x;
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

    private void enterCannons() {
        hasCannonsEntered = true;
        foreach (Animator cannonAnimator in cannonsAnimator) {
            cannonAnimator.SetBool("enterScene", true);
        }
        return;
    }

    [ClientRpc]
    private void clientRPCEnterCannons() {
        enterCannons();
        return;
    }

    private void exitCannons() {
        hasCannonsEntered = false;
        foreach (Animator cannonAnimator in cannonsAnimator) {
            cannonAnimator.SetBool("enterScene", false);
        }
        return;
    }

    [ClientRpc]
    private void clientRPCExitCannons() {
        exitCannons();
        return;
    }

    private IEnumerator shootCannons(float waitSecond) {
        while (true) {
            for (int i = 0; i < cannonList.Count; i++) {
                GameObject spawnedCannonShell = Instantiate(cannonInformationHolders[i].cannonShell, cannonInformationHolders[i].cannonTip.transform.position, Quaternion.identity, cannonList[i].transform);
                NetworkServer.Spawn(spawnedCannonShell);
                clientRPCUpdateCannonShell(spawnedCannonShell, i);
                spawnedCannonShell.GetComponent<Rigidbody2D>().velocity = new Vector2(15, 0);
            }
            yield return new WaitForSeconds(waitSecond);
        }
    }

    [ClientRpc]
    private void clientRPCUpdateCannonShell(GameObject _gameObject, int i) {
        if (isServer == true) {
            return;
        }
        _gameObject.transform.parent = cannonList[i].transform;
        //We do this so the cannon shell does not spawn in the middle of the screen.
        _gameObject.transform.position = new Vector3(-500f, -500f, 0f);
        Destroy(_gameObject.GetComponent<Rigidbody2D>());
        Destroy(_gameObject.GetComponent<CircleCollider2D>());
        return;
    }
    #endregion

    #region Winds.
    private void generateWinds(bool isFromAwake) {
        if (_heightScript.currentGameMaxHeight >= windGenerationHeight) {
            for (int i = (objectiveScore - newObjectiveScoreAddition + 1); i <= objectiveScore; i = (i + UnityEngine.Random.Range(minimumDifferenceForEachWinds, (newObjectiveScoreAddition + 1)))) {
                float leftSide = (sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x / 2.5f);
                Vector3 windPosition = new Vector3(leftSide, i, 0);
                //GameObject generatedWind = Instantiate(windPrefab, windPosition, Quaternion.identity, windsEmptyObject.transform);
                GameObject generatedWind = Instantiate(windPrefab);

                if (isFromAwake == true) {
                    winds.Add(generatedWind);
                } else {
                    tempWinds.Add(generatedWind);
                }
                NetworkServer.Spawn(generatedWind, connectionToClient);
                clientRPCParalyzeWind(generatedWind, isFromAwake, i, windPosition);
            }
        }
        return;
    }

    [ClientRpc]
    private void clientRPCParalyzeWind(GameObject _gameObject, bool isFromAwake, int i, Vector3 position) {
        if (isFromAwake == true) {
            winds.Add(_gameObject);
        } else {
            tempWinds.Add(_gameObject);
        }
        _gameObject.SetActive(false);
        _gameObject.transform.parent = windsEmptyObject.transform;
        _gameObject.transform.position = position;
        _gameObject.name = (i + " wind");
        if (isServer == true) {
            BoxCollider2D generatedWindBoxCollider2D = _gameObject.GetComponent<BoxCollider2D>();
            float orthographicSize = sharedMonobehaviour._sharedMonobehaviour.mainCamera.orthographicSize, width = ((2f * orthographicSize) * sharedMonobehaviour._sharedMonobehaviour.mainCamera.aspect);
            generatedWindBoxCollider2D.size = new Vector2(width, generatedWindBoxCollider2D.size.y);
            generatedWindBoxCollider2D.offset = new Vector2((generatedWindBoxCollider2D.size.x / orthographicSize), generatedWindBoxCollider2D.offset.y);
            return;
        }
        Destroy(_gameObject.GetComponent<windScript>());
        Destroy(_gameObject.GetComponent<BoxCollider2D>());
        return;
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

    [Server]
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
                GameObject selectedWind = _winds[UnityEngine.Random.Range(0, _winds.Count)];
                if (selectedWind != null) {
                    int randomDelay = UnityEngine.Random.Range(0, newSecond);
                    StartCoroutine(actuallyToggleWinds(selectedWind, randomDelay));
                    StartCoroutine(actuallyToggleWinds(selectedWind, (randomDelay + windSustainTime)));
                }
                yield return new WaitForSeconds(UnityEngine.Random.Range((newSecond - 5), (newSecond + 5)));
            }
        }
    }

    [Server]
    private IEnumerator actuallyToggleWinds(GameObject _wind, int delay) {
        yield return new WaitForSeconds(delay);
        clientRPCActuallyToggleWinds(_wind);
        yield return null;
    }

    [ClientRpc]
    private void clientRPCActuallyToggleWinds(GameObject _wind) {
        if (_wind != null) {
            _wind.SetActive(!_wind.activeInHierarchy);
        }
        return;
    }
    #endregion

    private IEnumerator countDown() {
        bool needsToStopCoroutine = false;
        IEnumerator shootCannonsFunction = shootCannons(cannonShootingDelay);
        while (true) {
            if (second > -1) {
                if (needsToStopCoroutine == true) {
                    needsToStopCoroutine = false;
                    StopCoroutine(shootCannonsFunction);
                }
                if ((second == newSecond) && (hasCannonsEntered == true)) {
                    clientRPCExitCannons();
                } else if ((second < 6) && (hasCannonsEntered == false)) {
                    clientRPCEnterCannons();
                }
            } else if (needsToStopCoroutine == false) {
                needsToStopCoroutine = true;
                StartCoroutine(shootCannonsFunction);
            }
            second--;
            yield return new WaitForSeconds(1f);
        }
    }

    private void syncSecond(int oldSecond, int _newSecond) {
        _ = oldSecond;
        second = _newSecond;
        timerText.text = ("Time left : " + second + ".");
        return;
    }

    private void syncNewSecond(int oldSecond, int _newSecond) {
        _ = oldSecond;
        newSecond = _newSecond;
        return;
    }
}