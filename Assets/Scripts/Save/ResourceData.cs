using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceData
{
    public int currentWood, currentGold, currentFood, currentFreeWorkers, currentHouses;
    public int maxWood, maxGold, maxFood;

    public int blacksmithTier, swordTier, armorTier, magicShopTier;

    public string warehouseTile = null;
    public string magicShopTile = null;
    public string blacksmithTile = null;
    public string settlementTile = null;

    public int settlementHealth;

    public bool[] maxxedOutBuildings;

    public ResourceData(ResourceManager resourceManager, Blacksmith blacksmith, Settlement settlement)
    {
        currentWood = resourceManager.GetCurrentWood();
        currentFood = resourceManager.GetCurrentFood();
        currentGold = resourceManager.GetCurrentGold();
        currentFreeWorkers = resourceManager.GetCurrentFreeWorkers();
        currentHouses = resourceManager.GetCurrentHouses();

        maxWood = resourceManager.GetMaxWood();
        maxFood = maxWood;
        maxGold = maxWood;

        maxxedOutBuildings = new bool[5];


        ResourceBuilder resourceBuilder = resourceManager.GetComponent<ResourceBuilder>();
        ResourceManagerVisuals RMV = resourceManager.GetComponent<ResourceManagerVisuals>();

        if (resourceBuilder.GetWarehouseTile() != null) warehouseTile = resourceBuilder.GetWarehouseTile().GetName();
        if (resourceBuilder.GetSettlement() != null) settlementTile = resourceBuilder.GetSettlement().GetName();
    
        
        if (resourceBuilder.GetBlacksmith() != null)
        {
            blacksmithTile = resourceBuilder.GetBlacksmith().GetName();

            blacksmithTier = blacksmith.GetTier();
            swordTier = blacksmith.GetSwordTier();
            armorTier = blacksmith.GetArmorTier();    
        }
        if(resourceBuilder.GetMagicShop() != null)
        {
            magicShopTile = resourceBuilder.GetMagicShop().GetName();
            magicShopTier = resourceBuilder.GetMagicShop().GetTier();
        }

        settlementHealth = settlement.GetSettlementHealth();
       
        maxxedOutBuildings = RMV.maxxedOutBuildings;            
    }
}
