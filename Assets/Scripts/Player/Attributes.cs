using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Attributes : MonoBehaviour
{

    [SerializeField] public int baseAttack = 5;
    [SerializeField] public int baseDefence = 5;
    [SerializeField] public Vector2Int weaponAttack = new Vector2Int(1, 4);
    [SerializeField] public Vector2Int armourDefence = new Vector2Int(2, 7);

    [SerializeField] public int maxHealth = 20;
    [SerializeField] public int currentHealth;

    [SerializeField] public int maxMana = 10;
    [SerializeField] public int currentMana;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void SetMaxHealth(int amount)
    {
        maxHealth += amount;
    }
    public void ReloadMaxHealth(int amount)
    {
        maxHealth = amount;
    }
    public void SetMaxMana(int amount)
    {
        maxMana += amount;
        currentMana = maxMana;
    }
    public void ReloadMaxMana(int amount)
    {
        maxMana  = amount;
    }
    public int GetHealth()
    {
        return currentHealth;
    }
    public int GetMana()
    {
        return currentMana;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetMaxMana()
    {
        return maxMana;
    }

    public void SetValue(int amount, string type)
    {
        if (type == "health") currentHealth = ((currentHealth + amount) > maxHealth) ? maxHealth : currentHealth + amount;

        else if (type == "mana") currentMana = ((currentMana + amount) > maxMana) ? maxMana : currentMana + amount;
    }

    public Vector2Int GetAttributes(string attribute)
    {
        if (attribute == "weapon")
            return weaponAttack;
        else if (attribute == "armor")
            return armourDefence;
        else return default;
    }

    public void UpgradeEquipment(bool IsSword , Vector2Int upgrade)
    {
        if (IsSword) weaponAttack = upgrade;
        else armourDefence = upgrade;
    }

    public void OnPlayerDeath()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

}
