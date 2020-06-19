using System;
using UnityEngine;
using UnityEngine.UI;

public class objectScript : MonoBehaviour {
    private objectClass[] _objectClass = new objectClass[5];
    //[SerializeField] private Sprite[] objectSprites = null;
    [SerializeField] private GameObject[] objects = null;
    [SerializeField] private Image[] images = null;

    private void Awake() {
        for (short i = 0; i < images.Length; i++) {
            _objectClass[i] = images[i].GetComponent<objectClass>();
        }
        return;
    }

    private void Start() {
        resetRandomization();
        shuffleItems();
        return;
    }

    private void resetRandomization() {
        UnityEngine.Random.InitState(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second * DateTime.Now.Millisecond);
        return;
    }

    private void shuffleItems() {
        for (short i = 0; i < images.Length; i++) {
            //images[i].sprite = objectSprites[UnityEngine.Random.Range(0, objectSprites.Length)];
            short random = (short)(UnityEngine.Random.Range(0, objects.Length));
            images[i].sprite = objects[random].GetComponent<SpriteRenderer>().sprite;
            _objectClass[i].objectCount = random;
            images[i].GetComponent<dragAndDropScript>().objectToPlace = objects[random];
        }
        return;
    }
}