using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonHandler : MonoBehaviour
{
    public void LoadNewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("StartingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("StartingScene");
    }
}
