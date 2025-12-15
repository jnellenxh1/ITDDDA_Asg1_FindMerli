using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    /// <summary>
    /// Loads a scene by its name
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) != -1)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings!");
        }
    }
}