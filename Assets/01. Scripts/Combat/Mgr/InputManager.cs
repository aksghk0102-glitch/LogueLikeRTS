using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager inst { get; private set; }

    [Header("Mouse Click")]
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private BarrackUI barrackUI;

    Building targetBiliding;
    public bool IsDragging = false;
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
        // UI 클릭 중일 때는 레이캐스트(선택) 무시
        if (EventSystem.current.IsPointerOverGameObject()
            || IsDragging) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
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
        {
            UIStateManager.inst.ShowClassInfo(barracks.UnitType);

        }
        else
            UIStateManager.inst.CloseBarrackUI();
    }

    public void Deselect()
    {
        targetBiliding = null;
        UIStateManager.inst.CloseBarrackUI();
    }
}
