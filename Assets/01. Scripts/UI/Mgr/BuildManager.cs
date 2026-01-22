using UnityEngine;
using System;

public class BuildManager : MonoBehaviour
{
    public static BuildManager inst;

    [Header("References")]
    [SerializeField] BuildGhost ghost;
    [SerializeField] LayerMask slotLayer;

    [Header("Building Prefabs")]
    [SerializeField] Building[] prefabs;

    Building selPrefab;
    BuildSlot curTargetSlot;
    bool isDrag { get; set; } = false;

    // 직관적인 조작을 위한 가상의 평면 생성
    Plane ground = new Plane(Vector3.up, Vector3.zero); // y=0 좌표에 가상 평면

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    public void StartBuild(UnitClassType type)
    {
        // 생성할 프리팹 매칭
        selPrefab = Array.Find(prefabs, prefab =>
        {   if (prefab is Barracks b)
                return b.UnitType == type;      // 올바른 배럭이면 true
            return false;
        });
    
        if(selPrefab != null)
        {
            isDrag = true;
            ghost.Show(type);        // 고스트에 유닛 타입 전달
        }
    }

    public void OnDrag(Vector2 screenPos)
    {
        if (!isDrag) return;

        // 드래그 중인 상태면 레이로 맵 탐색 
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        
        // 슬롯 레이어를 검사
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, slotLayer))
        {
            curTargetSlot = hit.collider.GetComponent<BuildSlot>();
            bool isValid = curTargetSlot != null
                && curTargetSlot.CanBuild(UnitFaction.Player);

            // 고스트를 건물이 지어질 위치에 표시
            ghost.UpdateGhost(curTargetSlot.GetPosition(), isValid);
        }
        else
        {
            curTargetSlot = null;

            if(ground.Raycast(ray, out float input))
            {
                Vector3 pos = ray.GetPoint(input);
                ghost.UpdateGhost(pos, false);
            }
        }
    }

    const string unvaildMsg = "그곳에는 지을 수 없습니다.";
    public void RequestBuild()
    {
        if (!isDrag)
            return;

        isDrag = false;     // 드래그 루프 종료

        if(curTargetSlot != null &&
            curTargetSlot.CanBuild(UnitFaction.Player))
        {
            ghost.UpdateGhost(curTargetSlot.GetPosition(), true);
            UIStateManager.inst
                .ShowPopUp("이 위치에\n건물을 짓겠습니까?\n(- 5코스트)",
                ()=> { ConfimBuild();},
                ()=> { CancleBuild(); }
                );
        }
        else
        {
            InfoMassage.inst.ShowMessage(unvaildMsg);
            CancleBuild();
        }
    }

    // 팝업 확인 버튼 연결
    public void ConfimBuild()
    {
        if (selPrefab == null || curTargetSlot == null)
            return;

        if (GameManager.inst.SpendCost(5))
        {   
            // 사운드 호출
            SoundManager.inst.PlaySFX("Nail Wood 1-1");
            
            Building build = Instantiate(selPrefab,
            curTargetSlot.GetPosition(), Quaternion.identity);
            curTargetSlot.SetBuilding(build);

            InitState();
        }
        else
        {
            CancleBuild();
        }

        if(GameManager.inst.CurCost == 0)
            InfoMassage.inst.ShowPerMessage("준비가 완료 되었으면 '전투시작' 버튼을 누르세요.");
    }

    // 건설 취소
    public void CancleBuild()
    {
        // 사운드 호출
        SoundManager.inst.PlaySFX("Interface 6-5");

        InitState();

        UIStateManager.inst.CanclePopUp();
    }

    void InitState()
    {
        isDrag = false;
        ghost.Hide();
        selPrefab = null;
        curTargetSlot = null;
    }
}
