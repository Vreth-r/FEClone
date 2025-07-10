using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscenes/Cutscene Description")]
public class CutsceneDescriptionData : ScriptableObject
{
    public List<CutsceneStep> steps;
}