using UnityEngine;

public class floorScript : MonoBehaviour {
    //A variable for ending the game.
    [SerializeField] private endMenuManager _endMenuManager = null;

    #region Checking the collision.
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("platform") == false) {
            _endMenuManager.endGame();
            Destroy(this);
        }
        return;
    }

    private void OnTriggerEnter2D() {
        postDragAndDropScript.isCollisionFishy = true;
        return;
    }

    private void OnTriggerExit2D() {
        postDragAndDropScript.isCollisionFishy = false;
        return;
    }
    #endregion
}