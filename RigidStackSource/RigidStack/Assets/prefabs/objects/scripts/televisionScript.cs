using System;
using UnityEngine;
using UnityEngine.Video;

public class televisionScript : MonoBehaviour {
    //Variables to control the video.
    [NonSerialized] public VideoPlayer videoPlayer;
    [NonSerialized] public SpriteRenderer videoPlayerSpriteRenderer;

    private void Awake() {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        videoPlayerSpriteRenderer = videoPlayer.gameObject.GetComponent<SpriteRenderer>();
        return;
    }

    private void Update() {
        videoPlayer.playbackSpeed = Time.timeScale;
        if (Time.timeScale == 0f) {
            videoPlayer.Pause();
            Destroy(this);
        }
        return;
    }
}