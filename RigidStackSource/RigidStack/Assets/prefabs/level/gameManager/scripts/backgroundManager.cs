using System;
using System.Collections;
using UnityEngine;
#if !UNITY_EDITOR
    using UnityEngine.Experimental.Rendering.Universal;
#endif

public class backgroundManager : MonoBehaviour {
    //Variables for testing backgrounds.
    [SerializeField] private bool forceMorning = false, forceAfternoon = false, forceNight = false;

    //Variables for generating backgrounds.
    private float maximumHeightOfGeneratedBackgrounds;
    [SerializeField] private Transform gridTransform = null, backgroundHolderEmptyObject = null;
    private GameObject previousBackground;
    private Background background;
    [SerializeField] private DayBackgrounds dayBackgrounds = null;

    private void Start() {
#if !UNITY_EDITOR
            if (LoadedPlayerData.playerGraphics.isBackgroundEnabled == false) {
                GameObject globalLight = new GameObject("Global light.");
                Light2D light2D = globalLight.AddComponent<Light2D>();
                light2D.lightType = Light2D.LightType.Global;
                light2D.blendStyleIndex = 1;
                Destroy(this);
                return;
            }
#endif
        setBackground();
#if UNITY_EDITOR
        if (forceMorning == true) {
            background = dayBackgrounds.morningBackground;
        } else if (forceAfternoon == true) {
            background = dayBackgrounds.afternoonBackground;
        } else if (forceNight == true) {
            background = dayBackgrounds.nightBackground;
        }
#endif
        generateStaticBackgrounds();
        StartCoroutine(generateDynamicBackgrounds());
        return;
    }

    private void setBackground() {
        int hour = DateTime.Now.Hour;
        if ((hour >= 4) && (hour < 9)) {
            background = dayBackgrounds.morningBackground;
        } else if (hour >= 9 && hour < 20) {
            background = dayBackgrounds.afternoonBackground;
        } else {
            background = dayBackgrounds.nightBackground;
        }
        return;
    }

    private void generateStaticBackgrounds() {
        for (int i = 0; i < background.staticBackgrounds.Length; i++) {
            GameObject generatedBackground = Instantiate(background.staticBackgrounds[i], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
            resizeBackground(generatedBackground, LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio, sharedMonobehaviour._sharedMonobehaviour.mainCamera);
            Vector3 backgroundPosition;
            if (i == 0) {
                if (LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio == true) {
                    backgroundPosition = new Vector3(0f, gridTransform.position.y + (generatedBackground.GetComponent<backgroundInformationHolder>().aspectRatioGridOffset), 0f);
                } else {
                    backgroundPosition = new Vector3(0f, (generatedBackground.transform.localScale.y + gridTransform.position.y + generatedBackground.GetComponent<backgroundInformationHolder>().gridOffset), 0f);
                }
            } else {
                if (LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio == true) {
                    backgroundPosition = new Vector3(0f, ((previousBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.y * generatedBackground.transform.localScale.y) + previousBackground.transform.position.y), 0f);
                } else {
                    backgroundPosition = new Vector3(0f, (previousBackground.transform.position.y + (sharedMonobehaviour._sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
                }
            }
            previousBackground = generatedBackground;
            generatedBackground.transform.position = backgroundPosition;
            maximumHeightOfGeneratedBackgrounds = backgroundPosition.y;
        }
        return;
    }

    private IEnumerator generateDynamicBackgrounds() {
        while (true) {
            yield return null;
            if (sharedMonobehaviour._sharedMonobehaviour.mainCamera.transform.position.y > (maximumHeightOfGeneratedBackgrounds - sharedMonobehaviour._sharedMonobehaviour.mainCamera.orthographicSize)) {
                GameObject generatedBackground = Instantiate(background.dynamicBackgrounds[UnityEngine.Random.Range(0, background.dynamicBackgrounds.Length)], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
                resizeBackground(generatedBackground, LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio, sharedMonobehaviour._sharedMonobehaviour.mainCamera);
                Vector3 backgroundPosition;
                if (LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio == true) {
                    backgroundPosition = new Vector3(0f, ((previousBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.y * generatedBackground.transform.localScale.y) + previousBackground.transform.position.y), 0f);
                } else {
                    backgroundPosition = new Vector3(0f, (maximumHeightOfGeneratedBackgrounds + (sharedMonobehaviour._sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
                }
                previousBackground = generatedBackground;
                generatedBackground.transform.position = backgroundPosition;
                maximumHeightOfGeneratedBackgrounds = backgroundPosition.y;
            }
        }
    }

    //https://answers.unity.com/answers/620736/view.html
    public static void resizeBackground(GameObject background, bool keepAspectRatio, Camera mainCamera) {
        SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        float width = backgroundSpriteRenderer.sprite.bounds.size.x,
              height = backgroundSpriteRenderer.sprite.bounds.size.y,
              worldScreenHeight = (mainCamera.orthographicSize * 2f),
              worldScreenWidth = ((worldScreenHeight / Screen.height) * Screen.width);
        Vector3 imageScale = new Vector3(1f, 1f, 1f);
        if (keepAspectRatio == true) {
            Vector2 aspectRatio = new Vector2((width / height), (height / width));
            if ((worldScreenWidth / width) > (worldScreenHeight / height)) {
                imageScale.x = (worldScreenWidth / width);
                imageScale.y = (imageScale.x * aspectRatio.y);
            } else {
                imageScale.y = (worldScreenHeight / height);
                imageScale.x = (imageScale.y * aspectRatio.x);
            }
        } else {
            imageScale.x = (worldScreenWidth / width);
            imageScale.y = (worldScreenHeight / height);
        }
        background.transform.localScale = imageScale;
    }
}