using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : MonoBehaviour
{
    public void LoadMap(string level)
    {
        // store the party before switching scenes
        PlayerPersistor.Instance.StorePartyInContainer();
        

        // just load whatever scene name is passed in
        if (SceneExists(level))
        {
            SceneManager.LoadScene(level);
        }
        else
        {
            Debug.LogError($"level scene not found: {level}");
        }
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}