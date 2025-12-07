using UnityEngine;

public class UnitState_Attack : UnitBaseState
{
    float attTimer = float.MaxValue;
    public override void OnEnter(UnitCtrl unit)
    {
        // 플래그 초기화
        unit.isAttacking = true;
        unit.isMoving = false;
        attTimer = float.MaxValue;

        // 애니메이션이 추가되면 여기서 초기화
    }

    public override void OnUpdate(UnitCtrl unit)
    {
        // 유효성 검사 : 타겟이 없거나 비활성화(풀링)되거나, 사망 상태면... 
        if(!unit.IsValidTarget())
        {
            // 새 타겟 지정
            if (unit.SearcForTarget())
            {
                unit.ChangeState(UnitState.Chase);
                return;
            }

            // 새 타겟도 없으면 상태 변경 후 리턴
            unit.ChangeState(UnitState.MoveAndSearch);
            return;
        }

        // 공격 사거리보다 적이 멀어지면 다시 추적
        if (unit.GetTargetDistace() > unit.AttackRange)
        {
            unit.ChangeState(UnitState.Chase);
            return;
        }

        // 공격 실행
        attTimer += Time.deltaTime;
        if(attTimer >= unit.AttackSpeed)
        {
            // 공격 쿨타임 초기화
            attTimer = 0f;

            // 데미지 구현 => 전투매니저에서 처리
            //unit.curTarget.TakeDamage(unit.AttackPower); // 기존코드 주석처리
            DamageBuffer buffer = new DamageBuffer(unit as IAttackable, unit.curTarget);
            

            Combat_Mgr.inst.EnqueueDamage(buffer);
        }
    }

    public override void OnExit(UnitCtrl unit)
    {
        unit.isAttacking = false;
    }
}
