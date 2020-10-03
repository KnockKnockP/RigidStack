#region Using tags.
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    #region Copying values from one class to another.
    //https://www.stackoverflow.com/a/36713403/
    public static void CopyAllTo<T>(this T source, T target) {
        Type type = typeof(T);
        foreach (PropertyInfo sourceProperty in type.GetProperties()) {
            PropertyInfo targetProperty = type.GetProperty(sourceProperty.Name);
            object temp = sourceProperty.GetValue(source);
            //Checking if the property has no set method.
            if (targetProperty.GetSetMethod() != null) {
                targetProperty.SetValue(target, temp);
            }
        }
        foreach (FieldInfo sourceField in type.GetFields()) {
            FieldInfo targetField = type.GetField(sourceField.Name);
            //Checking if the field is constant.
            if ((targetField.IsLiteral == true) && (targetField.IsInitOnly == false) == false) {
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }
    }
    #endregion
}
#endregion