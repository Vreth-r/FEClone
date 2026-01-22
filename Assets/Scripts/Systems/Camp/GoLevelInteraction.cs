using UnityEngine;

[CreateAssetMenu(fileName = "NewGoLevelInteraction", menuName = "Interactions/Go Level")]
public class GoLevelInteractionAction : InteractionAction
{
    public string levelID;

    public override void PerformAction()
    {
        base.PerformAction();
        LoadingScreenManager.Instance.LoadLevel(levelID);
    }
}