using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tactics RPG/Weapon Type")]
public class WeaponType : ScriptableObject
{
    public string typeName;

    public bool isRanged;

    public List<WeaponType> strongAgainstWeapon; // a bonus against these weapons will apply when YOU ATTACK THEM
    public List<WeaponType> weakAgainstWeapon; // a hinderance will be applied when YOU ATTACK these weapons
    public List<ClassTag> strongAgainstClass; // a bonus against these class types will apply when YOU ATTACK THEM
    public List<ClassTag> weakAgainstClass; // a hinderance will be applied when YOU ATTACK these classes
}
