using UnityEngine;

public class GoToSceneInteractable : CampInteractable
{

    public string sceneName;
    public override void Interact()
    {
        Debug.Log("Loading scene");
        LoadingScreenManager.Instance.LoadScene(sceneName, () => 
        {
            GameManager.Instance.MasterYarnRunner.StartDialogue(sceneName);
        });
    }
}