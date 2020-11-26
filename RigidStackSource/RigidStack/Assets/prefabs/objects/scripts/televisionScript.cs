using UnityEngine;
using UnityEngine.Video;

public class televisionScript : MonoBehaviour {
    //Variables to control the video.
    public VideoPlayer videoPlayer;
    public SpriteRenderer videoPlayerSpriteRenderer;

    private void OnValidate() {
#if UNITY_EDITOR
        if (videoPlayer == null) {
            videoPlayer = GetComponentInChildren<VideoPlayer>();
        }
        if (videoPlayerSpriteRenderer == null) {
            videoPlayerSpriteRenderer = videoPlayer.GetComponent<SpriteRenderer>();
        }
#endif
        return;
    }

    private void Update() {
        videoPlayer.playbackSpeed = Time.timeScale;
        if (Time.timeScale == 0f) {
            videoPlayer.Pause();
            enabled = false;
        }
        return;
    }
}