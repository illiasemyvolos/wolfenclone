using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string persistentScene = "GameManager";
    [SerializeField] private string mainMenuScene = "MainMenu";

    private void Start()
    {
        StartCoroutine(LoadStartupScenes());
    }

    private IEnumerator LoadStartupScenes()
    {
        // Load GameManager (persistent systems)
        if (!SceneManager.GetSceneByName(persistentScene).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(persistentScene, LoadSceneMode.Additive);
        }

        // Load MainMenu scene
        if (!SceneManager.GetSceneByName(mainMenuScene).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
        }

        // Unload bootstrap scene itself
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
    
   
}