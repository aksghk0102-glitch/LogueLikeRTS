using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Inst { get; private set; }

    Queue<DamageInfo> damageQueue = new Queue<DamageInfo>();

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
}
