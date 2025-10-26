using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{
    // Serializable UnityEvent that carries the name of the loaded level
    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    // Editable, reorderable list of scene names. Change order in the inspector.
    [Header("Ordered list of level scene names (must be added to Build Settings)")]
    [SerializeField] private List<string> levels = new List<string>();

    [Header("Behavior")]
    [Tooltip("If true, after the last entry in 'levels' it will loop back to the first.")]
    [SerializeField] private bool loopLevels = false;

    [Tooltip("Optional scene to load after the last level (overrides loopLevels when non-empty).")]
    [SerializeField] private string finalSceneName = "";

    [Header("Events")]
    [Tooltip("Invoked after a level is loaded. Parameter is the loaded scene name.")]
    [SerializeField] private StringEvent onLevelLoaded;

    // Singleton instance (persistent)
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // Simple singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Optional sanity checks
        if (levels.Count == 0)
        {
            Debug.LogWarning("LevelManager: 'levels' list is empty. The manager will fall back to build order.");
        }
    }

    // Public API used by level-complete logic
    public void LevelComplete()
    {
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        int currentIdx = GetCurrentLevelIndex();

        // If levels list provided and current scene is inside it:
        if (levels != null && levels.Count > 0 && currentIdx >= 0)
        {
            int nextIdx = currentIdx + 1;

            // Reached end of custom list
            if (nextIdx >= levels.Count)
            {
                if (!string.IsNullOrEmpty(finalSceneName))
                {
                    LoadSceneByName(finalSceneName);
                    return;
                }

                if (loopLevels)
                {
                    nextIdx = 0;
                }
                else
                {
                    Debug.Log("LevelManager: Reached last level in list and loopLevels is false. No further level to load.");
                    return;
                }
            }

            LoadSceneByName(levels[nextIdx]);
            return;
        }

        // Fallback: use Build Settings scene order
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = currentBuildIndex + 1;
        if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextBuildIndex);
            onLevelLoaded?.Invoke(SceneManager.GetSceneByBuildIndex(nextBuildIndex).name);
        }
        else if (!string.IsNullOrEmpty(finalSceneName))
        {
            LoadSceneByName(finalSceneName);
        }
        else if (loopLevels && SceneManager.sceneCountInBuildSettings > 0)
        {
            SceneManager.LoadScene(0);
            onLevelLoaded?.Invoke(SceneManager.GetSceneByBuildIndex(0).name);
        }
        else
        {
            Debug.Log("LevelManager: Reached last build scene and no further action configured.");
        }
    }

    public void LoadLevel(int index)
    {
        if (levels != null && index >= 0 && index < levels.Count)
        {
            LoadSceneByName(levels[index]);
            return;
        }

        Debug.LogWarning($"LevelManager: LoadLevel({index}) - index out of range for 'levels' list.");
    }

    public void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("LevelManager: LoadLevel(sceneName) - sceneName is null or empty.");
            return;
        }
        LoadSceneByName(sceneName);
    }

    public void RestartLevel()
    {
        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.name);
        onLevelLoaded?.Invoke(active.name);
    }

    private void LoadSceneByName(string sceneName)
    {
        
        Debug.Log($"LevelManager: Loading scene '{sceneName}'");
        SceneManager.LoadScene(sceneName);
        onLevelLoaded?.Invoke(sceneName);
    }

    // Returns index inside the 'levels' list, or -1 when not found / fallback should be used
    private int GetCurrentLevelIndex()
    {
        if (levels == null || levels.Count == 0) return -1;
        string current = SceneManager.GetActiveScene().name;
        return levels.IndexOf(current);
    }
}
