using UnityEngine;

public class windScript : MonoBehaviour {
    //DIFFICULTY IMPLEMENTATION
    public static int windStrength = 5;

    private void OnCollisionStay2D(Collision2D collision) {
        Debug.Log("Here 1.");
        if (collision.gameObject.CompareTag("object") == true) {
            Rigidbody2D rigidbody2D = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 newVelocity = new Vector3((rigidbody2D.velocity.x + windStrength), rigidbody2D.velocity.y);
            rigidbody2D.velocity = newVelocity;
        }
        return;
    }
}