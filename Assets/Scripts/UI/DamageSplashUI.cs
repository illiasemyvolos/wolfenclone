using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageSplashUI : MonoBehaviour
{
    [SerializeField] private Image splashImage;
    [SerializeField] private float fadeInDuration = 0.05f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float maxAlpha = 0.5f;

    private Coroutine currentCoroutine;

    public void PlayDamageEffect()
    {
        Debug.Log("✅ Damage splash triggered!");

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(DoSplash());
    }

    private IEnumerator DoSplash()
    {
        Color color = splashImage.color;
        float t = 0f;

        // Fade in
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, maxAlpha, t / fadeInDuration);
            splashImage.color = color;
            Debug.Log($"Fade In Alpha: {color.a}");
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0f, t / fadeOutDuration);
            splashImage.color = color;
            Debug.Log($"Fade Out Alpha: {color.a}");
            yield return null;
        }

        splashImage.color = new Color(color.r, color.g, color.b, 0f);
        currentCoroutine = null;
    }
}