using UnityEngine;

public class backgroundManager : MonoBehaviour {
    [SerializeField] private sharedMonobehaviour _sharedMonobehaviour = null;
    [SerializeField] private Transform gridTransform = null;


    [SerializeField] private Transform backgroundHolderEmptyObject = null;
    [SerializeField] private GameObject[] backgrounds = null;

    private void Awake() {
        generateExtendedSkies(generateBaseBackground());
        return;
    }

    private GameObject generateBaseBackground() {
        GameObject generatedBackground = Instantiate(backgrounds[0], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
        //SETTINGS IMPLEMENTATION
        resizeBackground(generatedBackground, false);
        Vector3 baseBackgroundPosition = new Vector3(0f, (generatedBackground.transform.localScale.y + gridTransform.position.y + generatedBackground.GetComponent<backgroundInformationHolder>().gridOffset), 0f);
        generatedBackground.transform.position = baseBackgroundPosition;
        return generatedBackground;
    }

    //TODO : Fucking rework this function.
    private void generateExtendedSkies(GameObject baseBackground) {
        GameObject generatedBackground = null;
        for (int i = 1; i < backgrounds.Length; i++) {
            float previousGeneratedBackgroundSize = 0f;
            if (generatedBackground != null) {
                previousGeneratedBackgroundSize = generatedBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            }
            generatedBackground = Instantiate(backgrounds[i], Vector3.zero, Quaternion.identity, backgroundHolderEmptyObject);
            //SETTINGS IMPLEMENTATION
            resizeBackground(generatedBackground, false);
            Vector3 backgroundPosition;
            Debug.Log(baseBackground.GetComponent<backgroundInformationHolder>().gridOffset);
            if (i == 1) {
                backgroundPosition = new Vector3(0f, (baseBackground.transform.position.y + (_sharedMonobehaviour.mainCamera.orthographicSize * 2)), 0f);
            } else {
                backgroundPosition = new Vector3(0f, (previousGeneratedBackgroundSize + generatedBackground.transform.localScale.y + gridTransform.position.y + generatedBackground.GetComponent<backgroundInformationHolder>().gridOffset), 0f);
            }
            generatedBackground.transform.position = backgroundPosition;
        }
        return;
    }

    //https://answers.unity.com/answers/620736/view.html
    void resizeBackground(GameObject background, bool keepAspectRatio) {
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