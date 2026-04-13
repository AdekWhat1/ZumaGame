using UnityEngine;

public class CameraFit : MonoBehaviour
{
    public SpriteRenderer background; // —юди перет€гни св≥й фон гри

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (background == null) return;

        // –озраховуЇмо необх≥дний розм≥р камери, щоб вл≥з весь фон
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = background.bounds.size.x / background.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = background.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = background.bounds.size.y / 2 * differenceInSize;
        }
    }
}