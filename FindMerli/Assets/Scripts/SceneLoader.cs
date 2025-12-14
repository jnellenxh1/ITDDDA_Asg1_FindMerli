using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Public function to be called by the Home Page 'Scan' button
    public void LoadSceneByName(string sceneName)
    {
        // Check if the scene name is valid before loading (optional but good)
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