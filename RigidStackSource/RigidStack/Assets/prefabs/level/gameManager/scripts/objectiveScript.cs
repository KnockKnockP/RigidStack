/*
    I wish I wrote this entire game clean.
    I need to deal with this super entangled spaghetti and I am confused as fuck.
*/

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

    //Variables for cannons.
    bool hasCannonsEntered = false;
    [SyncVar] private float cannonShootingDelay = 5f;
    private IEnumerator shootCannonsFunction;
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

    private void syncDifficulty(Difficulty oldDifficulty, Difficulty newDifficulty) {
        _ = oldDifficulty;
        difficulty = newDifficulty;
        _heightScript = FindObjectOfType<heightScript>();
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
        setCannons();
        if (isServer == true) {
            StartCoroutine(countDown());
        }
        return;
    }

    public void generateObjective(bool isFromAwake) {
        if (isFromAwake == false) {
            reset();
            if (isServer == true) {
                clientRPCFreezeAll();
            }
            /*
                This isn't actually a ClientRPC function but I have named it that way.
                Fix this later.
            */
            clientRPCRemoveCannons();
            removeWinds();
        }

        if (toggleWindsFunction != null) {
            StopCoroutine(toggleWindsFunction);
        }

        second = newSecond;

        int newScore = (objectiveScore + newObjectiveScoreAddition);
        float leftSide = (sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x + textCanvasTemplate.GetComponent<RectTransform>().rect.width - textCanvasTemplate.transform.localScale.x);
        for (int i = objectiveScore; i <= newScore; i = (i + 5)) {
            Canvas newCanvas = Instantiate(textCanvasTemplate, new Vector2(leftSide, i), Quaternion.identity, heightTextsParent);
            newCanvas.GetComponentInChildren<Text>().text = (newCanvas.transform.position.y.ToString() + ".");
            newCanvas.gameObject.SetActive(true);
            textCanvases.Add(newCanvas);

            generateCannons(i, isFromAwake);
        }
        objectiveScore = newScore;
        _heightScript.updateHeightText();

        if (isServer == true) {
            generateWinds(isFromAwake);
            toggleWindsFunction = toggleWinds(isFromAwake);
            StartCoroutine(toggleWindsFunction);
        }
        return;
    }

    [ClientRpc]
    private void clientRPCFreezeAll() {
        Color32 dimmedColor = new Color32(50, 50, 50, 255);
        foreach (GameObject placedObject in _heightScript.placedObjects) {
            placedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
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

    private void setCannons() {
        shootCannonsFunction = shootCannons(cannonShootingDelay);
        return;
    }

    private void clientRPCRemoveCannons() {
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
            yield return null;
            for (int i = 0; i < cannonList.Count; i++) {
                Vector2 tipPosition = cannonInformationHolders[i].cannonTip.transform.position;
                Vector3 position = new Vector3(tipPosition.x, tipPosition.y, 0);
                Instantiate(cannonInformationHolders[i].cannonShell, position, cannonInformationHolders[i].cannonTip.transform.rotation, cannonList[i].transform).GetComponent<Rigidbody2D>().velocity = new Vector2(15, 0);
            }
            yield return new WaitForSeconds(waitSecond);
        }
    }

    [ClientRpc]
    private void clientRPCStartCannons() {
        StartCoroutine(shootCannonsFunction);
        return;
    }

    [ClientRpc]
    private void clientRPCStopCannons() {
        StopCoroutine(shootCannonsFunction);
        return;
    }
    #endregion

    #region Winds.
    [Server]
    private void generateWinds(bool isFromAwake) {
        if (_heightScript.currentGameMaxHeight >= windGenerationHeight) {
            for (int i = (objectiveScore - newObjectiveScoreAddition + 1); i <= objectiveScore; i = (i + UnityEngine.Random.Range(minimumDifferenceForEachWinds, (newObjectiveScoreAddition + 1)))) {
                float leftSide = (sharedMonobehaviour._sharedMonobehaviour.mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 10f)).x / 2.5f);
                Vector3 windPosition = new Vector3(leftSide, i, 0);
                GameObject generatedWind = Instantiate(windPrefab, windPosition, Quaternion.identity, windsEmptyObject.transform);

                generatedWind.SetActive(false);
                generatedWind.name = (i + " wind");

                BoxCollider2D generatedWindBoxCollider2D = generatedWind.GetComponent<BoxCollider2D>();
                float width = ((2f * sharedMonobehaviour._sharedMonobehaviour.mainCamera.orthographicSize) * sharedMonobehaviour._sharedMonobehaviour.mainCamera.aspect);
                generatedWindBoxCollider2D.size = new Vector2(width, generatedWindBoxCollider2D.size.y);
                generatedWindBoxCollider2D.offset = new Vector2((generatedWindBoxCollider2D.size.x / 2), generatedWindBoxCollider2D.offset.y);

                if (isFromAwake == true) {
                    winds.Add(generatedWind);
                } else {
                    tempWinds.Add(generatedWind);
                }
                NetworkServer.Spawn(generatedWind, connectionToClient);
            }
        }
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

    private IEnumerator actuallyToggleWinds(GameObject _wind, int delay) {
        yield return new WaitForSeconds(delay);
        if (_wind != null) {
            if (_wind.activeInHierarchy == false) {
                _wind.SetActive(true);
            } else {
                _wind.SetActive(false);
            }
        }
        yield return null;
    }
    #endregion

    private IEnumerator countDown() {
        bool needsToStopCoroutine = false;

        while (true) {
            if (second > -1) {
                if (needsToStopCoroutine == true) {
                    needsToStopCoroutine = false;
                    clientRPCStopCannons();
                }
                if ((second == newSecond) && (hasCannonsEntered == true)) {
                    clientRPCExitCannons();
                } else if ((second < 6) && (hasCannonsEntered == false)) {
                    clientRPCEnterCannons();
                }
            } else {
                if (needsToStopCoroutine == false) {
                    needsToStopCoroutine = true;
                    clientRPCStartCannons();
                }
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