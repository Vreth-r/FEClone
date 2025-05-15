using UnityEngine;

public static class CampInputBlocker
{
    public static bool Blocked { get; private set; }

    public static void SetBlocked(bool value)
    {
        Blocked = value;
    }
}