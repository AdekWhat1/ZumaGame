using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour
{
    public float timeToLive = 1.0f;

    void Start()
    {
        // Використовуємо корутину, яка ігнорує паузу гри
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        // Чекаємо в реальних секундах (навіть якщо Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(timeToLive);
        Destroy(gameObject);
    }
}