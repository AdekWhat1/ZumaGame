using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    public Transform[] waypoints;

    // Сюди запишемо, на якій відстані від старту знаходиться кожна точка
    // Наприклад: [0, 1.5, 3.2, 4.0 ...]
    private float[] cumulativeDistances;
    public float totalLength;

    void Awake()
    {
        CalculateLengths();
    }

    public void CalculateLengths()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        cumulativeDistances = new float[waypoints.Length];
        cumulativeDistances[0] = 0f;
        totalLength = 0f;

        // Проходимо по всіх точках і записуємо сумарну довжину
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float dist = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            totalLength += dist;
            cumulativeDistances[i + 1] = totalLength;
        }
    }

    // Найточніший метод пошуку позиції
    public Vector3 GetPointAtDistance(float dist)
    {
        // Обмеження: не виходити за межі дороги
        if (dist <= 0) return waypoints[0].position;
        if (dist >= totalLength) return waypoints[waypoints.Length - 1].position;

        // Шукаємо, між якими точками ми знаходимось
        for (int i = 0; i < cumulativeDistances.Length - 1; i++)
        {
            // Якщо наша дистанція менша, ніж дистанція наступної точки - ми знайшли відрізок!
            if (dist <= cumulativeDistances[i + 1])
            {
                // Ми між точкою [i] та [i+1]

                float startDist = cumulativeDistances[i];
                float endDist = cumulativeDistances[i + 1];

                // Рахуємо відсоток проходження саме цього шматочка (від 0 до 1)
                float t = (dist - startDist) / (endDist - startDist);

                return Vector3.Lerp(waypoints[i].position, waypoints[i + 1].position, t);
            }
        }

        return waypoints[waypoints.Length - 1].position;
    }

    [ContextMenu("Авто-заповнити точки")]
    void AutoFillPoints()
    {
        int childCount = transform.childCount;
        waypoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
        CalculateLengths();
        Debug.Log("Шлях перераховано! Довжина: " + totalLength);
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.color = Color.green;
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}