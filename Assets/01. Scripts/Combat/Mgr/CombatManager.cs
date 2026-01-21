using UnityEngine;
using System.Collections.Generic;

// 통계 보관용 클래스
public class StatRecord
{
    public int UniqID;
    public UnitClassType Type;
    public float TotalDamage;       // 총 피해량
    public float TotalReceivedRaw;     // 감소되지 않은 받은 피해량
    public float TotalReceivedAct;     // 총 받은 피해량 (차이로 감소한 피해량 계산)
    public float TotalHealed;       // 총 회복량

    public StatRecord(int id, UnitClassType type)
    {
        UniqID = id;
        Type = type;
        TotalDamage = 0;
        TotalReceivedRaw = 0;
        TotalReceivedAct = 0;
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



    public StatRecord GetRecord(int id)
    {
        if(combatStats.TryGetValue(id, out StatRecord record))
            return record;
        return null;
    }
}
