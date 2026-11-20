using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleResourcesManager : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    [SerializeField] BattleVisuals battleVisuals;

    [SerializeField] PlayerManaSystem playerManaSystem;
    Inventory playerInventory;

    [SerializeField] GameObject CurrentResource;
    [SerializeField] Image SpellIconSprite;
    Image BackgroundImage;

    [SerializeField] SpellObjects currentItem;

    bool changingItem;

    private void Start()
    {
       if(CurrentResource != null) SetUpResource();
        BackgroundImage = GetComponent<Image>();

        playerInventory = playerManaSystem.gameObject.GetComponent<Inventory>();
        playerInventory.OnItemChosen += OnChangeAttackButton;

       this.gameObject.SetActive(false);

    }

    public void OnChangeAttackButton(SpellObjects spellObject, GameObject chosenButton)
    {
        if (chosenButton == this.gameObject)
        {
            changingItem = true;

            if (!battleManager.GetIsBattling())
            {
                SpellIconSprite.gameObject.SetActive(true);
                SpellIconSprite.sprite = spellObject.GetComponentInChildren<SpriteRenderer>().sprite;
                CurrentResource = spellObject.gameObject;

                BackgroundImage.sprite = spellObject.GetComponent<SpellObjects>().backgroundSprite;

                SetUpResource();          
            }

            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(.1f);
        changingItem = false;
    }
    public SpellObjects GetResource()
    {
        if (CurrentResource == null) return null;
       return CurrentResource.GetComponent<SpellObjects>();
    }
    void SetUpResource()
    {
       currentItem = CurrentResource.GetComponent<SpellObjects>();
    }

    public void UseAttack()
    {
        if (currentItem == null)
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Error);
            return;
        }
        if (!battleManager.GetIsBattling() && currentItem.GetIsPotion() && playerInventory.HasPotion(currentItem)
            && !changingItem)
        {
            playerManaSystem.UsePotion(currentItem);
            return;
        }

        if (battleManager.canUseResource && currentItem.GetIsSkill() && !battleManager.playerCurrentSkills.Contains(currentItem))
        {
            ChooseSkill();
        }
        else if (battleManager.canUseResource && currentItem.GetIsPotion() && playerInventory.HasPotion(currentItem))
        {
            ChoosePotion();
        }
        else if (battleManager.canUseResource && (playerManaSystem.GetCurrentMana() > currentItem.GetManaCost())
            && !currentItem.GetIsPotion() && !currentItem.GetIsSkill())
        {
            ChooseSpell();
        }
        else if(playerManaSystem.GetCurrentMana() < currentItem.GetManaCost() || !playerInventory.HasPotion(currentItem)
            || battleManager.playerCurrentSkills.Contains(currentItem))
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Error);
        }
        
    }

    public void ChooseSpell()
    {
        SoundSystem.instance.PlaySound(SoundSystem.Sound.Spell);


        if (currentItem.GetSpellType() == SpellTypesSO.SpellType.EndBossMagicAttack && battleManager.GetIsBossBattle()
            && battleManager.GetIsCharging())
        {
            Debug.Log("endboss!!");
            battleManager.StopBossAttack();
            SetUpValues();
            playerManaSystem.UseMana(currentItem);
        }
        else if (currentItem.GetSpellType() != SpellTypesSO.SpellType.EndBossMagicAttack)
        {
            SetUpValues();
            playerManaSystem.UseMana(currentItem);

            if (currentItem.GetIsBuff() && !battleManager.PlayerCurrentBuffs.Contains(currentItem)
                    && (currentItem.GetSpellType() == SpellTypesSO.SpellType.Healing || currentItem.GetBuff().buffType == BuffTypeSO.BuffTypes.Boost))
            {
                battleManager.PlayerCurrentBuffs.Add(currentItem);
                battleVisuals.AddBuffs(battleManager.PlayerCurrentBuffs, battleVisuals.buffIconListPlayer);
            }
            else if (currentItem.GetIsBuff() &&
                !(currentItem.GetSpellType() == SpellTypesSO.SpellType.Healing || currentItem.GetBuff().buffType == BuffTypeSO.BuffTypes.Boost))
            {
                if (!battleManager.EnemyCurrentBuffs.Contains(currentItem))
                {
                    battleManager.EnemyCurrentBuffs.Add(currentItem);
                    battleVisuals.AddBuffs(battleManager.EnemyCurrentBuffs, battleVisuals.buffIconListEnemy);
                }
                battleManager.DamageEnemy(currentItem.resourcePullType, currentItem.GetPercentage());
            }
            else
            {
                battleManager.DamageEnemy(currentItem.resourcePullType, currentItem.GetPercentage());
            }
        }
    }
    public void ChooseSkill()
    {
        SetUpSkillValues();
        if (currentItem.GetSpellType() == SpellTypesSO.SpellType.EndBossMagicAttack && battleManager.GetIsBossBattle()
                && battleManager.GetIsCharging())
        {
            battleManager.StopBossAttack();
            battleVisuals.AddBuffs(battleManager.PlayerCurrentBuffs, battleVisuals.buffIconListPlayer);
            battleManager.playerCurrentSkills.Add(currentItem);
            battleManager.PlayerCurrentBuffs.Add(currentItem);
            SetUpSkillValues();
        }
        else if (currentItem.GetSpellType() != SpellTypesSO.SpellType.EndBossMagicAttack)
        {
            if (!currentItem.GetIsBuff())
            {
                battleManager.DamageEnemy(currentItem.GetResourcePullType(), currentItem.GetPercentage());
                battleManager.playerCurrentSkills.Add(currentItem);
            }
            else if (currentItem.GetBuff().GetBuffType() == BuffTypeSO.BuffTypes.Boost)
            {
                Debug.Log("boost");

                battleVisuals.AddBuffs(battleManager.PlayerCurrentBuffs, battleVisuals.buffIconListPlayer);

                battleManager.playerCurrentSkills.Add(currentItem);
                battleManager.PlayerCurrentBuffs.Add(currentItem);
            }
            else
            {
                Debug.Log("enemy debuff");

                battleManager.DamageEnemy(currentItem.GetResourcePullType(), currentItem.GetPercentage());
                battleManager.playerCurrentSkills.Add(currentItem);
                battleManager.EnemyCurrentBuffs.Add(currentItem);

                battleVisuals.AddBuffs(battleManager.EnemyCurrentBuffs, battleVisuals.buffIconListEnemy);
            }
        }
    }
    public void ChoosePotion()
    {
        SetUpValues();

        if (currentItem.GetIsBuff() && currentItem.GetSpellType() != SpellTypesSO.SpellType.Healing &&
            !battleManager.PlayerCurrentBuffs.Contains(currentItem))
        {

            battleManager.PlayerCurrentBuffs.Add(currentItem);
            battleVisuals.AddBuffs(battleManager.PlayerCurrentBuffs, battleVisuals.buffIconListPlayer);


        }
        playerManaSystem.UsePotion(currentItem);

    }

    void SetUpValues()
    {
        currentItem.currentBuffLength = currentItem.buffLength;
        battleManager.canUseResource = false;
    }
    void SetUpSkillValues()
    {
        SetUpValues();
        currentItem.currentSkillCooldown = currentItem.GetCooldown();
        battleVisuals.SkillCooldownVisuals(currentItem, IsDeleting: false);
    }

    public SpellObjects GetCurrentItem()
    {
        return currentItem;
    }

    public enum ResourcePullType
    {
        Health,
        Mana,
        Attack,
        Defence
    }
}

