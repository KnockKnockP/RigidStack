using UnityEngine;

public static class StaticClass {
    public static bool isDragging, isCollisionFishy;
    public static byte angle = 1;
    public static float cameraMovementSpeed = 0.1f;
    private static int _objectiveScore;
    public static int objectiveScore {
        get {
            return _objectiveScore;
        }
        set {
            _objectiveScore = value;
            Debug.Log("Objective score has been set to " + objectiveScore);
        }
    }

    public static Color Color32ToColor(byte r, byte g, byte b) {
        return new Color((r / 255), (g / 255), (b / 255));
    }

    public static Color Color32ToColor(byte r, byte g, byte b, byte a) {
        return new Color((r / 255), (g / 255), (b / 255), (a / 255));
    }
}