using System;
using UnityEngine;
using UnityEngine.UI;

public class objectScript : MonoBehaviour {
    private dragAndDropImageScript[] dragAndDropImageScripts = new dragAndDropImageScript[5];
    [SerializeField] private Image[] dragAndDropImages = null;

    //We are going to use this array to check for duplicates.
    private Sprite[] sprites = new Sprite[5];


    [SerializeField] private GameObject[] objects = null;

    private void Awake() {
        for (short i = 0; i < dragAndDropImages.Length; i++) {
            dragAndDropImageScripts[i] = dragAndDropImages[i].GetComponent<dragAndDropImageScript>();
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
        for (short i = 0; i < dragAndDropImages.Length; i++) {
            //We pick the random object to assign to.
            short random = (short)(UnityEngine.Random.Range(0, objects.Length));
            //We get the sprite of the random object.
            Sprite randomSprite = objects[random].GetComponent<SpriteRenderer>().sprite;
            //We check if the object we have chose was already assigned.
            isDuplicate = false;
            for (short j = 0; j < sprites.Length; j++) {
                if (sprites[j] == randomSprite) {
                    isDuplicate = true;
                    i--;
                    break;
                }
            }
            if (isDuplicate == false) {
                dragAndDropImages[i].sprite = randomSprite;
                sprites[i] = dragAndDropImages[i].sprite;
                //DIFFICULTY IMPLEMENTATION
                objectInformation selectedObjectsObjectInformation = objects[random].GetComponent<objectInformation>();
                dragAndDropImageScripts[i].objectCount = (short)(UnityEngine.Random.Range(selectedObjectsObjectInformation.minimumAmount, (selectedObjectsObjectInformation.maximumAmount + 1)));
                dragAndDropImages[i].GetComponent<dragAndDropScript>().objectToPlace = objects[random];
            }
        }
        return;
    }

    public void giveMoreItems() {
        foreach (Image image in dragAndDropImages) {
            image.sprite = null;
        }
        for (short i = 0; i < sprites.Length; i++) {
            sprites[i] = null;
        }
        shuffleItems();
        return;
    }
}