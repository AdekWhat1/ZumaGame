using UnityEngine;
using UnityEngine.SceneManagement; // Обов'язково для роботи зі сценами!

public class MainMenuManager : MonoBehaviour
{
    [Header("GitHub Settings")]
    [Header("Scene Settings")]
    [Tooltip("Точна назва сцени з твоєю грою")]
    public string githubURL = "https://github.com/твій_логін/твій_проект";
    public string gameSceneName = "GameScene"; // <-- Тут має бути назва твоєї ігрової сцени!

    // Цю функцію ми підключимо до кнопки PLAY
    public void StartGame()
    {
        Debug.Log("Запуск рівня: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    // Цю функцію можна підключити до кнопки виходу (якщо захочеш її додати)
    public void QuitGame()
    {
        Debug.Log("Вихід з гри!");
        Application.Quit();
    }
    public void OpenGitHub()
    {
        // Ця команда відкриває браузер за замовчуванням на вказаній сторінці
        Application.OpenURL(githubURL);
        Debug.Log("Відкриваємо GitHub: " + githubURL);
    }
}   
