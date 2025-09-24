using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string gameSceneName = "SampleScene";

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


