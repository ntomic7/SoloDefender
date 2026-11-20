using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    BattleManager battleManager;

    // grid
    [SerializeField] GridBuildingSystem gridBuildingSystem;
    GridHexXZ<GridBuildingSystem.GridObject> grid;

    [SerializeField] List<GameObject> chunkTypesList;
    [SerializeField] List<TileObjects> tileTypesList;

    // tiles
    TileObjects magicShopTile;
    TileObjects blacksmithTile;
    TileObjects warehouseTile;

    [SerializeField] FogOfWar FOW;

    // player
    GameObject playerGO;
    PlayerLevelManager playerLevelManager;
    Attributes playerAttributes;
    Inventory inventory;
    [SerializeField] List<GameObject> attackButtonsList;

    // resources
    ResourceManager resourceManager;
    ResourceBuilder resourceBuilder;
    ResourceManagerVisuals RMV;

    Blacksmith blacksmith;
    MagicShop magicShop;

    Settlement settlement;
   

    // enemy
    EnemyManager enemyManager;

    int[] campCooldows = new int[3];

    // random
    [SerializeField] Teleport teleport;
    [SerializeField] SoundSystem soundSystem;
    List<GameObject> villageTiles;
    GameObject settlementGO;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;


    [SerializeField] Camera _camera;

    private void Start()
    {
        grid = gridBuildingSystem.GetGrid();

        playerLevelManager = mainManager.GetPlayerLevelManager();
        playerAttributes = mainManager.GetPlayerAttributes();
        inventory = mainManager.GetInventory();
        playerGO = playerLevelManager.gameObject;

        battleManager = mainManager.GetBattleManager();

        enemyManager = mainManager.GetEnemyManager();

        resourceManager = mainManager.GetResourceManager();
        resourceBuilder = mainManager.GetResourceBuilder();
        RMV = resourceManager.GetComponent<ResourceManagerVisuals>();

        blacksmith = resourceManager.GetComponent<Blacksmith>();
        magicShop = resourceManager.GetComponent<MagicShop>();


    }

    public void SaveAll()
    {
        if (!battleManager.GetIsBattling())
        {
            SaveSystem.SaveGrid(gridBuildingSystem);
            SaveSystem.SavePlayer(playerGO, attackButtonsList);
            SaveSystem.SaveResources(resourceManager, blacksmith, mainManager.GetSettlementScript());
            SaveSystem.SaveEnemies(enemyManager, mainManager, campCooldows);
            SaveSystem.SaveRandom(teleport, _camera, soundSystem);
            SaveSystem.SaveInventory(inventory);
        }
        else
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Error);
        }
    }
    public void LoadAll()
    {
        LoadGrid();

        StartCoroutine(WaitToLoad());
    }

    IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(.1f);

        LoadTiles();
        LoadPlayer();
        LoadEnemies();

        LoadResources();
        LoadRandom();
        LoadInventory();

        // visuals
        mainManager.ReturnToGame();
        resourceManager.RefreshAfterBuilding();

    }


    void LoadGrid()
    {
        GridData data = SaveSystem.LoadGrid();

        for(int i = 0; i < data.chunks.Count; i++)
        {
            gridBuildingSystem.finishedHexChunkList.Add(ChangeToChunk(data.chunks[i]));
        }

        foreach (int[] vec in data.villageLocations) gridBuildingSystem.villageLocations.Add(new Vector2Int(vec[0], vec[1]));
        foreach (int[] vec in data.enemyCampLocations) gridBuildingSystem.enemyCampLocations.Add(new Vector2Int(vec[0], vec[1]));
        foreach (int[] vec in data.woodLocations) gridBuildingSystem.woodResourceLocation.Add(new Vector2Int(vec[0], vec[1]));
        foreach (int[] vec in data.goldLocations) gridBuildingSystem.goldResourceLocation.Add(new Vector2Int(vec[0], vec[1]));
        foreach (int[] vec in data.foodLocations) gridBuildingSystem.foodResourceLocation.Add(new Vector2Int(vec[0], vec[1]));   
    }

    void LoadTiles()
    {
        GridData data = SaveSystem.LoadGrid();

        int villageIndex = 0;


        for (int i = 0; i < data.tiles.Count; i++)
        {
            TileObjects tile = grid.GetGridObject(data.tileCoords[i][0], data.tileCoords[i][1]).GetTileObject();

            tile.SetTier(data.tileTiers[i]);
            tile.ChangeFullTile(ChangeToTile(data.tiles[i]));
            tile.SetWorkers(data.workers[i]);

            if (tile.GetResourceType() == TileObjects.TileType.MagicStore) magicShopTile = tile;
            else if (tile.GetResourceType() == TileObjects.TileType.Blacksmith) blacksmithTile = tile;
            else if(tile.GetResourceType() == TileObjects.TileType.Warehouse) warehouseTile = tile;

            else if (tile.GetResourceType() == TileObjects.TileType.Village) CheckVillage(tile, data, ref villageIndex);

            else if (tile.GetResourceType() == TileObjects.TileType.Farm) resourceManager.FarmList.Add(tile);
            else if (tile.GetResourceType() == TileObjects.TileType.Lumberyard) resourceManager.LumberyardList.Add(tile);
            else if (tile.GetResourceType() == TileObjects.TileType.Goldmine) resourceManager.GoldmineList.Add(tile);

        }
        int roadIndex = 0;

        for (int i = 0; i < gridBuildingSystem.fullWidth; i++)
        {
            for (int j = 0; j < gridBuildingSystem.fullHeight; j++)
            {
                TileObjects tile = grid.GetGridObject(i, j).GetTileObject();

                if (data.hasRoad[roadIndex])
                {
                    tile.SetIsRoad();
                    tile.GetComponentInChildren<RoadBuilder>().PlaceRoad();
                }

                if (!data.isFOW[roadIndex])
                {
                    tile.SetIsFOW();
                    FOW.ClearTile(i, j);
                }

                roadIndex++;

            }
        }
    }



    void CheckVillage(TileObjects tile, GridData data, ref int index)
    {
        if (data.villageConnected[index]) tile.GetComponent<VillageResources>().SetIsConnected();

        index++;
    }

    void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        playerLevelManager.SetLevel(data.level);
        playerLevelManager.currentPoints = data.attributePoints;
        playerLevelManager.numOfActiveAttacks = data.numOfAttackButtons;

        playerLevelManager.SetXPafterLoad(data.currentXP);
        playerLevelManager.SetRequiredXP(data.xpToNextLvl);

        playerAttributes.UpgradeEquipment(IsSword: true, new Vector2Int(data.weaponMin, data.weaponMax));
        playerAttributes.UpgradeEquipment(IsSword: false, new Vector2Int(data.armorMin, data.armorMax));

        playerAttributes.baseAttack = data.attack;
        playerAttributes.baseDefence = data.defence;

        playerAttributes.ReloadMaxHealth(data.maxHealth);
        playerAttributes.currentHealth = data.currentHealth;
        playerAttributes.ReloadMaxMana(data.maxMana);
        playerAttributes.currentMana = data.currentMana;

        playerGO.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);

        for(int i = 0; i < 6; i++)
        {
            attackButtonsList[i].gameObject.SetActive(true);
        }

        SetAttackButtons(data.attackButtons);

        for (int i = 0; i < 6; i++)
        {
            attackButtonsList[i].gameObject.SetActive(false);
        }
    }
    void LoadResources()
    {
        ResourceData data = SaveSystem.LoadResources();

        resourceManager.currentWood = data.currentWood;
        resourceManager.currentFood = data.currentFood;
        resourceManager.currentGold = data.currentGold;
        resourceManager.currentPopulation= data.currentFreeWorkers;
        resourceManager.currentHouses = data.currentHouses;

        resourceManager.maxWood = data.maxWood;
        resourceManager.maxFood = data.maxFood;
        resourceManager.maxGold = data.maxGold;

        resourceBuilder.SetWarehouseTile(warehouseTile);
        resourceBuilder.SetMagicShop(magicShopTile);
        resourceBuilder.SetBlacksmith(blacksmithTile);

        settlement = mainManager.GetSettlementScript();
        settlement.ReloadSettlementHealth(data.settlementHealth, settlement.GetMaxSettlementhealth());

        if(settlement.GetSettlementHealth() < settlement.GetMaxSettlementhealth())
            RMV.SetRepairSettlement(true, enemyManager.GetIsAttackingSettlement(),
                 settlement.GetMaxSettlementhealth() - settlement.GetSettlementHealth());

        if (resourceBuilder.GetBlacksmith() != null)
        {
            blacksmith.ReloadTier(data.blacksmithTier, data.swordTier, data.armorTier);
        }
        if(resourceBuilder.GetMagicShop() != null)
        {   
            resourceBuilder.GetMagicShop().SetTier(data.magicShopTier);
        }

        if (data.maxxedOutBuildings[0]) RMV.DisableBuildingPossibility(TileObjects.TileType.House);
        if (data.maxxedOutBuildings[1]) RMV.DisableBuildingPossibility(TileObjects.TileType.Warehouse);
        if (data.maxxedOutBuildings[2]) RMV.DisableBuildingPossibility(TileObjects.TileType.MagicStore);
        if (data.maxxedOutBuildings[3]) RMV.DisableBuildingPossibility(TileObjects.TileType.Blacksmith);
        if (data.maxxedOutBuildings[4]) RMV.DisableBuildingPossibility(TileObjects.TileType.Castle);
    }
    void LoadEnemies()
    {
        EnemyData data = SaveSystem.LoadEnemies();

        List<GameObject> enemies = new List<GameObject>();

        for (int i = 0; i < data.numOfEnemies; i++)
        {
            GameObject enemy = enemyManager.GetEnemyType(data.enemyTypes[i]);
            enemy.transform.position = new Vector3(data.enemyPosition[i][0],
                data.enemyPosition[i][1], data.enemyPosition[i][2]);
            enemies.Add(enemy);

            EnemyBuildingAttack buildingAttack = enemy.GetComponent<EnemyBuildingAttack>();
            buildingAttack.IsAttackingSettlement = data.enemyAttackingSettlement[i];
            buildingAttack.IsAttackingVillage = data.enemyAttackingVillage[i];

            enemyManager.ReloadEnemy(enemy, enemy.transform.position, data.enemySettlementWait[i]);
        }

        enemyManager.SetIsAttackingSettlement(data.IsAttackingSettlement);

        //for (int i = 0; i < 3; i++)
        //{
        //    //Debug.Log(mainManager.GetEnemyCamps()[i]);
        //    mainManager.GetEnemyCamps()[i].SetCooldown(data.enemyCooldowns[i]);
        //}
    }
    void LoadRandom()
    {
        RandomData data = SaveSystem.LoadRandom();

        villageTiles = teleport.GetVillageTiles();
        settlementGO = mainManager.GetSettlement();

        for (int i = 0; i < 3; i++)
        {
            teleport.SetIsAttacked(villageTiles[i], data.activeTeleportAttacks[i]);
        }
        teleport.SetIsAttacked(settlementGO, data.activeTeleportAttacks[3]);

        _camera.transform.position = new Vector3(data.cameraPosition[0], data.cameraPosition[1], data.cameraPosition[2]);

        // sound

        soundSystem.SetMusicVolume(data.musicVolume);
        musicSlider.value = data.musicVolume;
        sfxSlider.value = data.sfxVolume;
        soundSystem.SetSFXVolume(sfxSlider);

    }
    void LoadInventory()
    {
        InventoryData data = SaveSystem.LoadInventory(); 

        for(int i = 0; i < data.inventoryItems.Count; i++)
        {
         inventory.ReloadItem(ChangeToSpellObject(data.inventoryItems[i]), data.inventoryAmounts[i]);
        }

    }

    public SpellObjects ChangeToSpellObject(string name)
    {
        foreach(SpellObjects item in magicShop.GetPotionList())
        {
            if (name == item.GetName()) return item;
        }
        foreach (SpellObjects item in magicShop.GetSkillList())
        {
            if (name == item.GetName()) return item;
        }
        foreach (SpellObjects item in magicShop.GetSpellList())
        {
            if (name == item.GetName()) return item;
        }
        return null;
    }
    void SetAttackButtons(string[] attackButtons)
    {
        for (int i = 0; i < 6; i++)
        {
            if (attackButtons[i] != null)
            {
                SpellObjects item = ChangeToSpellObject(attackButtons[i]);
                GameObject button = attackButtonsList[i].gameObject;
                button.GetComponent<BattleResourcesManager>().OnChangeAttackButton(item, button);
            }
        }
    }
    public GameObject ChangeToChunk(string name)
    {
        foreach(GameObject chunk in chunkTypesList)
        {
            if (chunk.name == name)return chunk;
        }

        return null;
    }
    public TileObjects ChangeToTile(string name)
    {
        foreach (TileObjects tileType in tileTypesList)
        {
           
            if (tileType.GetName() == name)
            {
                return tileType;
            }
        }

        return null;
    }


}
