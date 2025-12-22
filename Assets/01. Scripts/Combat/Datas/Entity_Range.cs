using UnityEngine;

public class Entity_Range : Entity
{
    [Header("RangeUnit Settings")]
    [SerializeField] protected Transform firPos;        // 발사 지점
    [SerializeField] protected float projSpeed;         // 투사체 속도

    public override void OnAttackEvent()
    {
        if (curTarget == null || !curTarget.IsAlive)
        {
            EndAttack();
            return;
        }

        DamageInfo dmg = CreateDamagaInfo();

        // 투사체에 대미지 정보 위임
        //if ()
        //{
        //
        //}
        //else
        //{
        //    // 매니저 부재 시 즉시 데미지 처리
        //    CombatManager.Inst.EnqueueDamage(dmg);
        //}

    }
}
