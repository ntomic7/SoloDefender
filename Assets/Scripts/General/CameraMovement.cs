
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Camera _camera;

   

    [SerializeField] MAINmanager mainManager;
    BattleManager battleManager;
    [SerializeField] Transform playerTransform;

    Teleport teleport;

    Vector3 settlementPosition;

    bool canMove = true;
    bool canZoom = false;

    float minZoom = 7.6f;
    float maxZoom = 19f;

    int edgeScrollSize = 20;
    float movementSpeed = 20f;

    [SerializeField] float mapMinX = 7.65f, mapMaxX = 40.65f, mapMinY = 3.88f, mapMaxY = 31.73f;

    private void Start()
    {
        mainManager.OnMapAwake += OnMapAwake;
        battleManager = mainManager.GetBattleManager();
    }

    void Update()
    {
        if (!battleManager.GetIsBattling() && (canMove || canZoom))
        {
            MoveCamera();
        }

        if (canZoom) Zoom();


    }

    void MoveCamera()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.y = -1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.y = +1f;

        Vector3 cameraPos = transform.position;
        Vector3 final = cameraPos += inputDir * movementSpeed * Time.deltaTime;

        if (final.x < mapMinX || final.x > mapMaxX) return; 
        if (final.y < mapMinY || final.y > mapMaxY) return; 

        transform.position = final;
    }

    void Zoom()
    {
        if(Input.mouseScrollDelta.y > 0)
        {
            if((_camera.orthographicSize - 1) > minZoom) _camera.orthographicSize -= 1f;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            if((_camera.orthographicSize) + 1 < maxZoom) _camera.orthographicSize += 1f;
        }
    }
    Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = _camera.orthographicSize;
        float camWidth = _camera.orthographicSize * _camera.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;
    
        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.y);
    }
    void SetCanZoom(bool option)
    {
        canZoom = option;
    }

    public void CheckCameraConstraints()
    {
        Vector3 camPos = transform.position;
        float xDiff = camPos.x;
        float yDiff = camPos.y; 

        if(camPos.x < mapMinX) xDiff = mapMinX + .1f;
        else if(camPos.x > mapMaxX) xDiff = mapMaxX -.1f;

        if (camPos.y < mapMinY) yDiff = mapMinY + .1f;
        else if (camPos.y > mapMaxY) yDiff = mapMaxY - .1f;

        transform.position = new Vector3(xDiff, yDiff, -10f);
    }
 

    void OnMapAwake()
    {
        settlementPosition = mainManager.GetSettlement().transform.position;
        teleport = mainManager.GetTeleport();

        _camera = GetComponent<Camera>();

        teleport.SetIsOnMap += SetCanZoom;
        mainManager.OnMapAwake -= OnMapAwake;
    }

    public void OnFindPlayerButtonPressed()
    {
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
        CheckCameraConstraints();
    }
    public void OnFindettlementButtonPressed()
    {
        transform.position = new Vector3(settlementPosition.x, settlementPosition.y, -10f);
    }
    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
