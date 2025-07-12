using System.Collections.Generic;
using UnityEngine;

// Sciptable object to hold what happens in a cutscene
// Pretty much just holds an array of arrays of events that can happen in a cutscene
[CreateAssetMenu(menuName = "Cutscenes/Cutscene Description")]
public class CutsceneDescriptionData : ScriptableObject
{
    public List<CutsceneStep> steps;
}