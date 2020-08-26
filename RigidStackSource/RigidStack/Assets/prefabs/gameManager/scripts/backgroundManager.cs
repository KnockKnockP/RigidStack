using System.Collections;
using UnityEngine;

public class backgroundManager : MonoBehaviour {
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;
    [SerializeField] private Transform gridTransform = null;


    private float maximumHeightOfGeneratedBackgrounds;
    [SerializeField] private Transform backgroundHolderEmptyObject = null;
    //staticBackgrounds are backgrounds that does not care about the camera's position.
    [SerializeField] private GameObject[] staticBackgrounds = null;
    //dynamicBackgrounds are backgrounds that generates as the camera moves.
    [SerializeField] private GameObject[] dynamicBackgrounds = null;
    //private readonly List<GameObject> generatedDynamicBackgrounds = new List<GameObject>();

    private void Start() {
        if (PlayerSettings.isBackgroundEnabled == true) {
            generateStaticBackgrounds();
            StartCoroutine(generateDynamicBackgrounds());
        }
        return;
    }

    private void generateStaticBackgrounds() {
        GameObject previousBackground = null;
        for (int i = 0; i < staticBackgrounds.Length; i++) {
            GameObject generatedBackground = Instantiate(staticBackgrounds[i], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
            //SETTINGS IMPLEMENTATION
            resizeBackground(generatedBackground, false);
            Vector3 backgroundPosition;
            if (i == 0) {
                backgroundPosition = new Vector3(0f, (generatedBackground.transform.localScale.y + gridTransform.position.y + generatedBackground.GetComponent<backgroundInformationHolder>().gridOffset), 0f);
            } else {
                backgroundPosition = new Vector3(0f, (previousBackground.transform.position.y + (_sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
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
            if (_sharedMonobehaviour.mainCamera.transform.position.y > (maximumHeightOfGeneratedBackgrounds - _sharedMonobehaviour.mainCamera.orthographicSize)) {
                GameObject generatedBackground = Instantiate(dynamicBackgrounds[Random.Range(0, dynamicBackgrounds.Length)], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
                resizeBackground(generatedBackground, false);
                Vector3 backgroundPosition = new Vector3(0f, (maximumHeightOfGeneratedBackgrounds + (_sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
                generatedBackground.transform.position = backgroundPosition;
                maximumHeightOfGeneratedBackgrounds = backgroundPosition.y;
            }
        }
    }

    //https://answers.unity.com/answers/620736/view.html
    public void resizeBackground(GameObject background, bool keepAspectRatio) {
        SpriteRenderer backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        float width = backgroundSpriteRenderer.sprite.bounds.size.x,
              height = backgroundSpriteRenderer.sprite.bounds.size.y,
              worldScreenHeight = (_sharedMonobehaviour.mainCamera.orthographicSize * 2f),
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