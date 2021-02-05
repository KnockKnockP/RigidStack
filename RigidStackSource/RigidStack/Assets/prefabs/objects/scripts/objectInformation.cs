using Mirror;
using UnityEngine;

public class objectInformation : NetworkBehaviour {
    public int minimumAmount = 0, maximumAmount = 0;

    [SerializeField] protected SpriteRenderer spriteRenderer = null;
    [SerializeField] protected Collider2D _collider2D = null;
    [SerializeField] private Rigidbody2D _rigidBody2D = null;
    public Vector2 rectSize;

    protected static readonly Color32 dimmedColor = new Color32(50, 50, 50, 255);
    public delegate void DimDelegate();
    public DimDelegate dimDelegate = null;
    public delegate void UnDimDelegate();
    public UnDimDelegate unDimDelegate = null;

    protected static readonly Color32 highlightedColor = new Color32(255, 0, 0, 255);
    public delegate void HighlightDelegate();
    public HighlightDelegate highlightDelegate = null;
    public delegate void UnHighlightDelegate();
    public UnHighlightDelegate unHighlightDelegate = null;

    private void OnValidate() {
#if UNITY_EDITOR
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (_collider2D == null) {
            _collider2D = GetComponent<Collider2D>();
        }
        if (_rigidBody2D == null) {
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }
#endif
        return;
    }

    private void Update() {
        if ((isServer == true) && (_collider2D.isTrigger == false) && (transform.position.y < -1f)) {
            FindObjectOfType<endMenuManager>().endGame();
        }
        return;
    }

    public virtual void Dim() {
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteRenderer.color = dimmedColor;
        return;
    }

    public virtual void UnDim() {
        spriteRenderer.color = Color.white;
        return;
    }

    public virtual void Highlight() {
        spriteRenderer.color = highlightedColor;
        return;
    }

    public virtual void UnHighlight() {
        spriteRenderer.color = Color.white;
        return;
    }
}