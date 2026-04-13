using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    void Start()
    {
        // «нищуЇ цей об'Їкт (ан≥мац≥ю вибуху) через 0.5 секунд.
        // якщо тво€ ан≥мац≥€ триваЇ довше або коротше, п≥дкоригуй значенн€ 0.5f
        Destroy(gameObject, 0.5f);
    }
}