//#define DEBUG_RANDOM_BACKGROUND_OBJECT_GENERATION_SCRIPT_CS
using System.Collections;
using UnityEngine;

public class randomBackgroundObjectGenerationScript : MonoBehaviour {
    [Header("A variable for storing random background objects.")]
    [SerializeField] private GameObject[] objects = null;

    private void Start() {
        StartCoroutine(generateRandomObjects());
        return;
    }

    private IEnumerator generateRandomObjects() {
        while (true) {
#if !DEBUG_RANDOM_BACKGROUND_OBJECT_GENERATION_SCRIPT_CS
            int waitSecond = Random.Range(1, 91);
#else
            int waitSecond = Random.Range(1, 5);
            Debug.Log(gameObject + "'s random object will generate after : " + waitSecond + " second(s).");
#endif
            yield return new WaitForSeconds(waitSecond);
            GameObject generatedRandomObject = Instantiate(objects[Random.Range(0, objects.Length)], transform.position, Quaternion.identity, transform);
            backgroundManager.resizeBackground(generatedRandomObject, LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio, sharedMonobehaviour._sharedMonobehaviour.mainCamera);
            yield return new WaitForSeconds(generatedRandomObject.GetComponent<randomBackgroundObjectInformationHolder>().secondsToExistInTheScene);
            Destroy(generatedRandomObject);
        }
    }
}