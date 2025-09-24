using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    public Vector3 SpawnPoint { get; private set; }

    const string KeyX = "cp_x";
    const string KeyY = "cp_y";
    const string KeyZ = "cp_z";
    const string KeyScene = "cp_scene";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPrefs.HasKey(KeyScene) && PlayerPrefs.GetString(KeyScene) == scene.name)
        {
            SpawnPoint = new Vector3(
                PlayerPrefs.GetFloat(KeyX),
                PlayerPrefs.GetFloat(KeyY),
                PlayerPrefs.GetFloat(KeyZ)
            );
        }
        else
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) SpawnPoint = player.transform.position;
        }
    }

    public void SetCheckpoint(Vector3 pos)
    {
        SpawnPoint = pos;
        PlayerPrefs.SetString(KeyScene, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(KeyX, pos.x);
        PlayerPrefs.SetFloat(KeyY, pos.y);
        PlayerPrefs.SetFloat(KeyZ, pos.z);
        PlayerPrefs.Save();
    }

    public void ClearSavedCheckpoint()
    {
        PlayerPrefs.DeleteKey(KeyScene);
        PlayerPrefs.DeleteKey(KeyX);
        PlayerPrefs.DeleteKey(KeyY);
        PlayerPrefs.DeleteKey(KeyZ);
        PlayerPrefs.Save();
    }
}
