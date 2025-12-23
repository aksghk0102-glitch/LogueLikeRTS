using UnityEngine;
using System;

public class BuildManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BuildGhost ghost;
    [SerializeField] LayerMask slotLayer;

    [Header("Building Prefabs")]
    [SerializeField] Building[] prefabs;

    Building selPrefab;
    BuildSlot curTargetSlot;
    bool isDrag = false;

    // 직관적인 조작을 위한 가상의 평면 생성
    Plane ground = new Plane(Vector3.up, Vector3.zero); // y=0 좌표에 가상 평면

    public void StartBuild(UnitClassType type)
    {
        // 고스트에 유닛 타입 전달
        ghost.Show(type);

        // 생성할 프리팹 매칭
        selPrefab = Array.Find(prefabs, prefab =>
        {   if (prefab is Barracks b)
                return b.UnitType == type;      // 올바른 배럭이면 true
            return false;
        });
    
        if(selPrefab != null)
            isDrag = true;
    }

    private void Update()
    {
        if (!isDrag) return;

        // 드래그 중인 상태면 레이로 맵 탐색 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // 슬롯 레이어를 검사
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, slotLayer))
        {
            curTargetSlot = hit.collider.GetComponent<BuildSlot>();
            bool isValid = curTargetSlot != null && curTargetSlot.CanBuild;

            // 고스트를 건물이 지어질 위치에 표시
            ghost.UpdateGhost(curTargetSlot.GetPosition(), isValid);

            // 마우스를 떼면 건설
            if (Input.GetMouseButtonUp(0))
            {
                if (isValid)
                    Build();
                else
                    Cancle();
            }
        }
        else
        {
            curTargetSlot = null;

            if(ground.Raycast(ray, out float input))
            {
                Vector3 pos = ray.GetPoint(input);
                ghost.UpdateGhost(pos, false);
            }
            if (Input.GetMouseButtonUp(0))
                Cancle();
        }
    }

    void Build()
    {
        Building b = Instantiate(selPrefab,
            curTargetSlot.GetPosition(), Quaternion.identity);

        // 슬롯에게 건물 정보 넘기기
        curTargetSlot.SetBuilding(b);
        Cancle();
    }

    void Cancle()
    {
        isDrag = false;
        ghost.Hide();
        selPrefab = null;
        curTargetSlot = null;
    }
}
