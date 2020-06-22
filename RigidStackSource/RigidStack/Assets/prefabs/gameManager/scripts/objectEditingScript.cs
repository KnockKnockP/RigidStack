﻿using UnityEngine;
using UnityEngine.UI;

public class objectEditingScript : MonoBehaviour {
    [SerializeField] private Text angleText = null;

    public void confirmPlacement() {
        dragAndDropScript._dragAndDropScript.placeObject();
        return;
    }

    public void cancelPlacement() {
        dragAndDropScript._dragAndDropScript.cancelPlacingObject();
        return;
    }

    public void rotateLeft() {
        dragAndDropScript._dragAndDropScript.rotateLeft();
        return;
    }

    public void rotateRight() {
        dragAndDropScript._dragAndDropScript.rotateRight();
        return;
    }

    public void updateAngleValue(Slider slider) {
        StaticVariables.angle = (byte)(slider.value);
        angleText.text = "";
        if (StaticVariables.angle < 10) {
            angleText.text = "00";
        } else if (StaticVariables.angle < 100) {
            angleText.text = "0";
        }
        angleText.text = (angleText.text + StaticVariables.angle.ToString());
        return;
    }
}