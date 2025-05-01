using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Weapon")]
public class WeaponItem : Item // holy coding btw
{   
    [Header("Weapon Stats")]
    public WeaponType weaponType;
    public int hit; // the extra hit a unit gets for using this item
    public int avoid; // the extra avoidance a unit gets for using this item
    public int crit; // the extra crit a unit gets for using this item
    public int durability = 50; // debating on whether to have this or do what fates did
    public int bonusStrength;
    public int bonusArcane;
    public int bonusDefense;
    public int bonusSpeed;
    public int bonusSkill;
    public int bonusResistance;
    public int bonusLuck;
    public WeaponProficiency proficiency;
    public DamageType damageType;

    private void OnEnable()
    {
        itemType = ItemType.Weapon;
    }

    public override void Use(Unit user, Unit target)
    {
        Debug.Log($"{user.name} attacks {target.name} with {itemName}!");
        // placeholder for now, combat logic maybe later 
    }

    public bool IsEffectiveAgainstWeapon(WeaponType targetType)
    {
        return weaponType.strongAgainstWeapon.Contains(targetType);
    }

    public bool IsWeakToWeapon(WeaponType targetType)
    {
        return weaponType.weakAgainstWeapon.Contains(targetType);
    }

    public bool IsEffectiveAgainstClass(ClassTag targetClass)
    {
        return weaponType.strongAgainstClass.Contains(targetClass);
    }

    public bool IsWeakToClass(ClassTag targetClass)
    {
        return weaponType.weakAgainstClass.Contains(targetClass);
    }
}

public enum DamageType { Physical, Magical }