using System;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SplashObject.", menuName = "ScriptableObjects/SplashObject.")]
public class SplashObject : ScriptableObject {
    [NonSerialized] public bool isSprite, isVideo;
    [Range(2f, 10f)] public float durationTime = 2f;
    public SplashEffect splashEffect;
    public Sprite spriteSplash;
    public VideoClip videoSplash;

    public enum SplashEffect : byte {
        NoEffects = 0,
        FadeInAndFadeOut = 1
    }

    private void OnValidate() {
        bool _isSprite = (spriteSplash != null), _isVideo = (videoSplash != null);
        if ((_isSprite == true) && (_isVideo == true)) {
            Debug.LogWarning(this + " can not be both sprite and video.");
            spriteSplash = null;
            videoSplash = null;
        } else {
            isSprite = _isSprite;
            isVideo = _isVideo;
        }
        return;
    }
}