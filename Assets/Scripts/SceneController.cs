using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Для роботи зі слайдерами

public class SceneController : MonoBehaviour
{
    [Header("Панелі")]
    public GameObject pauseMenuPanel;

    [Header("Налаштування (Повзунки)")]
    public Slider volumeSlider;
    public Slider speedSlider;

    [Header("Посилання на менеджери")]
    public ChainManager chainManager;

    private bool isPaused = false;

    void Start()
    {
        // Встановлюємо початкове значення гучності
        if (volumeSlider != null)
            volumeSlider.value = AudioListener.volume;

        // Встановлюємо початкове значення швидкості з твого ChainManager
        if (speedSlider != null && chainManager != null)
            speedSlider.value = chainManager.speed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Метод для зміни гучності
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    // Метод для зміни швидкості кульок (BallSpeed)
    public void SetGameSpeed(float value)
    {
        if (chainManager != null)
        {
            chainManager.speed = value;
        }
    }

    // Методи навігації
    public void StartGame() { Time.timeScale = 1f; SceneManager.LoadScene(1); }
    public void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
        }

        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Debug.Log("Вихід з гри..."); // Це для перевірки в консолі
        Application.Quit(); // Ця команда закриває скомпільовану гру (.exe)
    }
}