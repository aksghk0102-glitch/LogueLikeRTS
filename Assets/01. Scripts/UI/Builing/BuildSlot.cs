using UnityEngine;
using System;

// 건설 가능 블록에 설치

public class BuildSlot : MonoBehaviour
{
    // 건물이 들어서 있는지 체크
    [SerializeField]bool canBuild = true;       // 디버그용 직렬화
    public bool CanBuild => canBuild;
    Vector3 buildPos;

    private void Awake()
    {
        buildPos = transform.position + new Vector3(0, 1.5f, 0);
    }

    public void SetBuilding(Building building)
    {
        canBuild = false;

        // 건물 파괴 시점 이벤트 등록
        building.OnDestroy += InitSlot;
    }

    void InitSlot()
    {
        canBuild = true;
    }

    public Vector3 GetPosition()
    {
        return buildPos;
    }
}
