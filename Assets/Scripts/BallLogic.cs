using UnityEngine;

public class BallLogic : MonoBehaviour
{
    public Sprite[] availableSprites;
    public int BallID = -1;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Ми ПРИБРАЛИ Start(). Тепер кулька ніколи не змінює колір сама.
    // Вона чекає наказу від Менеджера.

    public void SetColor(int id)
    {
        if (availableSprites == null || availableSprites.Length == 0) return;

        BallID = id;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (BallID >= 0 && BallID < availableSprites.Length)
        {
            spriteRenderer.sprite = availableSprites[BallID];
            spriteRenderer.color = Color.white; // Страховка від "сірого" кольору
        }
    }
}