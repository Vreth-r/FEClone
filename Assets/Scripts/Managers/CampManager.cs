using UnityEngine;
using Yarn.Unity;

public class CampManager : MonoBehaviour
{
    public static CampManager Instance;

    public DialogueRunner dRunner;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
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