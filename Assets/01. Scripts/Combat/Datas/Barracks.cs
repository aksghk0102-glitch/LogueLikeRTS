using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

//[RequireComponent(typeof(LineRenderer))]
public class Barracks : Building
{
    [Header("Barracks Settings")]
    [SerializeField] UnitClassType unitType;
    public UnitClassType UnitType => unitType;
    [SerializeField] Transform spawnPoint;      // 첫 스폰 위치

    [Header("State")]
    [SerializeField] int curLevel = 1;
    [SerializeField] Vector3 rallyPoint;        // 스폰 후 이동할 목표 지점
    [SerializeField] float lineYOffset = 0.5f;

    public int CurLevel => curLevel;
    public Vector3 RallyPoint => rallyPoint;

    LineRenderer line;

    public int Cost => 5;

    protected override void Awake()
    {
        base.Awake();

        //line = GetComponent<LineRenderer>();

        // 초기 랠리 포인트 설정 (Y축 보정 포함)
        Vector3 defaultPos = spawnPoint.position + transform.forward * 2f;
        defaultPos.y += lineYOffset;
        rallyPoint = defaultPos;

        //line.enabled = false;
        UpdateRallyLine();
    }

    public void Start()
    {
        ObjectManager.Inst.RegistObject(this);
    }

    public override void Init(UnitFaction a_Faction)
    {
        base.Init(a_Faction);
    }



    // 라운드 시작 시 유닛 1기 생산
    public void SpawnUnit()
    {
        if (UnitFactory.inst == null)
            return;

        Entity spawnUnit = UnitFactory.inst.CreateUnit(unitType, spawnPoint.position, Faction); ;

        if (ObjectManager.Inst != null)
            ObjectManager.Inst.RegistObject(spawnUnit);

        // 랠리 포인트 지정해주기
    }

    public void Upgrade()
    {
        int upgradeCost = 5;

        // 팝업을 띄우는 걸로 바꾸면 좋을 듯 

        if (GameManager.inst.SpendCost(upgradeCost))
        {
            curLevel++;
            // 레벨업에 따른 스탯 상승 로직 추가
        }
    }

    // 판매
    public void Sell()
    {
        GameManager.inst.AddCost(2);
        OnDie();
    }

    // 랠리 포인트 시각화 업데이트
    public void UpdateRallyLine()
    {
        if (line == null) return;

        // 라인의 시작점은 스폰 위치, 끝점은 랠리 포인트
        line.positionCount = 2;

        // 시작점과 끝점 모두 설정된 Offset만큼 Y축을 띄움
        Vector3 startPos = spawnPoint.position;
        startPos.y += lineYOffset;

        Vector3 endPos = rallyPoint;
        endPos.y += lineYOffset;

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }

    public void SetRallyPoint(Vector3 newPoint)
    {
        rallyPoint = newPoint;
        // 비주얼 라인 업데이트
        if(line.enabled)
            UpdateRallyLine();
    }

    public void RallyOn()
    {
        //line.enabled = true;
        UpdateRallyLine();
    }

    public void RallyOff()
    {
        //line.enabled = false;
    }

    public void OnClickBarrack()
    {
        // 배럭 클릭 시 UI 호출
    }
}
