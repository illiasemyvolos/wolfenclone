using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image transitionImage;
    public float fadeDuration = 1f;

    private void Start()
    {
        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(true);
            StartCoroutine(FadeOut());  // Scene starts by fading from black
        }
    }

    // Start Scene Transition
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionScene(sceneName));
    }

    IEnumerator TransitionScene(string sceneName)
    {
        yield return StartCoroutine(FadeIn());
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        transitionImage.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            transitionImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transitionImage.color = new Color(0f, 0f, 0f, 1f); // Ensure fully black
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            transitionImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transitionImage.color = new Color(0f, 0f, 0f, 0f); // Fully transparent
        transitionImage.gameObject.SetActive(false);  // Hide after fade-out
    }
}
