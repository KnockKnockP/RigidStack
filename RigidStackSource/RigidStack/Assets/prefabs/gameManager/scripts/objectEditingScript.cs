#region Using tags.
using UnityEngine;
using UnityEngine.UI;
#endregion

#region "MonoBehaviour" inherited "objectEditingScript" class.
public class objectEditingScript : MonoBehaviour {
    #region Variables for angle management.
    [HideInInspector] public static byte angle = 1;
    [SerializeField] private Text angleText = null;
    public Button confirmButton = null;
    [SerializeField] private Slider slider = null;
    #endregion

    #region Confirming object placement.
    public void confirmPlacement() {
        dragAndDropScript._dragAndDropScript.placeObject();
        return;
    }
    #endregion

    #region Cancelling object placement.
    public void cancelPlacement() {
        dragAndDropScript._dragAndDropScript.cancelPlacingObject();
        return;
    }
    #endregion

    #region Rotating the object to the left.
    public void rotateLeft() {
        dragAndDropScript._dragAndDropScript.rotateLeft();
        return;
    }
    #endregion

    #region Rotating the object to the right.
    public void rotateRight() {
        dragAndDropScript._dragAndDropScript.rotateRight();
        return;
    }
    #endregion

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
        angleText.text = (angleText.text + angle.ToString());
        return;
    }
    #endregion
}
#endregion