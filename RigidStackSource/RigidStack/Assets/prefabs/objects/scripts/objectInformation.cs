using UnityEngine;

public class objectInformation : MonoBehaviour {
    //Variables for determining how many objects the player should get.
    public int minimumAmount = 0, maximumAmount = 0;

    //A variable to check if the object has been placed down.
    [SerializeField] private postDragAndDropScript _postDragAndDropScript = null;

    private void Update() {
        if ((_postDragAndDropScript == null) && (transform.position.y < -1f)) {
            FindObjectOfType<endMenuManager>().endGame();
        }
        return;
    }
}