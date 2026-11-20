using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    ResourceManager resourceManager;
    Attributes playerAttributes;
    Animator playerAnimator;
    PlayerLevelManager playerLevelManager;
    BattleVisuals battleVisuals;
    Camera mainCamera;
    CameraMovement cameraMovement;
    Tutorial tutorial;

    bool dead;
    int amountTakenBuildings = 20;
    int amountTakenOther = 5;
    [SerializeField] GameObject playerGO;
    Vector3 playerStartingPosition;

    // UI
    [SerializeField] GameObject DeathPanel;
    [SerializeField] GameObject SurrenderObject;
    [SerializeField] GameObject playerBackground;
    SpriteRenderer sprite;
    [SerializeField] GameObject playerSprite;

    [SerializeField] List<TextMeshProUGUI> textList;
    [SerializeField] List<TextMeshProUGUI> sacrificeTexts;
    [SerializeField] List<Image> currentImages;
    [SerializeField] List<Sprite> fullImageList;

    // resources
    int food; int wood; int gold; int workers; int health; int mana;
    [SerializeField] GameObject currentResourcesUI;

    List<int> resourceList;
    List<ResourceType> fullResourceList;
    List<ResourceType> chosenResources;

    List<ResourceType> buildingResources;

    private void Start()
    {
        resourceManager = mainManager.GetResourceManager();
        playerAttributes = mainManager.GetPlayerAttributes();
        playerAnimator = playerAttributes.GetComponentInChildren<Animator>();
        playerLevelManager = playerAttributes.GetComponent<PlayerLevelManager>();
        battleVisuals = mainManager.GetBattleVisuals();
        mainCamera = mainManager.GetCamera();
        cameraMovement = mainCamera.GetComponent<CameraMovement>();
        tutorial = mainManager.GetTutorial();

        playerStartingPosition = playerGO.transform.position;

        fullResourceList = new List<ResourceType>() { ResourceType.Wood, ResourceType.Food, ResourceType.Gold,
                                                  ResourceType.Workers, ResourceType.Health, ResourceType.Mana };
        buildingResources = new List<ResourceType>() { fullResourceList[0], fullResourceList[1], fullResourceList[2] };
    }

    public void ChooseSacrifice(int index)
    {
        ResourceType chosenSacrifice = chosenResources[index];
        bool sacrificing = false;

        switch (chosenSacrifice)
        {
            case ResourceType.Food:
            {
                    if (amountTakenBuildings <= resourceManager.GetCurrentFood())
                    {
                        resourceManager.SetCurrentFood(-amountTakenBuildings);
                        sacrificing = true;
                    }
                break;
            }
            case ResourceType.Wood:
            {
                    if (amountTakenBuildings <= resourceManager.GetCurrentWood())
                    {
                        resourceManager.SetCurrentWood(-amountTakenBuildings);
                        sacrificing = true;
                    }
                        break;
                }
            case ResourceType.Gold:
            {
                    if (amountTakenBuildings <= resourceManager.GetCurrentGold())
                    {
                        resourceManager.SetCurrentGold(-amountTakenBuildings);
                        sacrificing = true;
                    }
                    break;
                }   
            case ResourceType.Workers:
            {
                    if (amountTakenOther <= resourceManager.GetCurrentWorkers())
                    {
                        resourceManager.SetCurrentWorkers(-amountTakenOther);
                        sacrificing = true;
                    }
                    break;
                }
            case ResourceType.Health:
            {
                    if (amountTakenOther <= playerAttributes.GetMaxHealth())
                    {
                        playerAttributes.SetMaxHealth(-amountTakenOther);
                        sacrificing = true;
                    }
                break;
                }
            case ResourceType.Mana:
            {
                    if (amountTakenOther <= playerAttributes.GetMaxMana())
                    {
                        playerAttributes.SetMaxMana(-amountTakenOther);
                        sacrificing = true;
                    }
                break;
                }
        }
        if (sacrificing)
        {

            ActivateAnimator(currentImages[index]);
            StartCoroutine(WaitForSacrificeAnim(currentImages[index]));
        }
        else Debug.Log("not enough");
    }
    public int GetSacrificeAmount(ResourceType type, bool IsCurrentAmount)
    {
        switch (type)
        {
            case ResourceType.Food:
                {
                    if (IsCurrentAmount) return resourceManager.GetCurrentFood();
                    return resourceManager.GetCurrentFood() - amountTakenBuildings;
                }
            case ResourceType.Wood:
                {
                    if (IsCurrentAmount) return resourceManager.GetCurrentWood();
                    return resourceManager.GetCurrentWood() - amountTakenBuildings;
                }
            case ResourceType.Gold:
                {
                    if (IsCurrentAmount) return resourceManager.GetCurrentGold();
                    return resourceManager.GetCurrentGold() - amountTakenBuildings;
                }
            case ResourceType.Workers:
                {
                    if (IsCurrentAmount) return resourceManager.GetCurrentWorkers();
                    return resourceManager.GetCurrentWorkers() - amountTakenOther;
                }
            case ResourceType.Health:
                {
                    if (IsCurrentAmount) return playerAttributes.GetMaxHealth();
                    return playerAttributes.GetMaxHealth() - amountTakenOther;
                }
            case ResourceType.Mana:
                {
                    if(IsCurrentAmount) return playerAttributes.GetMaxMana();
                    return playerAttributes.GetMaxMana();
                }
        }
        return 0;
    }

    [EditorCools.Button]
    public void OnDeath()
    {
        RandomizeSacrifices();

        cameraMovement.SetCanMove(false);
        playerBackground.SetActive(true);
        sprite = playerBackground.GetComponent<SpriteRenderer>();
        sprite.color = new Color32(0, 0, 0, 0);

        tutorial.SkipTutorial();

        StartCoroutine(DeathAnimation());
    }
    void RandomizeSacrifices()
    {
        GetResources();

       chosenResources = new List<ResourceType>();
        List<ResourceType> notEnoughResourcesList = new List<ResourceType>();

        for(int i = 0; i < fullResourceList.Count; i++)
        {
            ResourceType random = (ResourceType)UnityEngine.Random.Range(0, ResourceType.GetNames(typeof(ResourceType)).Length);

            if (!chosenResources.Contains(random) && GetSacrificeAmount(random, IsCurrentAmount:false) > 0
                && chosenResources.Count < 3) chosenResources.Add(random);
            else notEnoughResourcesList.Add(random);
        }

        while (chosenResources.Count < 3)
        {
            ResourceType random = notEnoughResourcesList[UnityEngine.Random.Range(0, notEnoughResourcesList.Count)];
            if (!chosenResources.Contains(random))
            {
                chosenResources.Add(random);
                notEnoughResourcesList.Remove(random);
            }
        }

        // check if there is anything to sacrifice
        bool notEnoughResources = false;
        for (int i = 0; i < 3; i++)
        {
            if (GetSacrificeAmount(chosenResources[i], IsCurrentAmount:false) < 0) notEnoughResources = true;
            else
            {
                notEnoughResources = false;
                return;
            }
            
        }

        if (notEnoughResources)
        {
            SurrenderObject.SetActive(true);
        }
    }
    void GetResources()
    {
        food = resourceManager.GetCurrentFood();
        wood = resourceManager.GetCurrentWood();
        gold = resourceManager.GetCurrentGold();
        workers = resourceManager.GetCurrentWorkers(); 

        health = playerAttributes.GetHealth();
        mana = playerAttributes.GetMana();

        resourceList = new List<int>() { food, wood, gold, workers, health, mana };
    }

    void ActivateAnimator(Image image)
    {
        image.GetComponentInChildren<Animator>(true).gameObject.SetActive(true);
        image.GetComponentInChildren<Animator>().SetTrigger("OnSacrifice");
    }
    void SetUpSacrificeVisuals()
    {
        int amount;

        for (int i = 0; i < 3; i++)
        {
            if (buildingResources.Contains(chosenResources[i])) amount = amountTakenBuildings;
            else amount = amountTakenOther;

            textList[i].text = $"{chosenResources[i].ToString()}\n-{amount}\n (current {GetSacrificeAmount(chosenResources[i], IsCurrentAmount:true)})";
            currentImages[i].sprite = fullImageList[fullResourceList.IndexOf(chosenResources[i])];
        }
    }
    void SetUpAfterSacrificeVisuals()
    {
        amountTakenBuildings += (int)(amountTakenBuildings * .4f);
        amountTakenOther += (int)(amountTakenOther * .4f);
        DeathPanel.SetActive(false);

        playerAttributes.OnPlayerDeath();
        playerAnimator.SetTrigger("OnSacrifice");
        SoundSystem.instance.ChangeMusic(SoundSystem.Music.Main);

        playerLevelManager.RefreshText();
        battleVisuals.RoundVisualsPlayer(playerAttributes.GetMana(), playerAttributes.GetHealth());
        resourceManager.RefreshAfterBuilding();
        mainManager.ChooseScriptActivity(true);

        cameraMovement.SetCanMove(true);
        mainCamera.transform.position = new Vector3(playerAttributes.transform.position.x, playerAttributes.transform.position.y, -10);
    }


    IEnumerator WaitForSacrificeAnim(Image image)
    {
        mainManager.OnPlayerDeathAction();
        playerGO.transform.position = playerStartingPosition;

        yield return new WaitForSeconds(1.5f);

        image.GetComponentInChildren<Animator>().gameObject.SetActive(false);

        SetUpAfterSacrificeVisuals();
    }
    IEnumerator DeathAnimation()
    {
        mainManager.ChooseScriptActivity(false);

        float fadeDuration = 1.8f; 
        float elapsedTime = 0f;

        sprite.color = new Color(0, 0, 0, 0);

        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            sprite.color = new Color(0, 0, 0, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerBackground.SetActive(false);
        DeathPanel.SetActive(true);

        SetUpSacrificeVisuals();

    }

    public enum ResourceType
    {
        Food,
        Wood,
        Gold,
        Workers,
        Health,
        Mana
    }
}
