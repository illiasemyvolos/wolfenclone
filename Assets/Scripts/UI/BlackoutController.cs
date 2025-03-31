using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackoutController : MonoBehaviour
{
    public static BlackoutController Instance { get; private set; }

    [SerializeField] private CanvasGroup blackoutCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    public bool IsFading { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        if (blackoutCanvasGroup == null)
        {
            Debug.LogWarning("BlackoutCanvasGroup is missing or destroyed during fade.");
            yield break;
        }

        IsFading = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            if (blackoutCanvasGroup != null)
            {
                blackoutCanvasGroup.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            }
            else
            {
                Debug.LogWarning("CanvasGroup destroyed mid-fade.");
                IsFading = false;
                yield break;
            }

            yield return null;
        }

        if (blackoutCanvasGroup != null)
            blackoutCanvasGroup.alpha = to;

        IsFading = false;
    }
}