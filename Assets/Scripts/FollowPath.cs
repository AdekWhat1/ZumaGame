using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path roadPath;
    public float distanceTraveled = 0f;

    // Кулька просто чекає, поки їй скажуть, де стати
    public void SetPositionByDistance(float dist)
    {
        distanceTraveled = dist;

        if (roadPath != null)
        {
            // Запитуємо у дороги точні координати
            transform.position = roadPath.GetPointAtDistance(distanceTraveled);

            // --- ДОДАЄМО ПОВОРОТ (Щоб жаба бачила, куди котиться обличчя) ---
            // Дивимось трохи вперед (+0.1 одиниці), щоб зрозуміти напрямок
            Vector3 nextPos = roadPath.GetPointAtDistance(distanceTraveled + 0.1f);
            Vector3 direction = nextPos - transform.position;

            // Повертаємо спрайт (Z-axis rotation для 2D)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}