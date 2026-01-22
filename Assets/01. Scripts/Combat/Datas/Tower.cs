using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Tower : Building, IAttacker
{
    [Header("Status")]
    [SerializeField] float dmg = 20f;
    [SerializeField] float attRange = 8f;
    [SerializeField] float attSpeed = 1.0f;

    [Header("Settings")]
    [SerializeField] BulletKey bulletKey;
    [SerializeField] Vector3 firePos;
    [SerializeField] GameObject showAttRange;

    float attTimer = 0f;

    IDamageable curTarget;
    //public int ID { get; private set; }

    void Start()
    {
        ObjectManager.Inst.RegistObject(this);
    }

    void Update()
    {
        // 사거리 내의 적을 탐지 후 공격
        if (curTarget != null && !curTarget.IsAlive)
        {
            curTarget = null;
        }

        // 타겟 유효성 검사 및 재탐색
        if (curTarget == null)
            SearchTarget();

        // 타겟 지정 후 공격
        if (curTarget != null)
            Attack();


        // 공격 속도 적용
        if (attTimer >= 0f)
            attTimer -= Time.deltaTime;
    }

    void SearchTarget()
    {
        curTarget = null;

        // 적 진영 리스트 받아오기
        var enemies = ObjectManager.Inst.GetEnemyList(this.Faction);
        if (enemies == null || enemies.Count == 0)
            return;

        IDamageable closest = null;
        float minDist = attRange;
        Vector3 myPos = transform.position;

        // 모든 적 유닛 탐색 후 시야 내에 있는지 확인
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;

            float dist2Centor = Vector3.Distance(myPos, enemy.WorldPosition);
            float dist2Surface = dist2Centor - enemy.Radius;
            if (dist2Surface <= attRange && dist2Surface < minDist)
            {
                minDist = dist2Surface;
                closest = enemy;
            }
        }

        // 타겟 할당
        curTarget = closest;
    }
    void Attack()
    {
        if (curTarget == null || !curTarget.IsAlive)
            return;

        if (attTimer > 0f)
            return;

        attTimer = attSpeed;

        DamageInfo dmgInfo = new DamageInfo
        {
            Attker = this,
            Target = curTarget,
            Damage = dmg,
            Source = DamageSource.Default,
            type = DamageType.Magic,
            MetaData = new Dictionary<string, float>()
        };

        // 투사체에 대미지 정보 위임
        if (BulletManager.inst != null)
        {
            Vector3 spawnPos = transform.position + firePos;
            BulletManager.inst.SpawnBullet(bulletKey, spawnPos,
                curTarget, dmgInfo);
        }
        else
        {
            // 매니저 부재 시 즉시 데미지 처리
            CombatManager.Inst.EnqueueDamage(dmgInfo);
        }
    }

    public void OnHit(IDamageable target, DamageInfo info)
    {

    }

}
