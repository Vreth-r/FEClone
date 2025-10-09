using UnityEngine;

public class GoToLevelInteractable : CampInteractable
{

    public string sceneName;
    public override void Interact()
    {
        Debug.Log("Loading Level");
        LoadingScreenManager.Instance.LoadScene(sceneName);
    }
}