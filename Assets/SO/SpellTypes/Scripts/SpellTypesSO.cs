using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellTypesSO", menuName = "ScriptableObjects/SpellTypesSO")] 
public class SpellTypesSO : ScriptableObject
{
    public string spellName;
    public SpellType spellType;

    //public Sprite spellSprite;
    public BuffTypeSO BuffTypeSO;
    
    public float initialPercentage;
    public int manaCost;
    public bool IsBuff;

    public int skillCooldown;

    public enum SpellType
    {
        Fire,
        Lightning,
        Poisoning,
        Bleeding,
        Healing,
        Shield,
        Translucent,
        AttackBoost,
        AttackDecrease,
        DefenceBoost,
        DefenceDecrease,
        None,
        EndBossMagicAttack,
    }


}
