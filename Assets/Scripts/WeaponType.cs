using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Weapon Type")]
public class WeaponType : ScriptableObject
{
    public string typeName;

    public bool isRanged;
    public DamageType damageType;

    public List<WeaponType> strongAgainstWeapon; // a bonus against these weapons will apply when YOU ATTACK THEM
    public List<WeaponType> weakAgainstWeapon; // a hinderance will be applied when YOU ATTACK these weapons
    public List<ClassTag> strongAgainstClass; // a bonus against these class types will apply when YOU ATTACK THEM
    public List<ClassTag> weakAgainstClass; // a hinderance will be applied when YOU ATTACK these classes

    /* 
    Bit of background on weapon type advantages:
    FE uses a simple triangle, swords beat axes, axes beat lances, lances beat swords, magic and bows isnt even in it.
    No matter whose attacking or defending, those weapons will be buffed and debuffed like 1.5x and 0.75x damage for that ENTIRE encounter respectively.

    I like the idea but in honestly i have never truly utilized this mechanic in a fire emblem game and ive played like 5 of them, so im revamping it

    I want it to work more like pokemon types (an actually good typed-attack system) and i want to encourage a genuine difference between attacking and defending turns,
    instead of just shoving your busted unit to the front of your army and watch it one tap everything on attack and defense.

    Instead, a weapon will have an advantage/disadvantage when its attacking INTO another weapon type (like in real life), for example, a bow will have an
    advantage attacking into a lance because how the hell is a skinny lance going to block an arrow unless the guy is cracked out of his mind, however, a lance WILL
    have an advantage attacking into a bow because a bow has no reliable way to block a stabbing motion. I additionally have included many more magic types into the system
    because im a mage at heart and wanna show some love to the good ol arcane.

    Decisions were made based off what i think would happen IRL, but will prob change for balance. And a bonus or hinderance is only applied if you would have an easier time
    rushing that weapon type, like a sword has no disernable advantage rushing a fire mage rather than a water mage, so no bonus is applied for either. Notably, im assuming
    a "hit and defend" system, where the defender wouldnt try to sucker punch the attacker, so thats why swords arent weak into axes cause an axe would just slam a sword 
    into the dirt everytime it got close to attack. The nature of attacking and defending DOES play a key role in combat. Can you tell im a martial artist yet lmao?

    A sucker punch would be a cool skill tho oh wait FE has vantage.

    Some decisions were premade for balance like Arcane

    TLDR: pokemon types because fun!

    Small Outline: (this is defined in the weapontype scriptable objects but im writing it here as a quick reference so i dont have to go into the editor if i need a reminder)
    Physical Damage:
        Sword: The all-rounder
            Strong into: Axe, Beast Claws, Dragon Breath
            Weak into: Lance, Wind, Earth, Dark, Ice
        Lance: The fast one, hits a lot but not great at blocking
            Strong into: Sword, Axe, Wind, Bow, Lance (yes itself)
            Weak into: Earth, Water, Dark, Ice
        Axe: The slow one, decent at blocking and packs a punch
            Strong into: Sword, Lance, Beast Claws, Dragon Breath, Wind, Bow, Ice
            Weak into: Earth, Dark
        Bow: The ranged nimble one, balancing for the fact that bow users will have high avoid and hit
            Strong into: Lance, Fire, Bow, Beast Claws, Dragon Breath
            Weak into: Dark, Wind, Axe, Light, Ice
        Beast Claws: The fast/strong one, but not great at blocking
            Strong into: Sword, Lance, Bow, Dragon Breath, Water
            Weak into: Fire, Wind, Earth, Dark
        Earth: Mage tanks.
            Strong into: Beast Claws, Ice
            Weak into: Axe, Water

    Magical Damage:
        Fire: Raw power, balancing that fire users will high arcane boosts
            Strong into: Beast Claws, Ice, Dark
            Weak into: Water, Wind, Earth
        Water: Area control, water mages might have something to do with terrain
            Strong into: Earth, Sword, Bow, Light, Fire
            Weak into: Wind, Ice
        Wind: Anti air area control
            Strong into: Lance, Sword
            Weak into: Fire, Axe, Earth
        Dark: Edge. Trad FE dark magic being off-tanks or high-skill crit-happy mage snipers
            Strong into: Light
            Weak into: Fire
        Light: Anti-Edge. Anti-area control, kind of like a support
            Strong into: Dark
            Weak into: Water, Earth
        Ice: Magic equivalent of axe
            Strong into: Water, Sword, Lance, Beast Claws, Dragon Breath
            Weak into: Fire, Earth
        Arcane: Anti-magic magic
            Strong into: Fire, Water, Wind, Earth, Dark, Light, Ice, Staff
            Weak into: Arcane, Sword, Lance, Axe, Bow, Beast Claws, Dragon Breath
        Dragon Breath: Inverse Anti-magic mage tanks (holy game design)
            Strong into: Arcane
            Weak into: nothing
        Staff: Support
            Strong into: nothing
            Weak into: nothing
            
    */
}

public enum DamageType { Physical, Magical }
