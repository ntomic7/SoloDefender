using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level, attributePoints, numOfAttackButtons;
    public int currentXP, xpToNextLvl;
    public int weaponMin, weaponMax, armorMin, armorMax, attack, defence;
    public int currentHealth, currentMana, maxHealth, maxMana;
    public float[] position = new float[3];

    public string[] attackButtons = new string[6];


    public PlayerData(GameObject playerGO, List<GameObject> attackButtonsList)
    {
        PlayerLevelManager PLM = playerGO.GetComponent<PlayerLevelManager>();

        level = PLM.GetPlayerLevel();
        attributePoints = PLM.GetCurrentPoints();
        numOfAttackButtons = PLM.GetActiveAttackNum();

        currentXP = PLM.GetCurrentXP();
        xpToNextLvl = PLM.GetRequiredXP();

        Attributes playerAttributes = playerGO.GetComponent<Attributes>();

        weaponMin = playerAttributes.weaponAttack.x;
        weaponMax = playerAttributes.weaponAttack.y;
        armorMin  = playerAttributes.armourDefence.x;
        armorMax = playerAttributes.armourDefence.y;

        attack = playerAttributes.baseAttack;
        defence = playerAttributes.baseDefence;

        currentHealth = playerAttributes.currentHealth;
        currentMana = playerAttributes.currentMana;

        maxHealth = playerAttributes.maxHealth;
        maxMana = playerAttributes.maxMana;
        position[0] = playerGO.transform.position.x;
        position[1] = playerGO.transform.position.y;
        position[2] = playerGO.transform.position.z;

        AddAttackButtons(attackButtonsList);
    }

    // retrieval funcs

    void AddAttackButtons(List<GameObject> attackButtonsGO)
    {
         for(int i = 0; i < 6; i++)
        {
            BattleResourcesManager brm = attackButtonsGO[i].GetComponent<BattleResourcesManager>();
            if(brm.GetCurrentItem() != null) attackButtons[i] = brm.GetCurrentItem().GetName();
        }
    }
}
