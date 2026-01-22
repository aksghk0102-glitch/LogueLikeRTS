using UnityEngine;
using System.Collections.Generic;

// 통계 보관용 클래스
public class StatRecord
{
    public int UniqID;

    // 가한 피해량
    public float Damage_Sum
        => Damage_Phys + Damage_Magic + Damage_True;  // 총 피해량
    public float Damage_Phys;  // 물리
    public float Damage_Magic; // 마법
    public float Damage_True;  // 고정피해량

    // 받은 피해량
    public float Received_Sum
        => Received_Phys+ Received_Magic+ Received_True+ Received_Protected;     // 표기를 위해 모든 Receive 변수 합
    public float Received_Phys;
    public float Received_Magic;
    public float Received_True;

    public float Received_Protected;        // 감소량 누적

    // 회복량
    public float TotalHealed;       // 총 회복량

    public StatRecord(int id)
    {
        UniqID = id;

        Damage_Phys = Received_Magic = Received_True = 0;
        Received_Phys = Received_Magic = Received_True = Received_Protected = 0;

        TotalHealed = 0;
    }
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Inst { get; private set; }

    // 데미지 처리용 큐
    Queue<DamageInfo> damageQueue = new Queue<DamageInfo>();
    
    // 데미지 통계 저장
    Dictionary<int, StatRecord> combatStats = new Dictionary<int, StatRecord>();

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        ProcessQueue();
    }

    void ProcessQueue()
    {
        while (damageQueue.Count > 0)
        {
            DamageInfo dmg = damageQueue.Dequeue();

            // 유효성 검사
            if (dmg.Target == null || !dmg.Target.IsAlive)
                continue;

            IAttacker attacker = dmg.Attker;
            IDamageable target = dmg.Target;

            // 공격자 컨디션 개입
            if (attacker != null && attacker as Entity)
            {
                Entity entity = attacker as Entity;
                entity.AttackerCDT(ref dmg);
            }

            // 피격자 데미지 판정
            target.TakeDamage(dmg);

            // 온힛 처리
            if (dmg.Source == DamageSource.Default &&
                attacker != null)
                attacker.OnHit(target, dmg);
        }
    }

    public void EnqueueDamage(DamageInfo dmg)
    {
        damageQueue.Enqueue(dmg);
    }

    #region Stats

    // 라운드 시작 시 호출하여 통계 정보 초기화
    public void ResetCombatStats()
    {
        combatStats.Clear();
    }

    // 통계 조회
    public StatRecord GetStatsRecord(int id)
    {
        if(combatStats.TryGetValue(id, out StatRecord record))
            return record;
        return null;
    }

    // 통계 요청 로직
    public void RecordDealt(int id, float amount, DamageType type)
    {
        StatRecord record = GetRecord(id);

        if (type == DamageType.Physics)
            record.Damage_Phys += amount;
        else if (type == DamageType.Magic)
            record.Damage_Magic += amount;
        else if (type == DamageType.True)
            record.Damage_True += amount;
    }

    public void RecordRecieved(int id, float raw, float act, DamageType type)
    {
        StatRecord record = GetRecord(id);
        float p = raw - act;        // 감소량 protected

        record.Received_Protected += p;
        if (type == DamageType.Physics)
            record.Received_Phys += act;
        else if (type == DamageType.Magic)
            record.Received_Magic += act;
        else if (type == DamageType.True)
            record.Received_True += act;
    }
    public void RecordHeal(int id, float amount)
    {
        GetRecord(id).TotalHealed += amount;
    }

    StatRecord GetRecord(int id)
    {
        if (!combatStats.ContainsKey(id))
            combatStats.Add(id, new StatRecord(id));

        return combatStats[id];
    }

    public IEnumerable<StatRecord> GetAllStats()
    {
        return combatStats.Values;
    }

    #endregion
}
