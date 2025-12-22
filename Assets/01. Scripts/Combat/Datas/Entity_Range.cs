using UnityEngine;

public class Entity_Range : Entity
{
    [Header("RangeUnit Settings")]
    [SerializeField] protected BulletKey bulletKey;     // 인스펙터에서 설정

    // 발사 위치 고정값
    private static readonly Vector3 FIRE_POSITION = new Vector3(0f, 2f, 0.4f);

    public override void OnAttackEvent()
    {
        if (curTarget == null || !curTarget.IsAlive)
        {
            EndAttack();
            return;
        }

        DamageInfo dmg = CreateDamagaInfo();

        // 투사체에 대미지 정보 위임
        if (BulletManager.inst != null)
        {
            Vector3 spawnPos = FIRE_POSITION;
            BulletManager.inst.SpawnBullet(bulletKey, spawnPos,
                curTarget, dmg);
        }
        else
        {
            // 매니저 부재 시 즉시 데미지 처리
            CombatManager.Inst.EnqueueDamage(dmg);
        }

        if (!curTarget.IsAlive)
            EndAttack();
    }
}
