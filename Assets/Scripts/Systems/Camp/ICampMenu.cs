using UnityEngine;

public interface ICampMenu
{
    void Open();
    void Close();
}

public enum CampMenuType
{
    Shop,
    Army,
    Support,
    Options
}