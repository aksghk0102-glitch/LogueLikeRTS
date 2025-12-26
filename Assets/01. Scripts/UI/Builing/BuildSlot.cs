using UnityEngine;
using System;

// 건설 가능 블록에 설치

public class BuildSlot : MonoBehaviour
{
    // 건물이 들어서 있는지 체크
    [SerializeField]bool isBuild = false;       // 건물이 지어져 있는지
    [SerializeField] UnitFaction faction = UnitFaction.Neutral; // 누구의 소속 땅인지
    Vector3 buildPos;

    private void Awake()
    {
        isBuild = false;
        buildPos = transform.position + new Vector3(0, 1.5f, 0);
    }

    public bool CanBuild(UnitFaction a_Faction)
    {
        return !isBuild && faction == a_Faction;
    }

    public void SetBuilding(Building building)
    {
        isBuild = true;

        // 건물 파괴 시점 이벤트 등록
        building.OnDestroy += InitSlot;
    }

    // 빌드 슬롯을 소유하고 있는 마인에서 호출
    public void UpdateFaction(UnitFaction a_Faction)
    {
        // 소속 갱신
        faction = a_Faction;
    }

    void InitSlot()
    {
        isBuild = false;
    }

    public Vector3 GetPosition()
    {
        return buildPos;
    }
}
