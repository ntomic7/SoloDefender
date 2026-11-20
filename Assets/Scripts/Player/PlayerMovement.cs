using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    [SerializeField] TurnManager turnManager;
    [SerializeField] ResourceBuilder resourceBuilder;
    BattleManager battleManager;
    FortressAttack fortressAttack;
    GridHexXZ<GridBuildingSystem.GridObject> grid;

    public Action<Vector2Int> OnPlayerMovement;
    Animator animator;

    Vector2Int currentGridPosition;
    Vector3 nextTilePosition;
    Vector2Int nextTile;

    [SerializeField] GameObject nextTileVisual;
    bool canUseMovement = true;

    private IEnumerator Start()
    {
        while (turnManager == null || turnManager.GetGrid() == null)
        {
            yield return null;
        }
        grid = turnManager.GetGrid();

        if (!TryUpdateGridPosition())
        {
            yield break;
        }
    }
    bool TryUpdateGridPosition()
    {
        if (grid == null) return false;

        try
        {
            grid.GetXY(transform.position, out int newX, out int newY);
            currentGridPosition = new Vector2Int(newX, newY);
            animator = gameObject.GetComponentInChildren<Animator>();

            battleManager = mainManager.GetBattleManager();
            fortressAttack = battleManager.GetComponent<FortressAttack>();
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    void Update()
    {
        if (canUseMovement && grid != null && grid.GetGridObject(UtilsClass.GetMouseWorldPosition()) != null)
        {
            nextTileVisual.transform.position = grid.GetGridObject(UtilsClass.GetMouseWorldPosition()).GetTileObject().gameObject.transform.position;
        }

        if (grid != null && Input.GetMouseButtonDown(0) && grid.GetGridObject(UtilsClass.GetMouseWorldPosition()) != default && canUseMovement
                && !EventSystem.current.IsPointerOverGameObject() )
        {
            int tileX, tileY;
            grid.GetXY(UtilsClass.GetMouseWorldPosition(), out tileX, out tileY);

            List<Vector2Int> surroundingTiles = grid.GetSurroundingGridObjects(transform.position);
            Vector2Int chosenTilePosition = new Vector2Int(tileX, tileY);
            TileObjects chosenTile = grid.GetGridObject(tileX, tileY).GetTileObject();

            if(CheckIfCanMove(surroundingTiles, chosenTilePosition, chosenTile))
            {
                nextTile = new Vector2Int(tileX, tileY);
                nextTilePosition = grid.GetWorldPosition(tileX, tileY);

                OnBeginNextTurn();
            }
        }
    }

    void OnBeginNextTurn()
    {
        if(nextTilePosition != Vector3.zero)
        {
            if (nextTilePosition.x < animator.transform.position.x)
            {
                animator.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                animator.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            transform.position = nextTilePosition;
            currentGridPosition = new Vector2Int(nextTile.x, nextTile.y);   
            nextTilePosition = Vector3.zero;

            OnPlayerMovement?.Invoke(currentGridPosition);
            turnManager.BeginNextTurn();

            if(grid.GetGridObject(currentGridPosition.x, currentGridPosition.y).GetTileObject().GetResourceType() == TileObjects.TileType.EnemyCamp)
            {
                   fortressAttack.StartFortressAttack(grid.GetGridObject(currentGridPosition.x, currentGridPosition.y).GetTileObject()
                    , grid.GetGridObject(currentGridPosition.x, currentGridPosition.y).GetTileObject().GetCampEnemies());
            }
        }
    }
    bool CheckIfCanMove(List<Vector2Int> surroundingTiles, Vector2Int chosenTilePosition, TileObjects chosenTile)
    {
        if (battleManager.GetIsBattling()) return false;

        if (surroundingTiles.Contains(chosenTilePosition)) return true;
      
        return false;
    }

    public Vector2Int GetCurrentPlayerPosition()
    {
        return currentGridPosition;
    }

    public void SetCanUseMovement(bool option)
    {
        canUseMovement = option;    
    }

    private void OnDestroy()
    {
        turnManager.OnEndOfTurn -= OnBeginNextTurn;

    }
}
