// trust the process
// this lets me allot trigger skill effects, and generally takes a lot of mess
// away from combat calculations, which ill do later
public class CombatContext
{
    public Unit attacker;
    public Unit defender;
    public WeaponItem weapon;

    public int attackPower;
    public int defensePower;
    public int baseDamage;
    public int finalDamage;

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

    // need to add the secondary stats, will do later just skeletoning this out for now
}
