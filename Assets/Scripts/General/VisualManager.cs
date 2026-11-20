using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    ResourceBuilder resourceBuilder;
    ResourceManagerVisuals resourceManagerVisuals;
    [SerializeField] MagicShop magicShop;

    Inventory playerInventory;
    PlayerLevelManager PLM;

    [SerializeField] GameObject tutorialGO;

    [SerializeField] List<GameObject> pageLabels;
    GameObject currentPageLabel;
    [SerializeField] GameObject magicShopGO;
    [SerializeField] GameObject inventoryBackgrounds;

    public Action<MagicShop.SaleType> OnChangeMagicPage;
    public Action OnChangeInventoryPage;

    bool IsInInventory;
    bool canUseVisual = true;

    private void Start()
    {
        currentPageLabel = pageLabels[0];

        resourceBuilder = mainManager.GetResourceBuilder();
        resourceManagerVisuals = mainManager.GetRMV();
        playerInventory = mainManager.GetInventory();
        PLM = playerInventory.GetComponent<PlayerLevelManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canUseVisual && !mainManager.GetIsInMainMenu())
        {
            DeactivateAll();
            mainManager.SetCanOpenMenu(true);
        } 
    }

    public void ChangePage(GameObject pageLabel)
    {
        pageLabel.transform.SetAsLastSibling();

        if (pageLabels[0] == pageLabel && currentPageLabel != pageLabel)
        {
            ActivateFirstPage();
        }
        else if (pageLabels[2] == pageLabel && currentPageLabel != pageLabel)
        {
            pageLabels[1].transform.SetAsFirstSibling();
            pageLabels[0].transform.SetAsFirstSibling();

            OnChangeMagicPage?.Invoke(MagicShop.SaleType.Skill);
        }
        else if (currentPageLabel != pageLabel)
        {
            ActivateSecondPage();
        }

        currentPageLabel = pageLabel;


    }
    public void ActivateFirstPage()
    {
        pageLabels[1].transform.SetAsFirstSibling();
        pageLabels[2].transform.SetAsFirstSibling();

        OnChangeMagicPage?.Invoke(MagicShop.SaleType.Potion);
        OnChangeInventoryPage?.Invoke();

        if (IsInInventory)
        {
            resourceManagerVisuals.SetAttackButtonsActive(true);
            resourceManagerVisuals.ActivateInventoryPage();
        }
    }
    void ActivateSecondPage()
    {
        pageLabels[0].transform.SetAsFirstSibling();
        pageLabels[2].transform.SetAsFirstSibling();

        OnChangeMagicPage?.Invoke(MagicShop.SaleType.Spell);

        if (IsInInventory)
        {
            resourceManagerVisuals.ActivateAttributePage();
            resourceManagerVisuals.SetAttackButtonsActive(false);
        }
    }

    public void OnOpenMagicShop()
    {
        pageLabels[0].transform.SetAsLastSibling();
        currentPageLabel = pageLabels[0];
    }
    public bool IsOnFirstPage()
    {
        return currentPageLabel == pageLabels[0];
    }
    public void ShowThirdLabel()
    {
        pageLabels[2].SetActive(true);
    }
    public void HideThirdLabel()
    {
        pageLabels[2].SetActive(false);
        pageLabels[0].transform.SetAsLastSibling();
    }

    public void BuilderActive()
    {

        playerInventory.DeactivateInventoryUI();
        resourceManagerVisuals.DeactivateUpgradeUI();
        resourceManagerVisuals.DeactivateMagicShopUI();
        resourceManagerVisuals.DeactivateBlacksmithUI();

        resourceManagerVisuals.ActivateBuilderUI();
    }
    public void InventoryActive()
    {
        mainManager.CanUseScripts(false);

        currentPageLabel = pageLabels[0];

        pageLabels[1].transform.SetAsFirstSibling();
        pageLabels[2].transform.SetAsFirstSibling();

        pageLabels[0].transform.SetAsLastSibling();

        SetIsInInventory(true);

        resourceManagerVisuals.DeactivateUpgradeUI();
        resourceManagerVisuals.DeactivateBuilderUI();
        resourceManagerVisuals.DeactivateMagicShopUI();
        resourceManagerVisuals.DeactivateBlacksmithUI();
        
        HideThirdLabel();
    }
    public void UpgradeActive()
    {
        playerInventory.DeactivateInventoryUI();
        resourceManagerVisuals.DeactivateBuilderUI();
        resourceManagerVisuals.DeactivateMagicShopUI();
        resourceManagerVisuals.DeactivateBlacksmithUI();
    }
    public void DeactivateAll()
    {
        resourceBuilder.SetIsBuilding(false);
        resourceBuilder.DeactivateBuilder();
        playerInventory.DeactivateInventoryUI();
        resourceManagerVisuals.DeactivateUpgradeUI();
        resourceManagerVisuals.DeactivateBuilderUI();
        resourceManagerVisuals.DeactivateMagicShopUI();
        resourceManagerVisuals.DeactivateBlacksmithUI();

        // maybe
        mainManager.SetCanOpenMenu(true);
        //tutorialGO.SetActive(false);
    }

    public void SetIsInInventory(bool option)
    {
        IsInInventory = option;
    }
    public void SetCanUseVisual(bool option)
    {
        canUseVisual = option;
    }
}
