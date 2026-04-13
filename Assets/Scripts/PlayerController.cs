using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Налаштування Стрільби")]
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Header("Налаштування Повороту")]
    public float rotationOffset = 0f;

    [Header("Спрайти Кульок")]
    public Sprite[] availableSprites;

    [Header("Візуалізація")]
    public SpriteRenderer mouthBallVisual;
    public SpriteRenderer nextBallIndicator;

    private int currentBallID;
    private int nextBallID;

    private ChainManager chainManager;

    void Start()
    {
        chainManager = FindFirstObjectByType<ChainManager>();
        PrepareFirstBalls();
    }

    void Update()
    {
        // БЛОКУВАННЯ: Якщо гра закінчена, жаба нічого не робить
        if (Time.timeScale == 0f) return;
        if (chainManager != null && chainManager.isGameOver) return;

        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0)) Shoot();
        if (Input.GetMouseButtonDown(1)) SwapBalls();
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = (objectPos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + rotationOffset));
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        // ЗВУК: Постріл
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.fireBall);

        GameObject newBullet = Instantiate(bulletPrefab, shootPoint.position, transform.rotation);
        Projectile projectileScript = newBullet.GetComponent<Projectile>();

        if (projectileScript != null)
            projectileScript.SetColor(currentBallID, availableSprites[currentBallID]);

        ReloadFrog();
    }

    void SwapBalls()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonClick();

        int tempID = currentBallID;
        currentBallID = nextBallID;
        nextBallID = tempID;
        UpdateVisuals();
    }

    void PrepareFirstBalls()
    {
        if (availableSprites.Length == 0) return;
        currentBallID = Random.Range(0, availableSprites.Length);
        nextBallID = Random.Range(0, availableSprites.Length);
        UpdateVisuals();
    }

    void ReloadFrog()
    {
        if (availableSprites.Length == 0) return;
        currentBallID = nextBallID;
        nextBallID = Random.Range(0, availableSprites.Length);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (mouthBallVisual != null) mouthBallVisual.sprite = availableSprites[currentBallID];
        if (nextBallIndicator != null) nextBallIndicator.sprite = availableSprites[nextBallID];
    }
}