using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class objectScript : NetworkBehaviour {
    //A variable for syncing dragAndDropObjects.
    private bool isSynced = false;
    //Variables for drag and dropping objects.
    private readonly dragAndDropImageScript[] dragAndDropImageScripts = new dragAndDropImageScript[5];
    [SerializeField] private GameObject[] dragAndDropObjects = null;
    //We are going to use this array to check for duplicates.
    private readonly Sprite[] sprites = new Sprite[5];
    [SerializeField] private GameObject[] objects = null;

    private void Start() {
        for (short i = 0; i < dragAndDropObjects.Length; i++) {
            dragAndDropImageScripts[i] = dragAndDropObjects[i].GetComponent<dragAndDropImageScript>();
        }
        if (isServer == true) {
            resetRandomization();
            shuffleItems();
        }
        return;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        //We ensure it only gets called once.
        if ((isClientOnly == true) && (isSynced == false)) {
            isSynced = true;
            commandGiveMoreItems();
        }
    }

    [Command(ignoreAuthority = true)]
    private void commandGiveMoreItems() {
        giveMoreItems();
        return;
    }

    [Server]
    private void resetRandomization() {
        UnityEngine.Random.InitState(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second * DateTime.Now.Millisecond);
        return;
    }

    [Server]
    public void giveMoreItems() {
        for (short i = 0; i < sprites.Length; i++) {
            dragAndDropObjects[i].GetComponent<Image>().sprite = null;
            sprites[i] = null;
        }
        shuffleItems();
        return;
    }

    [Server]
    private void shuffleItems() {
        cancelPlacingObject();
        bool isDuplicate;
        for (short i = 0; i < dragAndDropObjects.Length; i++) {
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
                Image image = dragAndDropObjects[i].GetComponent<Image>();
                image.sprite = randomSprite;
                sprites[i] = image.sprite;
                objectInformation selectedObjectsObjectInformation = objects[random].GetComponent<objectInformation>();
                dragAndDropImageScripts[i].objectCount = (short)(UnityEngine.Random.Range(selectedObjectsObjectInformation.minimumAmount, (selectedObjectsObjectInformation.maximumAmount + 1)));
                dragAndDropObjects[i].GetComponent<dragAndDropScript>().objectToPlace = objects[random];
                updateDock(i, random, dragAndDropImageScripts[i].objectCount);
            }
        }
        return;
    }

    [ClientRpc]
    private void cancelPlacingObject() {
        if (dragAndDropScript._dragAndDropScript != null) {
            dragAndDropScript._dragAndDropScript.cancelPlacingObject();
        }
        return;
    }

    [ClientRpc]
    private void updateDock(int dragAndDropObjectIndex, int objectIndex, short objectCount) {
        dragAndDropObjects[dragAndDropObjectIndex].GetComponent<Image>().sprite = objects[objectIndex].GetComponent<SpriteRenderer>().sprite;
        dragAndDropObjects[dragAndDropObjectIndex].GetComponent<dragAndDropImageScript>().objectCount = objectCount;
        dragAndDropObjects[dragAndDropObjectIndex].GetComponent<dragAndDropScript>().objectToPlace = objects[objectIndex];
        return;
    }
}