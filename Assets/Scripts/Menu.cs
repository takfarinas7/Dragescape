using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string gameSceneName = "SampleScene";
    public AudioSource ButtonSound;
    public AudioSource MenuMusic;

    void Start()
    {
        if (MenuMusic != null && !MenuMusic.isPlaying)
        {
            MenuMusic.loop = true;
            MenuMusic.Play();
        }
    }

    public void PlayGame()
    {
        if (ButtonSound != null)
        {
            ButtonSound.Play();
            Invoke("LoadGameScene", ButtonSound.clip.length);
        }
        else
        {
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadSceneAsync(gameSceneName);
    }

    public void QuitGame()
    {
        if (ButtonSound != null)
        {
            ButtonSound.Play();
            Invoke("QuitAfterSound", ButtonSound.clip.length);
        }
        else
        {
            QuitAfterSound();
        }
    }

    void QuitAfterSound()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
