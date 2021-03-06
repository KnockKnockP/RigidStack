﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class objectEditingScript : MonoBehaviour {
    [NonSerialized] public static byte angle = 1;
    [SerializeField] private Text angleText = null;
    public Button confirmButton = null;
    [SerializeField] private Slider slider = null;

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

    #region Updating the angle.
    public void updateAngleValue() {
        slider.value = angle;
        makeText();
        return;
    }

    public void updateAngleValue(Slider _slider) {
        angle = (byte)(_slider.value);
        makeText();
        return;
    }

    public void updateAngleValue(Text inputText) {
        if (inputText.text.Length == 0) {
            inputText.text = ("0" + inputText.text);
        }
        if (byte.TryParse(inputText.text, out angle) == false) {
            return;
        }
        makeText();
        updateAngleValue();
        inputText.text = "";
        return;
    }

    private void makeText() {
        angleText.text = "";
        if (angle < 10) {
            angleText.text = "00";
        } else if (angle < 100) {
            angleText.text = "0";
        }
        angleText.text = (angleText.text + angle);
        return;
    }
    #endregion
}