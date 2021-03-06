﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectiveScript : NetworkBehaviour {
    [SyncVar(hook = nameof(syncSecond))] private int second;
    [SyncVar(hook = nameof(syncNewSecond))] private int newSecond = 60;
    private int windSustainTime = 1, windGenerationHeight = 50;
    [NonSerialized] public int newObjectiveScoreAddition = 10;
    [NonSerialized] public int objectiveScore = 0;
    [SyncVar(hook = nameof(syncDifficulty))] private Difficulty difficulty = Difficulty.Default;
    [SerializeField] private heightScript _heightScript = null;

    [Header("Variables for height canvases.")]
    private float heightCanvasPivotX;
    [SerializeField] private RectTransform heightCanvasRectTransform = null;
    [SerializeField] private Canvas textCanvasTemplate = null;
    private readonly List<Canvas> textCanvases = new List<Canvas>();

    [Header("Variables for cannons.")]
    bool hasCannonsEntered = false;
    [SyncVar] private float cannonShootingDelay = 5f;
    [SerializeField] private Text timerText = null;
    [SerializeField] private Transform heightTextsParent = null;

    private readonly List<cannonInformationHolder> cannonInformationHolders = new List<cannonInformationHolder>();
    private readonly List<Animator> cannonsAnimator = new List<Animator>();
    private readonly List<GameObject> cannonList = new List<GameObject>();

    [SerializeField] private GameObject cannonGrouperObject = null, cannon = null;

    [Header("Variables for winds."), SyncVar] public int minimumDifferenceForEachWinds = 5;
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
            freezeAll();
            removeWinds();
        }

        if (toggleWindsFunction != null) {
            StopCoroutine(toggleWindsFunction);
        }

        second = newSecond;

        int newScore = (objectiveScore + newObjectiveScoreAddition);
        float leftSide = (sharedMonobehaviour.leftSide + heightCanvasPivotX);

        const int step = 5;
        int index = 0;
        for (int i = objectiveScore; i <= newScore; i = (i + step)) {
            generateCannons(index, i, isFromAwake);
            Canvas newCanvas;
            if (isFromAwake == true) {
                newCanvas = Instantiate(textCanvasTemplate, new Vector2(leftSide, i), Quaternion.identity, heightTextsParent);
                textCanvases.Add(newCanvas);
            } else {
                newCanvas = textCanvases[index].GetComponent<Canvas>();
                index++;
            }
            Transform canvasTransform = newCanvas.transform;
            canvasTransform.position = new Vector2(leftSide, i);
            newCanvas.GetComponentInChildren<Text>().text = (canvasTransform.position.y.ToString() + ".");
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
        for (int i = 0; i < (_heightScript.placedObjects.Count - 1); i++) {
            objectInformation _objectInformation = _heightScript.placedObjects[i].GetComponent<objectInformation>();
            if (_objectInformation.dimDelegate == null) {
                _objectInformation.dimDelegate = _objectInformation.Dim;
            }
            _objectInformation.dimDelegate();
        }
        _heightScript.resetLists();
        return;
    }

    #region Cannons.
    private void generateCannons(int index, int height, bool isFromAwake) {
        Vector3 position = new Vector3((sharedMonobehaviour.leftSide - 1f), height);
        GameObject generatedCannon;
        if (isFromAwake == true) {
            generatedCannon = Instantiate(cannon, position, Quaternion.identity, cannonGrouperObject.transform);
            cannonList.Add(generatedCannon);
            cannonInformationHolders.Add(generatedCannon.GetComponent<cannonInformationHolder>());
            cannonsAnimator.Add(generatedCannon.GetComponent<Animator>());
        } else {
            generatedCannon = cannonList[index];
            generatedCannon.transform.position = position;
        }
        generatedCannon.name = height.ToString();
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
                Vector3 position = cannonInformationHolders[i].cannonTip.transform.position;
                GameObject spawnedCannonShell = Instantiate(cannonInformationHolders[i].cannonShell, position, Quaternion.identity, cannonList[i].transform);
                NetworkServer.Spawn(spawnedCannonShell);
                clientRPCUpdateCannonShell(spawnedCannonShell, i, position);
                spawnedCannonShell.GetComponent<Rigidbody2D>().velocity = new Vector2(15, 0);
            }
            yield return new WaitForSeconds(waitSecond);
        }
    }

    [ClientRpc]
    private void clientRPCUpdateCannonShell(GameObject _gameObject, int i, Vector3 position) {
        if (isServer == true) {
            return;
        }
        _gameObject.transform.parent = cannonList[i].transform;
        //We do this so the cannon shell does not spawn in the middle of the screen.
        _gameObject.transform.position = position;
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
        _gameObject.GetComponent<windScript>().enabled = false;
        _gameObject.GetComponent<BoxCollider2D>().enabled = false;
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