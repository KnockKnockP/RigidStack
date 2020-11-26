using UnityEngine;

[CreateAssetMenu(fileName = "DayBackgrounds.", menuName = "ScriptableObjects/DayBackgrounds.")]
public class DayBackgrounds : ScriptableObject {
    public Background morningBackground, afternoonBackground, nightBackground;
}