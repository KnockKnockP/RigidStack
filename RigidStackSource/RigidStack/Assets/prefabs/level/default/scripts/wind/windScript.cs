using UnityEngine;

public class windScript : MonoBehaviour {
    private float windStrength;

    private void Awake() {
        switch (LoadedPlayerData.playerData.difficulty) {
            case (Difficulty.Sandbox): {
                windStrength = 0.02f;
                break;
            }
            case (Difficulty.Easy): {
                windStrength = 0.1f;
                break;
            }
            case (Difficulty.Moderate): {
                windStrength = 0.2f;
                break;
            }
            case (Difficulty.Difficult): {
                windStrength = 0.4f;
                break;
            }
            case (Difficulty.Extreme): {
                windStrength = 0.5f;
                break;
            }
        }
        return;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("object") == true) {
            Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rigidbody2D.velocity;
            velocity.x = (velocity.x + windStrength);
            rigidbody2D.velocity = velocity;
        }
        return;
    }
}