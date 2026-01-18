using System;
using UnityEngine;
using System.Collections.Generic;

public static class UIEvents
{
    //public static Action<UIMenu> RequestPushMenu;
    public static Action RequestPopMenu;
    public static Action RequestClearAll;

    // menu specifics
    public static Action<Unit> RequestShowStats;
    public static Action RequestHideStats;

    public static Action<Unit, Vector3, List<UnitAction>> RequestShowActionMenu;
    public static Action RequestHideActionMenu;
}