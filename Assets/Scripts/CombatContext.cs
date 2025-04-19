// trust the process
// this lets me allot trigger skill effects, and generally takes a lot of mess
// away from combat calculations, which ill do later
public class CombatContext
{
    public int attackPower;
    public int defensePower;
    public int baseDamage;
    public int finalDamage;

    public bool isPlayerAttack;
    public bool isCounterAttack;

    public bool attackerHasAdvantage;
    public bool triggeredSkill;

    // need to add the secondary stats, will do later just skeletoning this out for now
}
