using UnityEngine;

//[RequireComponent(typeof(LineRenderer))]
public class Barracks : Building
{
    [Header("Barracks Settings")]
    [SerializeField] UnitClassType unitType;
    public UnitClassType UnitType => unitType;
    [SerializeField] Transform spawnPoint;      // 첫 스폰 위치

    [Header("State")]
    [SerializeField] int curLevel = 1;

    public int UniqID { get; private set; }

    public int CurLevel => curLevel;
    public int Cost => 5;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        if (ObjectManager.Inst != null)
            UniqID = ObjectManager.Inst.RegistBarracks(this);
    }

    public override void Init(UnitFaction a_Faction)
    {
        faction = a_Faction;
        UniqID = ObjectManager.Inst.RegistBarracks(this);
    }

    // 라운드 시작 시 유닛 1기 생산
    public void SpawnUnit()
    {
        if (UnitFactory.inst == null)
            return;

        Entity spawnUnit = UnitFactory.inst
            .CreateUnit(unitType, spawnPoint.position, Faction,
            curLevel, UniqID);

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

    public void OnClickBarrack()
    {
        // 배럭 클릭 시 UI 호출
    }
}
