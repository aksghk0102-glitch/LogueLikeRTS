using UnityEngine;

public class UnitState_Chase : UnitBaseState
{
    public override void OnEnter(UnitCtrl unit)
    {
        // 플래그 초기화
        unit.isMoving = true;
        unit.isAttacking = false;
    }

    public override void OnUpdate(UnitCtrl unit)
    {
        // 추적 중에도 최단거리 타겟 탐색
        unit.SearcForTarget();
        
        // 추적 도중 타겟이 없거나 비활성화(풀링)되거나, 사망 상태면... 
        if (!unit.IsValidTarget())
        {
            // 새 타겟도 없으면 상태 변경 후 리턴
            unit.ChangeState(UnitState.MoveAndSearch);
            return;
        }
        //Debug.Log(unit.GetTargetDistace() + " / " + unit.AttackRange);
        // 공격 사거리 내에 도달했다면 공격 상태로 전환

        if (unit.GetTargetDistace() <= unit.AttackRange)
        {
            unit.StopNav();
            unit.ChangeState(UnitState.Attack);
            return;
        }

        // 도달하지 않았다면 대상을 향해 이동
        //Vector3 unitPos = unit.transform.position;
        //Vector3 targetPos = unit.curTarget.Transform.position;
        //Vector3 dir = (targetPos - unitPos).normalized;
        //
        //unit.SetMovement(dir);
        //unit.transform.position += dir * unit.MoveSpeed * Time.deltaTime;
        //unit.LastMoveDir = dir;

        Vector3 targetPos = unit.curTarget.Transform.position;
        unit.MoveTo(targetPos);
    }

    public override void OnExit(UnitCtrl unit)
    {
        // 플래그 초기화
        unit.isMoving = false;
    }
}
