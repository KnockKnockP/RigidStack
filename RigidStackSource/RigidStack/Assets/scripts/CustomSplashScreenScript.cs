using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CustomSplashScreenScript : MonoBehaviour {
    [SerializeField] private string sceneToLoadAfterPlaying;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<SplashObject> splashObjects = new List<SplashObject>();

    private void Awake() {
        StartCoroutine(playAllSplashScreens());
        StartCoroutine(detectInput());
        return;
    }

    private IEnumerator playAllSplashScreens() {
        Camera camera = FindObjectOfType<Camera>();
        foreach (SplashObject splashObject in splashObjects) {
            if (splashObject.isSprite == true) {
                spriteRenderer.sprite = splashObject.spriteSplash;
            } else if (splashObject.isVideo == true) {
                videoPlayer.gameObject.SetActive(true);
                videoPlayer.clip = splashObject.videoSplash;
                videoPlayer.Play();
            }
            backgroundManager.resizeBackground(spriteRenderer.gameObject, false, camera);
            if (splashObject.splashEffect == SplashObject.SplashEffect.NoEffects) {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(splashObject.durationTime);
            } else if (splashObject.splashEffect == SplashObject.SplashEffect.FadeInAndFadeOut) {
                //Fade in.
                for (float i = 0; i <= 1; i = (i + Time.deltaTime)) {
                    spriteRenderer.color = new Color(1f, 1f, 1f, i);
                    yield return null;
                }
                //Wait.
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(splashObject.durationTime - 2);
                //Fade out.
                for (float i = 1; i >= 0; i = (i - Time.deltaTime)) {
                    spriteRenderer.color = new Color(1f, 1f, 1f, i);
                    yield return null;
                }
            }
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }
        spriteRenderer.gameObject.SetActive(false);
        changeScene();
        yield break;
    }

    private IEnumerator detectInput() {
        while (true) {
            if (Input.anyKey == true) {
                changeScene();
                yield break;
            }
            yield return null;
        }
    }

    private void changeScene() {
        SceneManager.LoadScene(sceneToLoadAfterPlaying);
        return;
    }
}