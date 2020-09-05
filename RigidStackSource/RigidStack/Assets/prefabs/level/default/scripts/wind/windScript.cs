#region A using tag.
using UnityEngine;
#endregion

#region "MonoBehaviour" inherited "windScript" class.
public class windScript : MonoBehaviour {
    #region A variable for the wind.
    private float windStrength = 0.001f;
    #endregion

    #region Start function.
    private void Start() {
        switch (LoadedPlayerData.playerData.difficulty) {
            case (Difficulty.Sandbox) : {
                windStrength = 0.001f;
                break;
            }
            case (Difficulty.Easy) : {
                windStrength = 0.01f;
                break;
            }
            case (Difficulty.Moderate) : {
                windStrength = 0.05f;
                break;
            }
            case (Difficulty.Difficult) : {
                windStrength = 0.1f;
                break;
            }
            case (Difficulty.Extreme) : {
                windStrength = 0.2f;
                break;
            }
        }
        return;
    }
    #endregion

    #region Pushing objects out.
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("object") == true) {
            Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 newVelocity = new Vector3((rigidbody2D.velocity.x + windStrength), rigidbody2D.velocity.y);
            rigidbody2D.velocity = newVelocity;
        }
        return;
    }
    #endregion
}
#endregion