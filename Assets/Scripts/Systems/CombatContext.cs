// trust the process
// this lets me allot skill effects, and generally takes a lot of mess
// away from combat calculations, which ill do later
public class CombatContext
{
    public Unit attacker; // the unit attacking
    public Unit defender; // the unit defending
    public WeaponItem attackerWeapon; // the weapon the attacker is using
    public WeaponItem defenderWeapon;
    public int attackerPrevHP;
    public int defenderPrevHP;

    public int attackPower; // attack power of the attacker
    public int defensePower; // defense power of the defender
    public float critPower = 1.5f; // the mod for damage for criticals
    public int baseDamage; // the base damage the attacker would deal
    public int bonusDamage = 0; // bonus damage from skills and whatnot
    public int finalDamage; // the final damage after skill effects, defender bonuses, etc

    public int hitRate;
    public int avoid;
    public int hitChance;
    public bool hitting;

    public int critRate;
    public int critAvoid;
    public int critChance;
    public bool critting;

    public bool isPlayerAttack;
    public bool isCounterAttack;

    public bool attackerHasAdvantage;
    public bool triggeredSkill;

    public bool defenderDied;
    public bool attackerDied;

    public TerrainTile attackerTerrain; // will fully implement later
    public TerrainTile defenderTerrain;
}
