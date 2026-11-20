using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    TurnManager turnManager;

    GridBuildingSystem gridBuildingSystem;
    GridHexXZ<GridBuildingSystem.GridObject> grid;
    [SerializeField] FogOfWar FOW;
    CameraMovement cameraMovement;
    
    ResourceBuilder resourceBuilder;
    ResourceManager resourceManager;
    PlayerMovement playerMovement;

    int pageIndex = 0;

    public bool IsInTutorial = true;
    int movementCount;

    string startingText = "Move by pressing one of the hexagons around you";

    List<Vector3> resourceLocations = new List<Vector3>();

    // visuals 
    [SerializeField] GameObject tutorialBox;

    [SerializeField] GameObject allButtons;
    [SerializeField] GameObject nextButton;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] TextMeshProUGUI skipButtonText;
    [SerializeField] GameObject resourceIcons;
    [SerializeField] GameObject builderArrowIcon;
    [SerializeField] GameObject settlementArrowIcon;

    [SerializeField] List<Sprite> villageImages; 

    void Start()
    {
        turnManager = mainManager.GetTurnManager();

        gridBuildingSystem = mainManager.GetGridBuildingSystem();
        grid = mainManager.GetGrid();
        cameraMovement = mainManager.GetCamera().GetComponent<CameraMovement>();
        
        resourceManager = mainManager.GetResourceManager();
        resourceBuilder = mainManager.GetResourceBuilder();
        playerMovement = mainManager.GetPlayerMovement();

        mainManager.OnMapAwake += OnMapAwake;
        turnManager.OnEndOfTurn += OnNextTurn;
        resourceManager.OnBuildingUpgrade += ActivateFromUpgrade;
        resourceBuilder.OnResourceLocationAdded += LoadResourceLocations;
        resourceBuilder.OnBuilding += ShowUpgrade;
    }

    private void Update()
    {
        if(pageIndex == 2 && resourceBuilder.IsUsingBuilder && IsInTutorial)
        {
            Build();
        }        
    }

    void OnMapAwake()
    {
        mainManager.OnMapAwake -= OnMapAwake;
    }
    void LoadResourceLocations(Vector3 position)
    {
        resourceLocations.Add(position);

        if (resourceLocations.Count >= 12) resourceBuilder.OnResourceLocationAdded -= LoadResourceLocations;
    }

    void OnNextTurn()
    {
        if (pageIndex <= 0) CheckMovement();
        if (pageIndex <= 1) CheckResources();
    

    }
    void CheckMovement()
    {
        movementCount++;

        if (movementCount >= 5)
        {
            pageIndex++;
            //nextButton.SetActive(true);
            ResourceUI();
        }
    }
    void CheckResources()
    {
        Vector2Int playerPosition = playerMovement.GetCurrentPlayerPosition();

        foreach(Vector3 vec in resourceLocations)
        {
            if (Mathf.Abs(playerMovement.transform.position.x - vec.x) < 1.5f && Mathf.Abs(playerMovement.transform.position.y - vec.y) < 1.5f)
            {
                FOW.ClearFOW(vec);
            
                resourceIcons.SetActive(false);
                pageIndex = 2;
                OpenBuild(); 
            }
        }
    }

    void ResourceUI()
    {

        tutorialText.text = "Find ruins to build new resource buildings";
        resourceIcons.SetActive(true);
    }
    void OpenBuild()
    {
        nextButton.SetActive(true);
        tutorialText.text = "To open the Build Menu press 'B' or click the Build Button";
        builderArrowIcon.SetActive(true);
    }
    void Build()
    {
        pageIndex++;
        builderArrowIcon.SetActive(false);
        tutorialText.text = "Choose the appropriate resource for the ruin";
    }
    void ShowUpgrade()
    {
        if (pageIndex == 3)
        {
            pageIndex++;
            tutorialText.text = "Buildings produce resources only when there are workers active. Press on the building to add workers.";
        }
       
    }
    void ActivateFromUpgrade()
    {
        if(pageIndex == 4)
        {
            ExplainVillage();
            nextButton.SetActive(true);
        }
        
        else if(pageIndex == 6)
        {
            ExplainSettlementUpgrade();
            nextButton.SetActive(true);
        }
    }
    public void ActivateNextButton()
    {
        Debug.Log(pageIndex);

        if(pageIndex == 1)
        {
            //pageIndex++;

            Build();
        }
        else if(pageIndex == 2)
        {
            pageIndex++;
            ShowUpgrade();
            builderArrowIcon.SetActive(false);
        }
        else if(pageIndex == 4)
        {
            ExplainVillage();
        }
        else if (pageIndex == 5)
        {
            resourceIcons.SetActive(false);

            ExplainSettlement();
        }
        else if( pageIndex == 6)
        {
            ExplainSettlementUpgrade();
        }
        else if (pageIndex == 7)
        {
            ExplainCamp();
            //ShowAllButtons();
        }
        else if (pageIndex == 8)
        {
            ShowAllButtons();
        }
    }
    void ShowAllButtons()
    {
        settlementArrowIcon.SetActive(false);
        nextButton.SetActive(false);
        resourceIcons.SetActive(false);

        allButtons.SetActive(true);
        tutorialText.text = "Close the tutorial when you're ready.";
        skipButtonText.text = "EXIT";
    }
    

    void ExplainSettlement()
    {
        pageIndex++;

        settlementArrowIcon.SetActive(true);
       
        cameraMovement.OnFindettlementButtonPressed();
        tutorialText.text = "The settlement needs to be protected. If it is destroyed you lose. Add Guards to help protect it.";

    }
    void ExplainSettlementUpgrade()
    {
        pageIndex++;
        settlementArrowIcon.SetActive(false);
        tutorialText.text = "The settlement can be upgraded to hold more guards.";
    }
    void ExplainVillage()
    {
        pageIndex++;
        tutorialText.text = "Villages need to be connected with the settlement by a road to produce resources.";

        resourceIcons.SetActive(true);
        for(int i = 0; i < 6;  i++)
        {
            if (i >= 2) resourceIcons.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            else
            {
                GameObject go = resourceIcons.gameObject.transform.GetChild(i).gameObject;
                go.SetActive(true);

                RectTransform rectTransform = go.GetComponent<RectTransform>();

                if (i == 0) rectTransform.anchoredPosition = new Vector2(134.9f, -27.8f);
                else if (i == 1) rectTransform.anchoredPosition = new Vector2(133.1f, -44.6f);

                go.GetComponent<Image>().sprite = villageImages[i];
            }
        }
    }
    void ExplainCamp()
    {
        pageIndex++;
        tutorialText.text = "Destroy three enemy camps to complete the game.";

        resourceIcons.SetActive(true);

        resourceIcons.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        GameObject go = resourceIcons.gameObject.transform.GetChild(1).gameObject;
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        go.GetComponent<Image>().sprite = villageImages[2];
        rectTransform.anchoredPosition = new Vector2(131, - 38.3f);
        rectTransform.localScale = new Vector2(.3f, .3f);
    
    
    }

    public void ReopenTutorial()
    {
        tutorialText.text = startingText;
        pageIndex = 0;
        movementCount = 0;
        skipButtonText.text = "SKIP TUTORIAL";

        IsInTutorial = true;
        CloseAllImages();
        tutorialBox.SetActive(true);
    }
    void CloseAllImages()
    {
        nextButton.SetActive(false);
        allButtons.SetActive(false);
        resourceIcons.SetActive(false);
        builderArrowIcon.SetActive(false);
        settlementArrowIcon.SetActive(false);
    }
        
    public void HideTutorial()
    {
        tutorialBox.SetActive(false);
    }
    public void RevealTutorial()
    {
        tutorialBox.SetActive(true);
    }
    public void SkipTutorial()
    {
        tutorialBox.SetActive(false);

        CloseAllImages();

        IsInTutorial = false;
        Debug.Log("closing tutorial");
    }
    
}
