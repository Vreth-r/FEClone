public class StatSet
{
    // meant to keep track of stats and modifications to stats, works for weapons, skills, and units

    public int movementRange; // how far the unit can mave based off its class
    public int attackRange = 0; // default to 0, changes with equipped weapon
    public int currentHP; // working hp
    public int maxHP; // max hp memory
    public int strength; // affects how hard unit hits for physical attacks
    public int arcane; // affects how hard unit hits for magic attacks
    public int defense; // affects how much physical damage a unit can shrug off
    public int speed; // affects how fast the unit (hitting chance and avoidance) is and if they can hit twice (5 more speed than op)
    public int skill; // affects crit chance and a lot of skill triggers
    public int resistance; // affects how much magical damage a unit can shrug off
    public int luck; // affects many things, mainly skill triggers and avoidances
    // will be adding MANY more stats theres a lot behind the scenes but these stats are what everyone can see up front

    // Secondary Stats
    public double avoidance; // avoidance affects how likely the unit is to avoid an attack (attacker hit - def avoid = %chance to hit)
    public double crit; // base % chance to deal a critical hit (damage increase depends on class)
    public double hit; // hit affects how likely the unit is to actually hit the unit they are attacking (see avoidance)
}

public enum StatType
{
    STR,
    ARC,
    SPD,
    DEF,
    SKL,
    RES,
    LCK,
    CHP,
    MHP,
    AVO,
    CRI,
    HIT
}
