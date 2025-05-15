using UnityEngine;

public class CampManager : MonoBehaviour
{
    public static CampManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void EnterCamp(Unit playerLeader)
    {
        // load scene and shit
        return;
    }

    public void ExitCamp()
    {
        // Save state and go to the map. And shit.
        return;
    }
}