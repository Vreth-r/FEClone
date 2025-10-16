using UnityEngine;


// this was rushed, need to reimplement
public class GoToLevelInteractable : CampInteractable
{

    public string sceneName;
    public override void Interact()
    {
        Debug.Log("Loading Level");
        LoadingScreenManager.Instance.LoadLevel(sceneName);
    }
}