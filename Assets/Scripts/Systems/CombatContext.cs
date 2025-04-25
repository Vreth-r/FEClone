// trust the process
// this lets me allot skill effects, and generally takes a lot of mess
// away from combat calculations, which ill do later
public class CombatContext
{
    public Unit attacker; // the unit attacking
    public Unit defender; // the unit defending
    public WeaponItem weapon; // the weapon the attacker is using

    public int attackPower; // attack power of the attacker
    public int defensePower; // defense power of the defender
    public int baseDamage; // the base damage the attacker would deal
    public int finalDamage; // the final damage after skill effects, defender bonuses, etc

    public float hitRate;
    public float avoid;
    public float hitChance;

    public float critRate;
    public float critAvoid;
    public float critChance;

    public bool isPlayerAttack;
    public bool isCounterAttack;

    public bool attackerHasAdvantage;
    public bool triggeredSkill;

    public bool defenderDied;
    public bool attackerDied;
}
