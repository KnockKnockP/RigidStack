using UnityEngine;

public class windScript : MonoBehaviour {
    //DIFFICULTY IMPLEMENTATION
    public static float windStrength = 0.1f;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("object") == true) {
            Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 newVelocity = new Vector3((rigidbody2D.velocity.x + windStrength), rigidbody2D.velocity.y);
            rigidbody2D.velocity = newVelocity;
        }
        return;
    }
}