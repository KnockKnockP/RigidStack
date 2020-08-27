using UnityEngine;

public class backgroundInformationHolder : MonoBehaviour {
    /*
        verticalSize is SpriteRenderer.sprite.bounds.size.y.
        gridOffset is the value you have to add or subtract for the background to fit perfectly.
        Exmaple usage : Vector3 baseBackgroundPosition = new Vector3(0f, (generatedBackground.transform.localScale.y + gridTransform.position.y + gridOffset), 0f);
        ..........................................................................................................................................^^^^^^^^^^
    */
    public float gridOffset = 0f, aspectRatioGridOffset = 0f;
    //aspectRatioGridOffset might be ((grid position) / 2).
}