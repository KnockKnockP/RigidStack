using UnityEngine;

public class floorScript : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("platform") == false) {
            FindObjectOfType<objectScript>().reset();
        }
        return;
    }

    private void OnTriggerEnter2D() {
        StaticVariables.isCollisionFishy = true;
        return;
    }

    private void OnTriggerExit2D() {
        StaticVariables.isCollisionFishy = false;
        return;
    }
}