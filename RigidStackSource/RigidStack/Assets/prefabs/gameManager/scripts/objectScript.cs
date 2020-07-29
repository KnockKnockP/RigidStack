using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class objectScript : MonoBehaviour {
    private objectClass[] _objectClass = new objectClass[5];
    [SerializeField] private Image[] images = null;
    private Sprite[] sprites = new Sprite[5];
    [SerializeField] private GameObject[] objects = null;

    private void Awake() {
        for (short i = 0; i < images.Length; i++) {
            _objectClass[i] = images[i].GetComponent<objectClass>();
        }
        resetRandomization();
        shuffleItems();
        return;
    }

    private void resetRandomization() {
        UnityEngine.Random.InitState(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second * DateTime.Now.Millisecond);
        return;
    }

    private void shuffleItems() {
        bool isDuplicate;
        for (short i = 0; i < images.Length; i++) {
            short random = (short)(UnityEngine.Random.Range(0, objects.Length));
            Sprite randomSprite = objects[random].GetComponent<SpriteRenderer>().sprite;
            isDuplicate = false;
            for (short j = 0; j < sprites.Length; j++) {
                if (sprites[j] == randomSprite) {
                    isDuplicate = true;
                    i--;
                    break;
                }
            }
            if (isDuplicate == false) {
                images[i].sprite = randomSprite;
                sprites[i] = images[i].sprite;
                //DIFFICULTY IMPLEMENTATION
                _objectClass[i].objectCount = (short)(UnityEngine.Random.Range(1, 10));
                images[i].GetComponent<dragAndDropScript>().objectToPlace = objects[random];
            }
        }
        return;
    }

    public void giveMoreItems() {
        foreach (Image image in images) {
            image.sprite = null;
        }
        for (short i = 0; i < sprites.Length; i++) {
            sprites[i] = null;
        }
        shuffleItems();
        return;
    }

    public void reset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return;
    }
}