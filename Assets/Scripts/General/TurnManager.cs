using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GridBuildingSystem;

public class TurnManager : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    [SerializeField] GridBuildingSystem gridBuildingSystem;
 
    GridHexXZ<GridObject> grid;
    public Action OnEndOfTurn;

    public int turnCount = 0;

    int campsDestroyed;

    void Start()
    {    
        grid = gridBuildingSystem.GetGrid();
    }
    public GridHexXZ<GridObject> GetGrid()
    {
        return grid;
    }

    [EditorCools.Button]
    public void BeginNextTurn()
    {
        turnCount++;
        OnEndOfTurn?.Invoke();
    }
    public int GetTurn()
    {
        return turnCount;
    }

    public void OnEnemyCampDestruction()
    {
        campsDestroyed++;

        if (campsDestroyed == 3) mainManager.GameOver();
    }
}
