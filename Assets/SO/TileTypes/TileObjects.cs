using System;
using System.Collections.Generic;
using UnityEngine;

public class TileObjects : MonoBehaviour
{
    [SerializeField] TileTypesSO tileTypeSO;
    [SerializeField] SpriteRenderer buildingSprite;
    SpriteRenderer FOWsprite;
    [SerializeField] List<Sprite> tierSprites;

    [SerializeField] bool IsOccupied;
    [SerializeField] bool IsInteractable;
    bool IsRoad;
    [SerializeField] bool IsFOW = true;

    public TileType tileType;
    int tier = 0;
    [SerializeField] int currentWorkers;
    int maxWorkers = 5;
    // put only for houses
    [SerializeField] int maxOfBuildings;
    [SerializeField] string description;

    public Action OnGuardNumChanged;

    
    private void Start()
    {
        if (transform.Find("FOW") != null)
        {
            FOWsprite = transform.Find("FOW").GetComponent<SpriteRenderer>();
        }
    }

    public int GetMaxBuildingNum()
    {
        return maxOfBuildings;
    }
    public int GetCurrentWorkers()
    {
        return currentWorkers;
    }
    public void AddWorkers(int amount)
    {
        currentWorkers += amount;

        if (tileType == TileType.Castle) OnGuardNumChanged?.Invoke();

    }
    public void SetWorkers(int amount)
    {
        currentWorkers = amount;

        if (tileType == TileType.Castle) OnGuardNumChanged?.Invoke();

    }
    public int GetMaxWorkers()
    {
        return maxWorkers;
    }
    public void AddMaxWorkers(int amount)
    {
        maxWorkers += amount;
    }
    public int GetBuildingPrice()
    {
        return tileTypeSO.price;
    }
    public int GetTier()
    {
        return tier;
    }
    public void SetTier(int amount)
    {
        tier = amount;
    }
    public void UpgradeTier()
    {
        tier++;
    }
    public void DegradeTier()
    {
        if(tier > 0) tier--;
        ChangeBuildingSprite();
    }


    public bool GetIsOccupied()
    {
        return IsOccupied;
    }
    public void SetIsOccupied(bool option)
    {
        IsOccupied = option;
    }    
    public bool GetIsBuildingTile()
    {
        return tileTypeSO.IsBuildingLand;  
    }
    public bool GetIsInteractable()
    {
        return IsInteractable;
    }
    public bool GetIsRoad()
    {
        return IsRoad;
    }
    public void SetIsRoad()
    {
        IsRoad = true;
    }
    public bool GetIsFOW()
    {
        return IsFOW;
    }
    public void SetIsFOW()
    {
        IsFOW = false;
    }


    public string GetName()
    {
        return tileTypeSO.tileName;
    }
    public string GetDescription()
    {
        return description;
    }
    public TileType GetResourceType()
    {
        return tileType;
    }
    public TileTypesSO GetTileTypeSO()
    {
        return tileTypeSO;
    }
    public List<GameObject> GetCampEnemies()
    {
        return GetComponentInParent<EnemyCampSpawner>().GetEnemyTypesList();
    }
    public Sprite GetBuildingSprite()
    {
        return buildingSprite.sprite;
    }
    public Sprite SetBuildingSprite()
    {
        return tierSprites[tier];
    }
    public SpriteRenderer GetFOW()
    {
        return FOWsprite;
    }
    
    public List<Sprite> GetVillageSpriteList()
    {
        return tileTypeSO.houseList;
    }
    public List<Sprite> GetSpriteList()
    {
        return tierSprites;
    }

    public void ChangeBuildingSprite()
    {
        buildingSprite.sprite = tierSprites[tier];

        if(this.tileType == TileType.Village)
        {
            GetComponent<VillageResources>().UpgradeVillageVisuals(tier);
        }
    }
    public void ChangeFullTile(TileObjects tileObject)
    {
        this.tierSprites = tileObject.GetSpriteList();
        this.tileTypeSO = tileObject.GetTileTypeSO();
        this.tileType = tileObject.GetResourceType();
        this.maxOfBuildings = tileObject.maxOfBuildings;
        this.buildingSprite.sprite = SetBuildingSprite();
        this.IsInteractable = tileObject.GetIsInteractable();
        this.IsOccupied = tileObject.GetIsOccupied();
    }

    public enum TileType
    {
        Grass,
        Other,
        Lumberyard,
        Goldmine,
        Farm,
        House,
        Warehouse,
        MagicStore,
        Blacksmith,
        Castle,
        Village,
        Road,
        EnemyCamp
    }

}


