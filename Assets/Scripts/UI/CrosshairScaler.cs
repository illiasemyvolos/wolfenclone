using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CrosshairScaler : MonoBehaviour
{
    [Header("Scaling Settings")]
    public float baseSize = 50f;        // Default crosshair size when idle
    public float maxSize = 100f;        // Max size when fully spread
    public float scaleSpeed = 10f;      // How fast it interpolates between sizes
    public float shrinkDelay = 0.5f;    // Time before crosshair starts shrinking back

    private float currentSize;
    private float targetSize;
    private float timeSinceLastBump;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("❌ CrosshairScaler requires a RectTransform.");
            return;
        }

        currentSize = baseSize;
        targetSize = baseSize;
        rectTransform.sizeDelta = new Vector2(baseSize, baseSize);
    }

    private void Update()
    {
        // Track time since last bump
        timeSinceLastBump += Time.deltaTime;

        // Auto-return to base size after delay
        if (timeSinceLastBump >= shrinkDelay)
        {
            targetSize = baseSize;
        }

        // Smoothly interpolate current size toward target size
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * scaleSpeed);
        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

    /// <summary>
    /// Bumps the crosshair based on recoil intensity (0 to 1).
    /// </summary>
    public void Bump(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);
        float bumpSize = Mathf.Lerp(baseSize, maxSize, intensity);

        if (bumpSize > targetSize)
        {
            targetSize = bumpSize;
        }

        timeSinceLastBump = 0f; // Reset timer to prevent early shrink
    }

    public void SetSpreadPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);
        targetSize = Mathf.Lerp(baseSize, maxSize, percent);
    }

    public void ResetSize()
    {
        targetSize = baseSize;
        timeSinceLastBump = 0f;
    }
}