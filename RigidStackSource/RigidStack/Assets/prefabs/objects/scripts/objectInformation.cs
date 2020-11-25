using Mirror;
using UnityEngine;

public class objectInformation : NetworkBehaviour {
    [Header("Variables for determining how many objects the player should get.")]
    public int minimumAmount = 0, maximumAmount = 0;

    //A variable to check if the object has been placed down.
    private Collider2D _collider2D;

    private void Awake() {
        _collider2D = GetComponent<Collider2D>();
        return;
    }

    private void Update() {
        if ((isServer == true) && (_collider2D.isTrigger == false) && (transform.position.y < -1f)) {
            FindObjectOfType<endMenuManager>().endGame();
        }
        return;
    }
}