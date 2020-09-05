#region A using tag.
using UnityEngine;
#endregion

#region Static "StaticClass" class.
public static class StaticClass {
    #region Checking values.
    public static bool isInBetweenOfTwoValues(byte value, byte least, byte max) {
        return ((value > least) && (value < max));
    }

    public static bool isInBetweenOfTwoValues(int value, int least, int max) {
        return ((value > least) && (value < max));
    }

    public static bool isInBetweenOfTwoValues(float value, float least, float max) {
        return((value > least) && (value < max));
    }

    public static bool isInBetweenOfTwoValues(Vector2 value, float least, float max) {
        return ((value.x > least) && (value.x < max) && (value.y > least) && (value.y < max));
    }

    public static bool isInBetweenOrEqualToTwoValues(byte value, byte least, byte max) {
        return ((value >= least) && (value <= max));
    }

    public static bool isInBetweenOrEqualToTwoValues(int value, int least, int max) {
        return ((value >= least) && (value <= max));
    }

    public static bool isInBetweenOrEqualToTwoValues(float value, float least, float max) {
        return ((value >= least) && (value <= max));
    }

    public static bool isInBetweenOrEqualToTwoValues(Vector2 value, float least, float max) {
        return ((value.x >= least) && (value.x <= max) && (value.y >= least) && (value.y <= max));
    }
    #endregion
}
#endregion