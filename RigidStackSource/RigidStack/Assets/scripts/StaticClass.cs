using UnityEngine;

public static class StaticClass {
    //Might add objectiveScript.cs's newObjectiveScoreAddition and newSecond = 15 to here.
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
}