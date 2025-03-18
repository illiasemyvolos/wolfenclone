using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageScreenController : MonoBehaviour
{
    [Header("UI Reference")]
    public Image damageScreen;      // Transparent red image
    public float fadeDuration = 0.5f;  // Time before the red fades out

    private Coroutine fadeCoroutine;

    private void Start()
    {
        damageScreen.color = new Color(1f, 0f, 0f, 0f); // Ensure fully transparent at start
    }

    public void ShowDamageEffect()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        damageScreen.color = new Color(1f, 0f, 0f, 0.5f); // Show red flash (50% alpha)

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, timer / fadeDuration);
            damageScreen.color = new Color(1f, 0f, 0f, alpha);

            yield return null;
        }

        damageScreen.color = new Color(1f, 0f, 0f, 0f); // Ensure fully transparent at the end
    }
}
