using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjects : MonoBehaviour
{
    [SerializeField] int xpOnDeath;
    [SerializeField] string enemyType;

    public int GetXPAmount()
    {
        return xpOnDeath ;
    }
    public string GetEnemyType()
    {
        return enemyType;
    }


}
