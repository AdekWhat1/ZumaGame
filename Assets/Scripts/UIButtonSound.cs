using UnityEngine;
using UnityEngine.EventSystems; // Обов'язково для відстеження мишки

// Додаємо інтерфейси для реакції на наведення та клік
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Звуки кнопки")]
    public AudioClip hoverSound; // Звук при наведенні
    public AudioClip clickSound; // Звук при кліку

    // Цей метод автоматично викликається, коли курсор миші заходить на кнопку
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(hoverSound);
        }
    }

    // Цей метод автоматично викликається при натисканні
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(clickSound);
        }
    }
}