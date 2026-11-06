using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GridData
{
    public List<string> chunks;
    public List<int[]> villageLocations;
    public List<int[]> enemyCampLocations;
    public List<int[]> woodLocations;
    public List<int[]> goldLocations;
    public List<int[]> foodLocations;

    public List<string> tiles;
    public List<int[]> tileCoords;
    public List<int> tileTiers; 
    public List<int> workers;

    public List<bool> villageConnected;
    public List<bool> hasRoad;
    public List<bool> isFOW;

    public GridData(GridBuildingSystem gridBuildingSystem, GridHexXZ<GridBuildingSystem.GridObject> grid)
    {
            chunks = new List<string>();
        villageLocations = new List<int[]>();
        enemyCampLocations = new List<int[]>();
        woodLocations = new List<int[]>();
        goldLocations = new List<int[]>();
        foodLocations = new List<int[]>();

        tiles = new List<string>();
        tileCoords = new List<int[]>();
        tileTiers = new List<int>();
        workers = new List<int>();

        villageConnected = new List<bool>();
        hasRoad = new List<bool>();
        isFOW = new List<bool>();

        List<TileTypesSO> tileTypes = gridBuildingSystem.GetTileTypes();

        foreach (GameObject chunk in gridBuildingSystem.GetFullChunkList())
        {
            int index = 0;

            chunks.Add(chunk.name.Replace("(Clone)", ""));
        }


        for (int i = 0; i < gridBuildingSystem.fullWidth; i++)
        {
            for (int j = 0; j < gridBuildingSystem.fullHeight; j++)
            {
                TileObjects tile = grid.GetGridObject(i, j).GetTileObject();

                if (tileTypes.Contains(grid.GetGridObject(i, j).GetTileObject().GetTileTypeSO()))
                {
                
                    tiles.Add(tile.GetName());
                    tileCoords.Add(new int[] { i, j });
                    tileTiers.Add(tile.GetTier());
                    workers.Add(tile.GetCurrentWorkers());
                    if (tile.GetResourceType() == TileObjects.TileType.Village) villageConnected.Add(tile.GetComponent<VillageResources>().GetIsConnected());
                    
                }
                

                hasRoad.Add(tile.GetComponentInChildren<RoadBuilder>().HasRoads());
                isFOW.Add(tile.GetIsFOW());
            } 
        }

        foreach (Vector2Int vec in gridBuildingSystem.villageLocations) villageLocations.Add(new int[] { vec.x, vec.y });
        foreach (Vector2Int vec in gridBuildingSystem.enemyCampLocations) enemyCampLocations.Add(new int[] { vec.x, vec.y });
        foreach (Vector2Int vec in gridBuildingSystem.woodResourceLocation) woodLocations.Add(new int[] { vec.x, vec.y });
        foreach (Vector2Int vec in gridBuildingSystem.goldResourceLocation) goldLocations.Add(new int[] { vec.x, vec.y });
        foreach (Vector2Int vec in gridBuildingSystem.foodResourceLocation) foodLocations.Add(new int[] { vec.x, vec.y });
    
        
    }
}
