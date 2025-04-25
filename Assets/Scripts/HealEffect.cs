using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Effects/Heal")]
public class HealEffect : Effect
{
    [Range(0, 1)] public float maxHPToHealPercent = 0f;
    public int flatHealthToHeal = 0;

    public override void ApplyTrigger(Unit attacker, Unit defender, CombatContext context)
    {   
        Debug.Log("Trigger Skill Heal On Kill activated");
        // Change this to a heal method later so ui stuff can happen
        attacker.currentHP += Mathf.Min(attacker.maxHP - attacker.currentHP, Mathf.RoundToInt(attacker.maxHP*maxHPToHealPercent) + flatHealthToHeal);
    }
}