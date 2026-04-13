using UnityEngine;
using System.Collections.Generic;

public class ChainManager : MonoBehaviour
{
    public Path roadPath;
    public GameObject ballPrefab;

    [Header("Ефекти")]
    public GameObject explosionPrefab;

    [Header("Череп (Кінець гри)")]
    public SkullLogic skull;
    public float totalPathLength = 50f;

    [Header("Рівень")]
    public int totalBallsInLevel = 30;
    private int ballsSpawned = 0;

    [Header("Стан гри")]
    public bool isGameOver = false;
    public bool isWin = false;
    public float gameOverSpeed = 15f;

    public List<FollowPath> balls = new List<FollowPath>();

    [Header("Налаштування руху")]
    public float speed = 2f;
    public float pullbackSpeed = 5f;
    public float ballSpacing = 0.6f;

    private float lastInsertTime = 0f;
    private bool hasGap = false;
    private int gapIndex = -1;
    private bool isPullingBack = false;

    public static ChainManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 1. ПЕРЕВІРКА НА ПЕРЕМОГУ (Вона має бути абсолютно першою)
        if (ballsSpawned >= totalBallsInLevel && balls.Count == 0 && !isGameOver)
        {
            HandleWin();
            return;
        }

        // 2. РЕЖИМ КІНЦЯ ГРИ (Якщо вже програли — засмоктуємо залишки)
        if (isGameOver)
        {
            HandleGameOverMovement();
            return;
        }

        // 3. СПАВН КУЛЬОК
        if (ballsSpawned < totalBallsInLevel)
        {
            bool zumaReached = (ScoreManager.Instance != null) ? ScoreManager.Instance.IsZumaReached() : false;
            if (!zumaReached)
            {
                if (balls.Count == 0 || balls[balls.Count - 1].distanceTraveled >= ballSpacing)
                {
                    SpawnBall();
                    ballsSpawned++;
                }
            }
        }

        // 4. БЕЗПЕЧНА ЛОГІКА РУХУ ТА ПРОГРАШУ
        // Запускаємо тільки якщо в списку реально є кульки
        if (balls.Count > 0)
        {
            HandleRegularMovement();

            // Перевірка програшу: чи не зникли кульки за цей кадр і чи доїхала перша
            if (!isGameOver && balls.Count > 0 && balls[0] != null)
            {
                if (balls[0].distanceTraveled >= totalPathLength)
                {
                    TriggerGameOver();
                    return;
                }
            }

            UpdatePhysicalPositions();

            // Оновлення черепа також захищене від помилок
            if (skull != null && balls.Count > 0 && balls[0] != null)
            {
                float distanceToSkull = totalPathLength - balls[0].distanceTraveled;
                skull.UpdateMouth(distanceToSkull);
            }
        }
    }

    void HandleWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("--- ЛОГІКА: ПЕРЕМОГА ---");
        Invoke("DelayedVictory", 0.5f);
    }

    void DelayedVictory()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.ShowVictory();
    }

    void HandleRegularMovement()
    {
        if (hasGap)
        {
            balls[gapIndex].distanceTraveled += speed * Time.deltaTime;
            UpdateSegmentPositions(gapIndex, balls.Count - 1);

            if (isPullingBack && gapIndex > 0)
            {
                balls[gapIndex - 1].distanceTraveled -= pullbackSpeed * Time.deltaTime;
                UpdateSegmentPositionsBackwards(gapIndex - 1, 0);
            }

            if (gapIndex > 0 && (balls[gapIndex - 1].distanceTraveled - balls[gapIndex].distanceTraveled) <= ballSpacing)
            {
                balls[gapIndex - 1].distanceTraveled = balls[gapIndex].distanceTraveled + ballSpacing;
                hasGap = false;
                isPullingBack = false;
                CheckMatches(gapIndex - 1);
            }
        }
        else
        {
            balls[0].distanceTraveled += speed * Time.deltaTime;
            UpdateSegmentPositions(0, balls.Count - 1);
        }
    }

    void HandleGameOverMovement()
    {
        if (balls.Count > 0)
        {
            balls[0].distanceTraveled += gameOverSpeed * Time.deltaTime;
            UpdateSegmentPositions(0, balls.Count - 1);

            if (balls[0].distanceTraveled >= totalPathLength)
            {
                Destroy(balls[0].gameObject);
                balls.RemoveAt(0);
                hasGap = false;
            }
            UpdatePhysicalPositions();
        }
        else
        {
            if (skull != null) skull.UpdateMouth(50f);
            if (ScoreManager.Instance != null) ScoreManager.Instance.ShowLoss();
        }
    }

    void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("--- ЛОГІКА: ПРОГРАШ ---");

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.choral1);
    }

    public void CheckMatches(int index)
    {
        int targetColor = GetBallColor(index);
        if (targetColor == -1) return;

        int startIndex = index;
        int endIndex = index;

        while (startIndex > 0 && GetBallColor(startIndex - 1) == targetColor) startIndex--;
        while (endIndex < balls.Count - 1 && GetBallColor(endIndex + 1) == targetColor) endIndex++;

        int count = endIndex - startIndex + 1;

        if (count >= 3)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayRandomDestruction();

            // НОВЕ НАРАХУВАННЯ ОЧОК: 20 за кульку + прогресивний бонус
            int pointsEarned = (count * 20) + (count > 3 ? (count - 3) * 30 : 0);
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(pointsEarned);

            for (int i = startIndex; i <= endIndex; i++)
            {
                // 1. ЗАПАМ'ЯТОВУЄМО позицію ДО того, як сховаємо або видалимо кульку
                Vector3 explosionPos = balls[i].transform.position;

                // 2. МИТТЄВО ховаємо кульку
                balls[i].gameObject.SetActive(false);

                // 3. Створюємо вибух на збережених координатах (а не в 0, 0, 0)
                if (explosionPrefab != null)
                {
                    Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
                }

                // 4. Тільки тепер остаточно видаляємо
                Destroy(balls[i].gameObject);
            }

            balls.RemoveRange(startIndex, count);

            // ПЕРЕВІРКА НА ПЕРЕМОГУ ПІСЛЯ ВИБУХУ
            if (balls.Count == 0 && ballsSpawned >= totalBallsInLevel)
            {
                HandleWin();
                return;
            }

            if (balls.Count > 0 && startIndex > 0 && startIndex < balls.Count)
            {
                hasGap = true;
                gapIndex = startIndex;
                isPullingBack = (GetBallColor(startIndex - 1) == GetBallColor(startIndex));
            }
        }
    }

    void SpawnBall()
    {
        GameObject newBallObj = Instantiate(ballPrefab);
        newBallObj.transform.SetParent(transform);
        FollowPath ballScript = newBallObj.GetComponent<FollowPath>();
        ballScript.roadPath = roadPath;

        BallLogic logic = newBallObj.GetComponent<BallLogic>();
        if (logic != null)
        {
            int randomId = Random.Range(0, logic.availableSprites.Length);
            logic.SetColor(randomId);
        }

        if (balls.Count > 0)
            ballScript.distanceTraveled = balls[balls.Count - 1].distanceTraveled - ballSpacing;
        else
            ballScript.distanceTraveled = 0;

        balls.Add(ballScript);
    }

    public void InsertBall(GameObject hitBall, int ballId)
    {
        if (isGameOver) return;
        if (Time.time - lastInsertTime < 0.1f) return;
        lastInsertTime = Time.time;

        FollowPath hitBallScript = hitBall.GetComponent<FollowPath>();
        int hitIndex = balls.IndexOf(hitBallScript);
        if (hitIndex == -1) return;

        GameObject newBall = Instantiate(ballPrefab);
        newBall.transform.SetParent(transform);
        FollowPath newBallScript = newBall.GetComponent<FollowPath>();
        BallLogic newBallLogic = newBall.GetComponent<BallLogic>();

        if (newBallLogic != null) newBallLogic.SetColor(ballId);

        newBallScript.roadPath = roadPath;
        newBallScript.distanceTraveled = hitBallScript.distanceTraveled;

        balls.Insert(hitIndex, newBallScript);
        UpdateSegmentPositions(hitIndex, balls.Count - 1);
        CheckMatches(hitIndex);
    }

    void UpdatePhysicalPositions()
    {
        foreach (var ball in balls)
        {
            if (ball != null) ball.SetPositionByDistance(ball.distanceTraveled);
        }
    }

    void UpdateSegmentPositions(int startIndex, int endIndex)
    {
        for (int i = startIndex + 1; i <= endIndex; i++)
            balls[i].distanceTraveled = balls[i - 1].distanceTraveled - ballSpacing;
    }

    void UpdateSegmentPositionsBackwards(int startIndex, int endIndex)
    {
        for (int i = startIndex - 1; i >= endIndex; i--)
            balls[i].distanceTraveled = balls[i + 1].distanceTraveled + ballSpacing;
    }

    int GetBallColor(int index)
    {
        if (index < 0 || index >= balls.Count) return -1;
        BallLogic logic = balls[index].GetComponent<BallLogic>();
        return (logic != null) ? logic.BallID : -1;
    }
}