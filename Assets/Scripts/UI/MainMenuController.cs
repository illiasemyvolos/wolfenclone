using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Replace with your gameplay scene name
    }

    public void OpenSettings()
    {
        Debug.Log("⚙️ Open Settings (To Be Implemented)");
        // Add settings logic here
    }

    public void QuitGame()
    {
        Debug.Log("❌ Quit Game");
        Application.Quit();
    }
}
