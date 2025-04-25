using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Effects/HealOnKill")]
public class HealOnKillEffect : Effect
{
    [Range(0, 1)] public float maxHPtoHealPercent = 0f;
    public int flatHealthToHeal = 0;

    public override void ApplyTrigger(Unit attacker, Unit defender, CombatContext context)
    {   
        Debug.Log("Trigger Skill Heal On Kill activated");
        // Change this to a heal method later so ui stuff can happen
        attacker.currentHP += Mathf.Min(attacker.maxHP - attacker.currentHP, Mathf.RoundToInt(attacker.maxHP*maxHPtoHealPercent) + flatHealthToHeal);
    }
}