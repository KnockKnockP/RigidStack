using UnityEngine;
using UnityEngine.Video;

public class televisionScript : objectInformation {
    [Header("Television script.")] public VideoPlayer videoPlayer;
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

    private void Awake() {
        dimDelegate = Dim;
        unDimDelegate = UnDim;
        highlightDelegate = Highlight;
        unHighlightDelegate = UnHighlight;
        return;
    }

    private void Update() {
        videoPlayer.playbackSpeed = Time.timeScale;
        if (Time.timeScale == 0f) {
            videoPlayer.Pause();
        }
        return;
    }

    public override void Dim() {
        base.Dim();
        videoPlayerSpriteRenderer.color = dimmedColor;
        videoPlayer.Pause();
        return;
    }

    public override void UnDim() {
        base.UnDim();
        videoPlayerSpriteRenderer.color = Color.white;
        videoPlayer.Play();
    }

    public override void Highlight() {
        base.Highlight();
        videoPlayerSpriteRenderer.color = highlightedColor;
        return;
    }

    public override void UnHighlight() {
        base.UnHighlight();
        videoPlayerSpriteRenderer.color = Color.white;
        return;
    }
}