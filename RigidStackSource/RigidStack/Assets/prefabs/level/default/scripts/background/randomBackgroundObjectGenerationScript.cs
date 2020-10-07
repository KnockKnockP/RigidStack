using System.Collections;
using UnityEngine;

public class randomBackgroundObjectGenerationScript : MonoBehaviour {
    //A variable for storing random background objects.
    [SerializeField] private GameObject[] objects = null;

    private void Start() {
        StartCoroutine(generateRandomObjects());
        return;
    }

    private IEnumerator generateRandomObjects() {
        backgroundManager _backgroundManager = FindObjectOfType<backgroundManager>();
        while (true) {
            yield return null;
            int waitSecond = Random.Range(1, 91);
            //int waitSecond = Random.Range(1, 5);
            //Debug.Log(gameObject + "'s random object will generate after : " + waitSecond + " second(s).");
            yield return new WaitForSeconds(waitSecond);
            GameObject generatedRandomObject = Instantiate(objects[Random.Range(0, objects.Length)], transform.position, Quaternion.identity, transform);
            _backgroundManager.resizeBackground(generatedRandomObject, LoadedPlayerData.playerGraphics.isBackgroundScalingKeepAspectRatio);
            yield return new WaitForSeconds(generatedRandomObject.GetComponent<randomBackgroundObjectInformationHolder>().secondsToExistInTheScene);
            Destroy(generatedRandomObject);
        }
    }
}