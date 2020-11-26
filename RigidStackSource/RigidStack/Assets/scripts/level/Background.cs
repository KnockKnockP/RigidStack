using UnityEngine;

[CreateAssetMenu(fileName = "Background.", menuName = "ScriptableObjects/Background.")]
public class Background : ScriptableObject {
    /*
        staticBackgrounds are backgrounds that does not care about the camera's position.
        dynamicBackgrounds are backgrounds that generates as the camera moves.
    */
    public GameObject[] staticBackgrounds, dynamicBackgrounds;
}