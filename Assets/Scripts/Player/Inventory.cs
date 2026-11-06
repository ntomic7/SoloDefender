using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    VisualManager visualManager;
    ResourceManager resourceManager;
    ResourceManagerVisuals resourceManagerVisuals;
    BattleManager battleManager;
    [SerializeField] MagicShop magicShop;

    bool canUseInventory = true;
    public bool IsInInventory;
    bool IsBattling;

    // UI
    [SerializeField] GameObject PlayUI;
    [SerializeField] GameObject InventoryUI;
    [SerializeField] GameObject inventoryBackgrounds;

    [SerializeField] TextMeshProUGUI descriptionText;

    [SerializeField] List<GameObject> inventoryObjects;
    string potion = "Potions", spells = "Spells";

    public Action<SpellObjects, GameObject> OnItemChosen;
    public Action<bool, GameObject> onItemChange;

    // items

    Dictionary<SpellObjects, int> inventoryDict = new Dictionary<SpellObjects, int>();
    List<SpellObjects> potionInventory;
    List<SpellObjects> spellInventory;
    List<SpellObjects> skillInventory;

    SpellObjects currentItem;
    [SerializeField] Animator itemAnimator;

    //testing
    [SerializeField] GameObject itemAmountGO;
    [SerializeField] List<SpellObjects> items;
    //

    // changing attack button
    SpellObjects chosenSpellObject;

    private void Start()
    {
        visualManager = mainManager.GetVisualManager();
        resourceManager = mainManager.GetResourceManager();
        resourceManagerVisuals = resourceManager.gameObject.GetComponent<ResourceManagerVisuals>();

        battleManager = mainManager.GetBattleManager();

        magicShop.OnChooseItem += SetCurrentItem;
        battleManager.OnBattleStart += SetIsBattling;
        visualManager.OnChangeInventoryPage += ActivateInventoryItemsUI;

        // remove later
        foreach (SpellObjects spellObjects in items)
        {
            if (!inventoryDict.ContainsKey(spellObjects))
                inventoryDict.Add(spellObjects, 1);
            else
                inventoryDict[spellObjects] += 1;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ActivateInventoryUI();
        }
    }

    public void ActivateInventoryUI()
    {
        if (!IsInInventory && !IsBattling && canUseInventory)
        {
            resourceManagerVisuals.ActivateInventoryUI();

            IsInInventory = true;
            magicShop.SetIsInInventory(true);
            mainManager.SetMoveCamera(false);
            mainManager.SetCanOpenMenu(false);

            ActivateInventoryItemsUI();
        }
        else if (IsInInventory && !IsBattling)
        {
            DeactivateInventoryUI();
        }
    }
    public void DeactivateInventoryUI()
    {
        IsInInventory = false;
        magicShop.SetIsInInventory(false);

        mainManager.SetMoveCamera(true);
        mainManager.CanUseScripts(true);

        resourceManagerVisuals.DeactivateInventoryUI();
    }

    // inventory item showcase

    public void ManageInventoryItems(GameObject chosenItem)
    {
        if (chosenItem.GetComponent<BattleResourcesManager>() != null && !battleManager.GetIsBattling())
        {
            ShowItemDescription(chosenItem.GetComponent<BattleResourcesManager>().GetResource());
        }
    }
    public void HideItemDescription()
    {
        descriptionText.text = "";
    }
    public SpellObjects GetChosenSpellObject(GameObject chosenItem)
    {
        if (!IsBattling)
        {
            int index = 0;

            GameObject parentObject = chosenItem.transform.parent.gameObject;
            List<SpellObjects> inventory = ChooseInventoryOption(parentObject);

            while (index < 8)
            {

                if (parentObject.transform.GetChild(index).gameObject == chosenItem)
                {
                    return inventory[index];
                }
                index++;
            }
        }
        return null;

    }
    public void ManageShowDescription(GameObject chosenItem)
    {
        ShowItemDescription(GetChosenSpellObject(chosenItem));
    }
    void ShowItemDescription(SpellObjects spellObject)
    {
        if (!IsBattling)
            descriptionText.text = spellObject.GetDescription();
    }
  


    // activate inventory
    public void ActivateInventoryItemsUI()
    {
        DeactivateInventoryItemsUI();

        int potionIndex = 0;
        int spellIndex = 0;
        int skillIndex = 0;

        float iconScale = 1.14f;

        potionInventory = new List<SpellObjects>();
        spellInventory = new List<SpellObjects>();
        skillInventory = new List<SpellObjects>();

        foreach (SpellObjects so in inventoryDict.Keys)
        {
            GameObject go;

            if (so.GetIsPotion() && inventoryDict[so] > 0)
            {
                go = inventoryObjects[0].transform.GetChild(potionIndex).gameObject;

                SetUpPotionAmounts(so, go);

                potionIndex++;
            }
            else if (so.GetIsSkill() && inventoryDict[so] > 0)
            {
                go = inventoryObjects[1].transform.GetChild(skillIndex).gameObject;
                skillInventory.Add(so);

                skillIndex++;
            }
            else if (inventoryDict[so] > 0)
            {
                go = inventoryObjects[2].transform.GetChild(spellIndex).gameObject;
                spellInventory.Add(so);

                spellIndex++;
            }
            else go = null;

            go.SetActive(true);
            go.GetComponent<Image>().sprite = so.GetItemSprite();
            if (!so.GetIsPotion()) go.transform.localScale = new Vector3(iconScale, iconScale, iconScale);
        }
    }
    void DeactivateInventoryItemsUI()
    {
        foreach(GameObject inventoryObj in inventoryObjects)
        {
            int index = 0;

            while(index < 8)
            {
                inventoryObj.transform.GetChild(index).GetComponent<Image>().sprite = null;
                inventoryObj.transform.GetChild(index).gameObject.SetActive(false);


                if(inventoryObj.transform.GetChild(index).childCount > 0)
                Destroy(inventoryObj.transform.GetChild(index).GetChild(0).gameObject);

                index++;
            }
        }
    }
    void SetUpPotionAmounts(SpellObjects so, GameObject go)
    {
        potionInventory.Add(so);

        GameObject itemAmount = Instantiate(itemAmountGO, go.transform);
        itemAmount.transform.position = new Vector3(go.transform.position.x + 21, go.transform.position.y - 25, go.transform.position.z);
        itemAmount.GetComponentInChildren<TextMeshProUGUI>().text = inventoryDict[so].ToString();
    }

    //set up attacks
    public void SetCurrentAttackButton(GameObject chosenBRM)
    {
        if (chosenSpellObject != null && !battleManager.GetIsBattling())
        {
            OnItemChosen?.Invoke(chosenSpellObject, chosenBRM);
            onItemChange?.Invoke(false, chosenBRM);

            chosenSpellObject = null;
        }
    }
    List<SpellObjects> ChooseInventoryOption(GameObject spellObject)
    {
        if (spellObject.name == potion) return potionInventory;
        else if (spellObject.name == spells) return spellInventory;
        else return skillInventory;
    }


    public Dictionary<SpellObjects, int> GetInventoryDict()
    {
        return inventoryDict;
    }

    // manage inventory

    public void AddItem()
    {
        if (currentItem != null &&  resourceManager.GetCurrentGold() < currentItem.GetPrice())
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Error);

        }
        else if (currentItem != null)
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Purchase);
            if (!inventoryDict.ContainsKey(currentItem))
            {
                inventoryDict.Add(currentItem, 1);
                resourceManager.SetCurrentGold(-currentItem.GetPrice());
                Debug.Log(currentItem.GetPrice());

                if (!currentItem.IsPotion) magicShop.RemoveFromShopInventory(currentItem);
            }
            else
            {
                resourceManager.SetCurrentGold(-currentItem.GetPrice());
                inventoryDict[currentItem] += 1;

                Debug.Log(currentItem.GetPrice());

            }

            itemAnimator.SetTrigger("OnBought");
            resourceManager.RefreshAfterBuilding();
            Debug.Log("refrenshed");
        }
    }
    public void ReloadItem(SpellObjects item, int amount)
    {
        if(!inventoryDict.ContainsKey(item)) inventoryDict.Add(item, amount);
    }
    public bool RemoveItem(SpellObjects item)
    {
        if (inventoryDict.ContainsKey(item) && inventoryDict[item] > 0)
        {
            inventoryDict[item] -= 1;

            if (inventoryDict[item] <= 0)
            {
                inventoryDict.Remove(item);
                return false;
            }
            
            return true;

            }
        else
        {
            return false;
        }
    }
    public bool HasPotion(SpellObjects spellObject)
    {
        if(inventoryDict.ContainsKey(spellObject)) return true;
        return false;
    }
    public int GetAmountOfItem(SpellObjects spellObject)
    {
        if(inventoryDict.ContainsKey(spellObject)) return inventoryDict[spellObject];

        return 0;
    }

    public void ChangeItem(GameObject go)
    {
        if (GetChosenSpellObject(go) != null)
        {
            onItemChange?.Invoke(true, go);
            chosenSpellObject = GetChosenSpellObject(go);
        }
    }
    void SetCurrentItem(SpellObjects spellObject)
    {
        currentItem = spellObject;
    }
    public void SetIsBattling(bool option)
    {
        IsBattling = option;
    }
    public void SetCanUseInventory(bool option)
    {
        canUseInventory = option;
    }
}
