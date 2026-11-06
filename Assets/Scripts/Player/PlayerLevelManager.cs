using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    [SerializeField] ResourceManagerVisuals rmv;

    [SerializeField] Attributes playerAttributes;
    [SerializeField] StatusBar xpBar;

    [SerializeField] StatusBar healthStatusBar;
    [SerializeField] StatusBar manaStatusBar;

    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI manaText;
    [SerializeField] TextMeshProUGUI attackText;
    [SerializeField] TextMeshProUGUI defenceText;

    [SerializeField] TextMeshProUGUI currentPointsText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI xpText;

    [SerializeField] List<GameObject> attackButtons;

    [SerializeField] List<GameObject> levelUpWarnings;

    int Level = 1;   
    public int currentPoints = 0;
    public bool hasPoints = false;
    public int numOfActiveAttacks = 3;

    public int currentXP = 0;
    public int xpToLevelUp = 100;

    public Action OnLevelUp;

    void Start()
    {
        RefreshText();
        xpBar.SetMaxValue(xpToLevelUp);
        xpBar.SetCurrentValue(currentXP);
        rmv.SetNumOfAttacks(numOfActiveAttacks);
    }

    public void SetCurrentXP(int xpAmount)
    {
        currentXP += xpAmount;
        

        while (currentXP > xpToLevelUp)
        {

            if (currentXP > xpToLevelUp)
            {
                currentXP = currentXP - xpToLevelUp;
                OnLevelUpFunction();
            }
        }
        xpBar.SetCurrentValue(currentXP);
        RefreshText();
    }

    public int GetPlayerLevel()
    {
        return Level;
    }
    public int GetActiveAttackNum()
    {
        return numOfActiveAttacks;
    }

    public void OnLevelUpFunction()
    {
        CalculatePoints();
        if(Level != 15) LevelUpFX();        

        if (Level >= 15)
        {
            numOfActiveAttacks = 6;
        }
        else if (Level >= 10)
        {
            numOfActiveAttacks = 5;
        }
        else if (Level >= 5)
        {
            numOfActiveAttacks = 4;
        }
        else
        {
            numOfActiveAttacks = 3;
        }

        if (Level == 15)
        {
            rmv.ShowErrorText("You have reached Level 15! The enemies have gotten stronger", 0, "", "");
        }

        RefreshText();
        OnLevelUp?.Invoke();

        rmv.SetNumOfAttacks(numOfActiveAttacks);
    }
    void LevelUpFX()
    {
        SoundSystem.instance.PlaySound(SoundSystem.Sound.LevelUp);
        levelUpWarnings[0].SetActive(true);
        hasPoints = true;
        rmv.ShowErrorText("LEVEL UP", 0, "", "");

    }
    void CalculatePoints()
    {
        Level++;
        xpToLevelUp += (int)(xpToLevelUp * 0.35f);
        xpBar.SetMaxValue(xpToLevelUp);
        xpBar.SetCurrentValue(currentXP);
        currentPoints = (Level >= 10) ? currentPoints + 3 : currentPoints + 2;

    }

    public void IncreaseAttribute(GameObject button)
    {
        if (currentPoints > 0)
        {
            switch (button.name)
            {
                case "HealthButton":
                    {
                        playerAttributes.maxHealth += 5;
                        playerAttributes.currentHealth += 5;
                        healthStatusBar.SetMaxValue(playerAttributes.maxHealth);
                        healthStatusBar.SetCurrentValue(playerAttributes.currentHealth);
                        break;
                    }
                case "ManaButton":
                    {
                        playerAttributes.maxMana += 3;
                        playerAttributes.currentMana += 3;
                        manaStatusBar.SetMaxValue(playerAttributes.maxMana);
                        manaStatusBar.SetCurrentValue(playerAttributes.currentMana);
                        
                        break;
                    }
                case "AttackButton":
                    playerAttributes.baseAttack += 1;
                    break;
                case "DefenceButton":
                    playerAttributes.baseDefence += (int)(playerAttributes.baseDefence * .1f);
                    break;
                default:
                    break;
            }
            currentPoints--;
            if (currentPoints <= 0)
            {
                foreach (GameObject g in levelUpWarnings) g.SetActive(false);
                hasPoints = false;
            }
            RefreshText();
        }
    }
    public void RefreshText()
    {
        healthText.text = playerAttributes.maxHealth.ToString();
        manaText.text = playerAttributes.maxMana.ToString();
        attackText.text = playerAttributes.baseAttack.ToString();
        defenceText.text = playerAttributes.baseDefence.ToString();

        currentPointsText.text = (currentPoints > 0) ? "Available points: " + currentPoints.ToString() : "";
        xpText.text = "XP: " + currentXP + " / " + xpToLevelUp;
        levelText.text = "LEVEL " + Level;
    }

    public int GetCurrentPoints()
    {
        return currentPoints;
    }
    public int GetCurrentXP()
    {
        return currentXP;
    }
    public void SetXPafterLoad(int amount)
    {
        currentXP = amount;
    }
    public int GetRequiredXP()
    {
        return xpToLevelUp;
    }
    public void SetRequiredXP(int amount)
    {
        xpToLevelUp = amount;
    }
    public void SetLevel(int amount)
    {
        Level = amount;
    }
    
}
