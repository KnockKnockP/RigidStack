#region Using tags.
using System.Collections;
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "backgroundManager" class.
public class backgroundManager : MonoBehaviour {
    #region Variables.

    #region A variable for accessing the main camera.
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;
    #endregion

    #region Variables for generating backgrounds.
    private float maximumHeightOfGeneratedBackgrounds;
    [SerializeField] private Transform gridTransform = null, backgroundHolderEmptyObject = null;
    /*
        staticBackgrounds are backgrounds that does not care about the camera's position.
        dynamicBackgrounds are backgrounds that generates as the camera moves.
    */
    private GameObject previousBackground;
    [SerializeField] private GameObject[] staticBackgrounds = null, dynamicBackgrounds = null;
    #endregion
    #endregion

    #region Start function.
    private void Start() {
        if (LoadedPlayerData.playerGraphics.isBackgroundEnabled == false) {
            Destroy(this);
            return;
        }
        generateStaticBackgrounds();
        StartCoroutine(generateDynamicBackgrounds());
        return;
    }
    #endregion

    #region Generating static backgrounds.
    private void generateStaticBackgrounds() {
        for (int i = 0; i < staticBackgrounds.Length; i++) {
            GameObject generatedBackground = Instantiate(staticBackgrounds[i], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
            resizeBackground(generatedBackground, LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
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
                    backgroundPosition = new Vector3(0f, (previousBackground.transform.position.y + (_sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
                }
            }
            previousBackground = generatedBackground;
            generatedBackground.transform.position = backgroundPosition;
            maximumHeightOfGeneratedBackgrounds = backgroundPosition.y;
        }
        return;
    }
    #endregion

    #region Generating dynamic backgrounds.
    private IEnumerator generateDynamicBackgrounds() {
        while (true) {
            yield return null;
            if (_sharedMonobehaviour.mainCamera.transform.position.y > (maximumHeightOfGeneratedBackgrounds - _sharedMonobehaviour.mainCamera.orthographicSize)) {
                GameObject generatedBackground = Instantiate(dynamicBackgrounds[Random.Range(0, dynamicBackgrounds.Length)], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
                resizeBackground(generatedBackground, LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
                Vector3 backgroundPosition;
                if (LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio == true) {
                    backgroundPosition = new Vector3(0f, ((previousBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.y * generatedBackground.transform.localScale.y) + previousBackground.transform.position.y), 0f);
                } else {
                    backgroundPosition = new Vector3(0f, (maximumHeightOfGeneratedBackgrounds + (_sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
                }
                previousBackground = generatedBackground;
                generatedBackground.transform.position = backgroundPosition;
                maximumHeightOfGeneratedBackgrounds = backgroundPosition.y;
            }
        }
    }
    #endregion

    #region Resizing backgrounds.
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
    #endregion
}
#endregion