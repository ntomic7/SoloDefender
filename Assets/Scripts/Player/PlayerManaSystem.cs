using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaSystem : MonoBehaviour
{
    Attributes playerAttributes;
    [SerializeField] BattleManager battleManager;
    [SerializeField] BattleVisuals battleVisuals;
    Inventory inventory;

    [SerializeField] TurnManager turnManager;
    [SerializeField] StatusBar manaStatusBar;
    [SerializeField] StatusBar healthStatusBar;

    [SerializeField] SpellTypesSO changeSpellType;

    string health = "health", mana = "mana";

    void Start()
    {
        playerAttributes = GetComponent<Attributes>();
        inventory = GetComponent<Inventory>();
        turnManager.OnEndOfTurn += OnBeginNextTurn;
    }

    public int GetCurrentMana()
    {
        return playerAttributes.currentMana;
    }


    public void UseMana(SpellObjects spellObject)
    {


        if (playerAttributes.GetMana() >= spellObject.GetManaCost())
        {
            playerAttributes.SetValue(-spellObject.GetManaCost(), "mana");
            manaStatusBar.SetValue(-spellObject.GetManaCost());
        }
    }

    public void UsePotion(SpellObjects potionObject)
    {
        bool hasRemeainingPotions = true;

        if (!potionObject.GetIsBuff() && playerAttributes.GetHealth() < playerAttributes.maxHealth
            && potionObject.GetResourcePullType() == BattleResourcesManager.ResourcePullType.Health)
        {
            int value = (int)(playerAttributes.maxHealth * (potionObject.GetPercentage() / 100));

            playerAttributes.SetValue(value, health);
            healthStatusBar.SetValue(value);

            hasRemeainingPotions = inventory.RemoveItem(potionObject);
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Potion);

        }
        else if (!potionObject.GetIsBuff() && playerAttributes.GetMana() < playerAttributes.maxMana
            && potionObject.GetResourcePullType() == BattleResourcesManager.ResourcePullType.Mana)
        {
            int value = (int)(playerAttributes.maxMana * (potionObject.GetPercentage()) / 100);

            playerAttributes.SetValue(value, mana);
            manaStatusBar.SetValue(value);

            hasRemeainingPotions = inventory.RemoveItem(potionObject);
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Potion);
        }
        else
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Error);
        }
        if (!hasRemeainingPotions && battleManager.GetIsBattling())
        {
            battleVisuals.SkillCooldownVisuals(potionObject, IsDeleting: false);
        }
        
            inventory.ActivateInventoryItemsUI();
       
    }

    void OnBeginNextTurn()
    {
        if(playerAttributes.currentMana < playerAttributes.maxMana)
        {
            playerAttributes.SetValue(1, "mana");
            playerAttributes.SetValue(1, "health");
            manaStatusBar.SetValue(1);
            healthStatusBar.SetValue(1);
        }
    }

}
