using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MAINmanager : MonoBehaviour
{
    // main menu
    Save save;

    MenuOption menuOption;
    bool CanOpenMenu = false;
    bool InMenu;
    bool InMainMenu = true;

    public System.Action OnMapAwake;

    [SerializeField] GameObject playCanvas;
    [SerializeField] GameObject mainMenuCanvas;

    [SerializeField] GameObject background;

    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] GameObject GameOverMenu;


    //
    [SerializeField] Camera mainCamera;
    CameraMovement cameraMovement;
    [SerializeField] Teleport teleport;
    [SerializeField] FogOfWar FOW;
    [SerializeField] Tutorial tutorial;

    [SerializeField] GridBuildingSystem gridBuildingSystem;
    GridHexXZ<GridBuildingSystem.GridObject> gridHex;
    [SerializeField] TurnManager turnManager;
    [SerializeField] Settlement settlement;

    [SerializeField] ResourceManager resourceManager;
    [SerializeField] ResourceBuilder resourceBuilder;
    ResourceManagerVisuals resourceManagerVisuals;
    [SerializeField] VisualManager visualManager;

    [SerializeField] BattleManager battleManager;
    [SerializeField] BattleVisuals battleVisuals;

    [SerializeField] Inventory playerInventory;
    PlayerMovement playerMovement;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] Death playerDeath;
    PlayerLevelManager playerLevelManager;
    Attributes playerAttributes;

    public System.Action OnPlayerDeath;

    

    // tile locations
    GameObject settlementGO;
    List<GameObject> villageTileSprites = new List<GameObject>();
    List<EnemyCampSpawner> enemyCampSpawners = new List<EnemyCampSpawner>();
    int[] enemyCampCooldowns = new int[3];
    List<VillageResources> villageResources = new List<VillageResources>();

    private void Awake()
    {
        //if (instance == null) instance = this;
        save = GetComponent<Save>();

        gridHex = gridBuildingSystem.GetGrid();
        cameraMovement = mainCamera.GetComponent<CameraMovement>();

        resourceBuilder = resourceManager.GetComponent<ResourceBuilder>();
        resourceManagerVisuals = resourceManager.GetComponent<ResourceManagerVisuals>();

        playerMovement = playerInventory.GetComponent<PlayerMovement>();
        playerLevelManager = playerInventory.GetComponent<PlayerLevelManager>();
        playerAttributes = playerInventory.GetComponent<Attributes>();

        villageResources = gridBuildingSystem.GetVillageResources();

        CanOpenMenu = false;
        menuOption = MenuOption.Main;
        visualManager.SetCanUseVisual(true);
        CanUseScripts(false);
    }

    private void Update()
    {
        if(CanOpenMenu && Input.GetKeyDown(KeyCode.Escape) && !InMenu)
        {
            OpenPauseMenu();
        }
        else if(InMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToGame();   
        }
    }

    public void CanUseScripts(bool option)
    {
        playerMovement.SetCanUseMovement(option);
        resourceManager.SetCanUse(option);
        resourceBuilder.SetCanActivateBuilder(option);
        playerInventory.SetCanUseInventory(option);
        cameraMovement.SetCanMove(option);
        if(battleManager.GetIsBattling() && !InMenu) battleManager.SetCanBattle(true);
        else battleManager.SetCanBattle(option);
    }
    public void ChooseScriptActivity(bool option)
    {
        visualManager.DeactivateAll();
        CanUseScripts(option);
    }

    // menus

    public void NewGame()
    {
        gridBuildingSystem.NewGame();
        OnMapAwake?.Invoke();

        mainMenuCanvas.SetActive(false);
        playCanvas.SetActive(true);

        MainMenu.SetActive(false);
        PauseMenu.SetActive(true);

        resourceManager.gameObject.SetActive(true);


        menuOption = MenuOption.Main;
        CanOpenMenu = true;
        InMainMenu = false;
        visualManager.SetCanUseVisual(true);
        CanUseScripts(true);
    }
    public void ContinueGame()
    {
        save.LoadAll();

        gridBuildingSystem.LoadGame();
        OnMapAwake?.Invoke();

        mainMenuCanvas.SetActive(false);
        playCanvas.SetActive(true);

        MainMenu.SetActive(false);
        PauseMenu.SetActive(true);

        resourceManager.gameObject.SetActive(true);

        menuOption = MenuOption.Main;
        InMainMenu = false;
        //CanOpenMenu = true;
        visualManager.SetCanUseVisual(true);
        CanUseScripts(true);

    }
    public void GameOver()
    {
        mainMenuCanvas.SetActive(true);
        playCanvas.SetActive(false);

        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(true);

        CanOpenMenu = false;
        InMenu = true;
        visualManager.SetCanUseVisual(false);
        CanUseScripts(true);
    }
    public void OpenPauseMenu()
    {
        background.SetActive(true);
        mainMenuCanvas.SetActive(true);
        playCanvas.SetActive(false);

        menuOption = MenuOption.Pause;
        CanOpenMenu = false;
        InMenu = true;
        visualManager.SetCanUseVisual(false);
        CanUseScripts(false);
    }
    public void ReturnToGame()
    {
        mainMenuCanvas.SetActive(false);
        playCanvas.SetActive(true);
        background.SetActive(false);

        MainMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(true);

        CanOpenMenu = true;
        InMenu = false;
        visualManager.SetCanUseVisual(true);
        CanUseScripts(true);

        if (battleManager.GetIsBattling()) battleVisuals.ActivateBattleUI(true);
    }
    public void ReturnToPreviousMenu() 
    {
        if(menuOption == MenuOption.Main)
        {
            MainMenu.SetActive(true);
        }
        else
        {
            PauseMenu.SetActive(true);
        }
        SettingsMenu.SetActive(false);

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenSettings()
    {
        MainMenu.SetActive(false);
        PauseMenu.SetActive(false);

        SettingsMenu.SetActive(true);
    }

    public void SetCanOpenMenu(bool option)
    {
        if (option == true)
        {
            StartCoroutine(DelayCanOpenMenu());
        }
        else CanOpenMenu = option;
    }
     
    IEnumerator DelayCanOpenMenu()
    { 
        yield return new WaitForSeconds(.1f);
        CanOpenMenu = true;
    }

    // tile functions

    public Settlement GetSettlementScript()
    {
        return settlement;
    }
    public GameObject GetSettlement()
    {
        return settlementGO;
    }
    public Vector2Int GetSettlementGridPosition()
    {
        int x; int y;
        gridHex.GetXY(settlementGO.transform.position, out x, out y);
        return new Vector2Int(x, y);
    }
    public void SetSettlementTile(GameObject settlement)
    {
        settlementGO = settlement;
    }
    public List<GameObject> GetVillageSpriteList()
    {
        return villageTileSprites;
    }
    public List<Vector2Int> GetVillageGridPositions()
    {
        List<Vector2Int> l = new List<Vector2Int>();

        foreach(GameObject villageGO in villageTileSprites)
        {
            int x; int y;
            gridHex.GetXY(villageGO.transform.position, out x, out y);
            l.Add(new Vector2Int(x, y));
        }

        return l;
    }
    public void AddVillage(GameObject village)
    {
        villageTileSprites.Add(village);
        teleport.AddVillageTile(village);

        gridHex.GetXY(village.transform.position, out int x, out int y);
        enemyManager.AddVillagePosition(new Vector2Int(x, y));

        villageResources.Add(village.GetComponent<VillageResources>());
    }

    // get scripts
    public TurnManager GetTurnManager()
    {
        return turnManager;
    }
    public Teleport GetTeleport()
    {
        return teleport;
    }
    public Tutorial GetTutorial()
    {
        return tutorial;
    }
    public FogOfWar GetFOW()
    {
        return FOW;
    }
    public ResourceBuilder GetResourceBuilder()
    {
        return resourceBuilder;
    }
    public ResourceManagerVisuals GetRMV()
    {
        return resourceManagerVisuals;
    }
    public ResourceManager GetResourceManager()
    {
        return resourceManager;
    }
    public VisualManager GetVisualManager()
    {
        return visualManager;
    }
    public BattleManager GetBattleManager()
    {
        return battleManager;
    }
    public BattleVisuals GetBattleVisuals()
    {
        return battleVisuals;
    }

    public PlayerLevelManager GetPlayerLevelManager()
    {
        return playerLevelManager;
    }
    public PlayerMovement GetPlayerMovement()
    {
        return playerMovement;
    }
    public Inventory GetInventory()
    {
        return playerInventory;
    }
    public Death GetDeath()
    {
        return playerDeath;
    }
    public EnemyManager GetEnemyManager()
    {
        return enemyManager;
    }
    public Attributes GetPlayerAttributes()
    {
        return playerAttributes;
    }
    public GridBuildingSystem GetGridBuildingSystem()
    {
        return gridBuildingSystem;
    }
    public GridHexXZ<GridBuildingSystem.GridObject> GetGrid()
    {
        return gridHex;
    }
    public GridHexXZ<PathNode> GetPathNodeGrid()
    {
        return gridBuildingSystem.GetPathNodeGrid();
    }
    public Camera GetCamera()
    {
        return mainCamera;
    }
    public List<EnemyCampSpawner> GetEnemyCamps()
    {
        return enemyCampSpawners;
    }
    
    public bool GetIsInMainMenu()
    {
        return InMainMenu;
    }

    //setters

    public void AddEnemyCamp(GameObject enemyChunk)
    {
        enemyCampSpawners.Add(enemyChunk.GetComponent<EnemyCampSpawner>());
    }
    public void SetMoveCamera(bool option)
    {
        cameraMovement.SetCanMove(option);
    }
    public void OnPlayerDeathAction()
    {
        OnPlayerDeath?.Invoke();    
    }

    enum MenuOption
    {
        Main,
        Pause,
        GameOver
    }
}
