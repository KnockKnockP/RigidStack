#region Using tags.
using UnityEngine;
using UnityEngine.Video;
#endregion

#region "MonoBehaviour" inherited "televisionScript" class.
public class televisionScript : MonoBehaviour {
    #region Variables to control the video.
    [HideInInspector] public VideoPlayer videoPlayer;
    [HideInInspector] public SpriteRenderer videoPlayerSpriteRenderer;
    //[HideInInspector] public GameObject videoPlayerGameObject;
    #endregion

    #region Awake function.
    private void Awake() {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        videoPlayerSpriteRenderer = videoPlayer.gameObject.GetComponent<SpriteRenderer>();
        return;
    }
    #endregion

    #region Update function.
    private void Update() {
        videoPlayer.playbackSpeed = Time.timeScale;
        if (Time.timeScale == 0f) {
            videoPlayer.Pause();
            Destroy(this);
        }
        return;
    }
    #endregion
}
#endregion