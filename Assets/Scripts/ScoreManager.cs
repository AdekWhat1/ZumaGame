using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI Елементи")]
    public TextMeshProUGUI scoreText;
    public Image zumaBar;
    public GameObject victoryPanel;
    public GameObject lossPanel;
    public GameObject gameHUD;

    [Header("Налаштування Очок")]
    public float scoreToReachZuma = 2000f; // Змінено на float для плавного зменшення
    private float currentScore = 0f;       // Змінено на float
    private bool isGameEnded = false;

    [Header("Налаштування Деградації Бару")]
    public float timeBeforeDegradation = 3f; // Скільки секунд чекати перед початком зменшення
    public float degradationSpeed = 50f;     // Скільки очок віднімати за секунду
    private float lastScoreTime = 0f;        // Час, коли востаннє отримували очки

    private void Awake() => Instance = this;

    private void Start()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
        if (lossPanel) lossPanel.SetActive(false);

        lastScoreTime = Time.time; // Запускаємо таймер зі старту
        UpdateUI();
    }

    private void Update()
    {
        // Якщо гра закінчилась або ми вже виграли - нічого не робимо
        if (isGameEnded || currentScore >= scoreToReachZuma) return;

        // Перевіряємо, чи пройшло достатньо часу без нарахування очок
        if (Time.time - lastScoreTime > timeBeforeDegradation)
        {
            // Починаємо віднімати очки (плавно, залежно від Time.deltaTime)
            currentScore -= degradationSpeed * Time.deltaTime;

            // Не даємо очкам впасти нижче нуля
            if (currentScore < 0) currentScore = 0;

            UpdateUI();
        }
    }

    public void AddScore(int amount)
    {
        if (isGameEnded) return;

        currentScore += amount;
        lastScoreTime = Time.time; // Скидаємо таймер "бездіяльності", бо гравець молодець

        UpdateUI();

        // НОВА ЛОГІКА ПЕРЕМОГИ: Відразу перевіряємо, чи ми вже набили потрібну кількість!
        if (currentScore >= scoreToReachZuma)
        {
            ShowVictory();
        }
    }

    void UpdateUI()
    {
        // Округлюємо float до цілого числа (int) тільки для виводу тексту на екран
        if (scoreText) scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();

        if (zumaBar)
        {
            float fill = currentScore / scoreToReachZuma;
            zumaBar.fillAmount = Mathf.Clamp01(fill);
        }
    }

    public void ShowVictory()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        // Повідомляємо іншим скриптам (наприклад, ChainManager), що ми виграли
        if (ChainManager.Instance != null)
        {
            ChainManager.Instance.isWin = true;
            ChainManager.Instance.isGameOver = true;
        }

        Debug.Log("ScoreManager: АКТИВУЮ ВІКНО ПЕРЕМОГИ (Бар заповнено!)");
        if (gameHUD) gameHUD.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowLoss()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("ScoreManager: АКТИВУЮ ВІКНО ПРОГРАШУ");
        if (gameHUD) gameHUD.SetActive(false);
        if (lossPanel) lossPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public bool IsZumaReached() => currentScore >= scoreToReachZuma;
}