using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public List<string> enemyTypes = new List<string>();
    public List<float[]> enemyPosition = new List<float[]>();
    public List<int> enemySettlementWait = new List<int>();
    public int[] enemyCampCooldowns = new int[3];

    public List<bool> enemyAttackingSettlement = new List<bool>();
    public List<bool> enemyAttackingVillage = new List<bool>();

    public int numOfEnemies;
    public bool IsAttackingSettlement;



    public EnemyData(EnemyManager enemyManager, MAINmanager mainManager, int[] enemyCooldowns)
    {
        foreach(GameObject enemy in enemyManager.GetCurrentEnemyList())
        {
            EnemyObjects enemyObjects = enemy.gameObject.GetComponent<EnemyObjects>();
            enemyTypes.Add(enemyObjects.GetEnemyType());
            float[] position = new float[3];

            position[0] = enemy.transform.position.x;
            position[1] = enemy.transform.position.y;
            position[2] = enemy.transform.position.z;

            enemyAttackingSettlement.Add(enemy.GetComponent<EnemyBuildingAttack>().IsAttackingSettlement);
            enemyAttackingVillage.Add(enemy.GetComponent<EnemyBuildingAttack>().IsAttackingVillage);
        
            enemyPosition.Add(position);
            enemySettlementWait.Add(enemy.GetComponent<EnemyMovement>().GetEnemyTurnCount());
        }

        numOfEnemies = enemyTypes.Count;

        for (int i = 0; i < 3; i++)
        {
            enemyCampCooldowns[i] = mainManager.GetEnemyCamps()[i].GetCooldown();
        }

        IsAttackingSettlement = enemyManager.GetIsAttackingSettlement();
        
    }


}
