using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager inst { get; private set; }

    [Header("Camera Move")]
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float edgeSize = 20f; // 화면 끝 감지 픽셀 범위
    [SerializeField] Vector2 limitMin; // 카메라 이동 제한 (최소)
    [SerializeField] Vector2 limitMax; // 카메라 이동 제한 (최대)

    [Header("Mouse Click")]
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private BarrackUI barrackUI;

    Building targetBiliding;
    Camera mainCam;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        mainCam = Camera.main;
    }

    void Update()
    {
        HandleCameraMove();

        // UI 클릭 중일 때는 레이캐스트(선택) 무시
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }
    private void HandleCameraMove()
    {
        Vector3 pos = mainCam.transform.position;
        Vector3 mousePos = Input.mousePosition;

        // 화면 상단 끝
        if (mousePos.y >= Screen.height - edgeSize) pos.z += moveSpeed * Time.deltaTime;
        // 화면 하단 끝
        else if (mousePos.y <= edgeSize) pos.z -= moveSpeed * Time.deltaTime;

        // 화면 우측 끝
        if (mousePos.x >= Screen.width - edgeSize) pos.x += moveSpeed * Time.deltaTime;
        // 화면 좌측 끝
        else if (mousePos.x <= edgeSize) pos.x -= moveSpeed * Time.deltaTime;

        // 맵 범위 제한 적용
        pos.x = Mathf.Clamp(pos.x, limitMin.x, limitMax.x);
        pos.z = Mathf.Clamp(pos.z, limitMin.y, limitMax.y);

        mainCam.transform.position = pos;
    }

    private void HandleSelection()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingLayer))
        {
            Building building = hit.collider.GetComponentInParent<Building>();
            if (building != null)
            {
                SelectBuilding(building);
                return;
            }
        }

        Deselect();
    }

    private void SelectBuilding(Building building)
    {
        targetBiliding = building;
        if (targetBiliding is Barracks barracks)
            barrackUI.OpenUI(barracks);
        else
            barrackUI.CloseUI();
    }

    public void Deselect()
    {
        targetBiliding = null;
        barrackUI.CloseUI();
    }
}
