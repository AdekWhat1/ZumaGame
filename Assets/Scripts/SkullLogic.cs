using UnityEngine;

public class SkullLogic : MonoBehaviour
{
    public SpriteRenderer skullRenderer;

    [Tooltip("Кадри черепа: від закритого [0] до повністю відкритого [останній]")]
    public Sprite[] skullFrames;

    [Tooltip("За скільки одиниць до кінця шляху череп починає відкривати рот")]
    public float dangerDistance = 5f;

    // Цю функцію буде викликати ChainManager кожен кадр
    public void UpdateMouth(float distanceToHole)
    {
        if (skullFrames.Length == 0 || skullRenderer == null) return;

        // Якщо кулька ще далеко (безпечно) - рот закритий
        if (distanceToHole > dangerDistance)
        {
            skullRenderer.sprite = skullFrames[0];
            return;
        }

        // Якщо близько - вираховуємо відсоток відкритості (від 0.0 до 1.0)
        float openness = 1f - (distanceToHole / dangerDistance);

        // Переводимо відсоток у конкретний індекс кадру
        int frameIndex = Mathf.FloorToInt(openness * skullFrames.Length);

        // Захист, щоб не вийти за межі масиву
        frameIndex = Mathf.Clamp(frameIndex, 0, skullFrames.Length - 1);

        // Підставляємо потрібний спрайт
        skullRenderer.sprite = skullFrames[frameIndex];
    }
}