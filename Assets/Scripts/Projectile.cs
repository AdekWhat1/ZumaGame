using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public int finalColorID;

    private ChainManager chainManager;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        chainManager = FindFirstObjectByType<ChainManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public void SetColor(int id, Sprite sprite)
    {
        finalColorID = id;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBall"))
        {
            // ÇÂÓÊ: Âëó÷àííÿ (Êë³ê)
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.ballClick);

            if (chainManager != null)
                chainManager.InsertBall(other.gameObject, finalColorID);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }
}