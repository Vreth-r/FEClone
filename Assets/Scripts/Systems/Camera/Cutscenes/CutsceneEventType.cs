// Cutscene event types
public enum CutsceneEventType
{
    PanToLocation,
    PanToUnit,
    CameraShake,
    UnitJump,
    UnitMoveToPos,
    UnitEmote,
    Wait
    // add more if needed.
    // also make sure to add relevant info to CutsceneEventDrawer.cs and CutsceneManager.cs
}