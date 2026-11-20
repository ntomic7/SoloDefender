using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    [SerializeField] MAINmanager mainManager;
    [SerializeField] GameObject teleportButton;

    [SerializeField] List<GameObject> PlayUIobjects;
    [SerializeField] GameObject TeleportUI;

    CameraMovement cameraMovement;
    Animator teleportButtonAnimator;
    Camera _camera;

    FogOfWar fogOfWar;

    GridHexXZ<GridBuildingSystem.GridObject> grid;

    GameObject settlementGO;
    List<GameObject> villageTiles = new List<GameObject>();
    [SerializeField] List<GameObject> villageSprites;

    Dictionary<GameObject, bool> isBuildingAttacked;

    float originalSize;

    Vector2 screenPoint;
    Vector2 anchoredPosition;

    Vector3 oldCameraPosition;

    public System.Action<bool> SetIsOnMap;


    private void Start()
    {
        mainManager.OnMapAwake += OnMapAwake;

        teleportButtonAnimator = teleportButton.GetComponent<Animator>();
        cameraMovement = mainManager.GetCamera().GetComponent<CameraMovement>();

        fogOfWar = GetComponent<FogOfWar>();

        _camera = mainManager.GetCamera();
        grid = mainManager.GetGrid();

        originalSize = _camera.orthographicSize;

        
    }

    void OnMapAwake()
    {

        //while (villageTiles.Count <= 0)
        //{
        //    villageTiles = mainManager.GetVillageSpriteList();
        //    Debug.Log(villageTiles.Count);
        //}
        StartCoroutine(WaitForVillageTiles());
        //settlementGO = mainManager.GetSettlement();

        //Debug.Log(villageTiles.Count);

        //isBuildingAttacked = new Dictionary<GameObject, bool>()
        //{
        //    {villageTiles[0], false},
        //    {villageTiles[1], false},
        //    {villageTiles[2], false},
        //    {settlementGO,    false},
        //};

        //mainManager.OnMapAwake -= OnMapAwake;
    }
    IEnumerator WaitForVillageTiles()
    {
        //while (villageTiles.Count < 3)
        {
            villageTiles = mainManager.GetVillageSpriteList();
        }

        yield return null;

        settlementGO = mainManager.GetSettlement();

        Debug.Log(villageTiles.Count);
        isBuildingAttacked = new Dictionary<GameObject, bool>()
        {
            {villageTiles[0], false},
            {villageTiles[1], false},
            {villageTiles[2], false},
            {settlementGO,    false},
        };

        mainManager.OnMapAwake -= OnMapAwake;

    }

    public void OpenTeleportMap()
    {
        SetIsOnMap?.Invoke(true);

        mainManager.ChooseScriptActivity(false);

        foreach(GameObject go in PlayUIobjects) go.SetActive(false);
        TeleportUI.SetActive(true);

        oldCameraPosition = _camera.transform.position;
        _camera.transform.position = new Vector3(24.24f, 18.18f, -10f);
        _camera.orthographicSize = 19;

        for(int i = 0; i < 3; i++)
        {
            SetUpSpriteLocations(villageTiles[i], i);
        }
        SetUpSpriteLocations(settlementGO, 3);
    }
    public void CloseTeleportMap()
    {
        SetIsOnMap?.Invoke(false);

        foreach (GameObject go in PlayUIobjects) go.SetActive(true);
        TeleportUI.SetActive(false);

        //_camera.transform.position = oldCameraPosition;
        _camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);
        cameraMovement.CheckCameraConstraints();
        _camera.orthographicSize = originalSize;

        foreach(GameObject go in villageSprites)
        {
            go.SetActive(false);
        }
        StartCoroutine(WaitToOpen());
    }
    IEnumerator WaitToOpen()
    {
        yield return null;

        mainManager.ChooseScriptActivity(true);
    }

    public void ChooseTeleportOption(GameObject go)
    {
        for (int i = 0; i < 4; i++)
        {
            if (villageSprites[i].gameObject == go && i < 3)
            {
                this.transform.position = new Vector3(villageTiles[i].transform.position.x -1,
                    villageTiles[i].transform.position.y, villageTiles[i].transform.position.z);
            }
            else if (villageSprites[3].gameObject == go && i == 3)
            {
                this.transform.position = new Vector3(settlementGO.transform.position.x - 1,
                    settlementGO.transform.position.y, settlementGO.transform.position.z);
            }
        }
        fogOfWar.ClearCircle();
        CloseTeleportMap();
    }

    void SetUpSpriteLocations(GameObject go, int i)
    {

        screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, go.transform.position);
        anchoredPosition = transform.InverseTransformPoint(screenPoint);
        anchoredPosition = new Vector2(anchoredPosition.x + 23f, anchoredPosition.y + 18f);

        villageSprites[i].transform.position = go.transform.position;

        if (i < 3 && grid.GetGridObject(go.transform.position).GetTileObject().gameObject.GetComponent<VillageResources>().GetIsConnected())
        {
            villageSprites[i].gameObject.SetActive(true);
        }
        else if(i == 3) villageSprites[i].gameObject.SetActive(true);
        if (isBuildingAttacked[go]) { ShowWarning(i); }
        else villageSprites[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public List<GameObject> GetVillageTiles()
    {
        return villageTiles;
    }
    public void AddVillageTile(GameObject location)
    {
        villageTiles.Add(location);
    }
    public void SetSettlement(GameObject settlement)
    {
        settlementGO = settlement;
    }

    public Dictionary<GameObject, bool> GetIsAttacked()
    {
        return isBuildingAttacked;
    }
    public void SetIsAttacked(GameObject go, bool option)
    {
        if (isBuildingAttacked[go] && !option)
        {
            HideMapWarning();
            isBuildingAttacked[go] = option;
            return;
        }

        isBuildingAttacked[go] = option;

        if (option)
        {
            ShowMapWarning();
        }
    }
    
    void ShowWarning(int i)
    {
        ShowMapWarning();

        villageSprites[i].GetComponent<Animator>().SetTrigger("OnWarning");
        villageSprites[i].GetComponent<Image>().color = new Color32(255, 0, 0, 255);
    }
    void ShowMapWarning()
    {
        teleportButtonAnimator.SetTrigger("OnMapWarning");
        teleportButtonAnimator.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
    }
    void HideMapWarning()
    {
        teleportButtonAnimator.SetTrigger("OnSafe");
        teleportButtonAnimator.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public bool GetIsSettlementAttacked()
    {
        return isBuildingAttacked[settlementGO];
    }
}
