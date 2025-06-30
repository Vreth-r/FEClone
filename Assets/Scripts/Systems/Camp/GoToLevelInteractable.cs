using UnityEngine;

public class GoToLevelInteractable : CampInteractable
{
    public override void Interact()
    {
        Debug.Log("PORTAL NOISE");
        LoadingScreenManager.Instance.LoadLevel("TerrainTestMap");
    }
}