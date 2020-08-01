using UnityEngine;
using UnityEngine.UI;

public class objectEditingScript : MonoBehaviour {
    [HideInInspector] public static byte angle = 1;

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
}