using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class CampManager : MonoBehaviour
{
    public static CampManager Instance;

    public DialogueRunner dRunner; // temp i think, might make this ref a dialogue manager exlusive once i figure out yarn

    private void Awake()
    {
        if (Instance == null) Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dRunner.StartDialogue("Start");
    } 

    public void EnterCamp(Unit playerLeader)
    {
        // this is here for testing
        return;
    }

    public void ExitCamp()
    {
        // Save state and go to the map. And shit.
        return;
    }
}