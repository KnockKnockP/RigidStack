using UnityEngine;

public class objectInformation : MonoBehaviour {
    public int minimumAmount = 0, maximumAmount = 0;


    [SerializeField] private postDragAndDropScript _postDragAndDropScript = null;

    private void Update() {
        if ((_postDragAndDropScript == null) && (transform.position.y < -1f)) {
            FindObjectOfType<endMenuManager>().endGame();
        }
        return;
    }
}