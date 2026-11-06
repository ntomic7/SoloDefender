using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellObjects : MonoBehaviour
{
    [SerializeField] SpellTypesSO spellTypeSO;
    [SerializeField] public Sprite backgroundSprite;
    [SerializeField] Sprite itemSprite;
    
    public BattleResourcesManager.ResourcePullType resourcePullType;
    public int currentSkillCooldown;
    public int currentBuffLength;
    public int buffLength;

    public bool IsPotion;
    public bool IsSkill;

    [SerializeField] int tier;
    [SerializeField] int price;
    [SerializeField] string description;

    private void Start()
    {
        currentBuffLength = buffLength;
        currentSkillCooldown = spellTypeSO.skillCooldown;
    }

    public int GetBuffAmount(Attributes attributes)
    {

        if (resourcePullType == BattleResourcesManager.ResourcePullType.Health) return (int)(attributes.maxHealth * (GetBuff().buffPercentage / 100));
        else if (resourcePullType == BattleResourcesManager.ResourcePullType.Mana) return (int)(attributes.maxMana * (GetBuff().buffPercentage / 100));
        else if (resourcePullType == BattleResourcesManager.ResourcePullType.Attack) return (int)(attributes.baseAttack * (GetBuff().buffPercentage / 100));
        else if (resourcePullType == BattleResourcesManager.ResourcePullType.Defence) return (int)(attributes.baseDefence * (GetBuff().buffPercentage / 100));
        return 0;
    }

    public Sprite GetBuffSprite()
    {
        return GetBuff().buffSprite;
    }
    public Sprite GetItemSprite()
    {
        return itemSprite;
    }
    public string GetName()
    {
        return spellTypeSO.spellName;
    }
    public string GetDescription()
    {
        return description;
    }

    public float GetPercentage()
    {
        return spellTypeSO.initialPercentage;
    }
    public int GetManaCost()
    {
        return spellTypeSO.manaCost;
    }
    public int GetPrice()
    {
        return price;
    }
    public  int GetTier()
    {
        return tier;
    }

    public bool GetIsBuff()
    {
        return spellTypeSO.IsBuff;
    }
    public bool GetIsSkill()
    {
        return IsSkill;
    }
    public bool GetIsPotion()
    {
        return IsPotion;
    }

    public int GetCooldown()
    {
        return spellTypeSO.skillCooldown;
    }
    public void DecreaseCooldown()
    {
        currentSkillCooldown--;
    }

    public SpellTypesSO.SpellType GetSpellType()
    {
        return spellTypeSO.spellType;
    }
    public BuffTypeSO GetBuff()
    {
        return spellTypeSO.BuffTypeSO;
    }
    public BattleResourcesManager.ResourcePullType GetResourcePullType()
    {
        return resourcePullType;
    }

    
}
