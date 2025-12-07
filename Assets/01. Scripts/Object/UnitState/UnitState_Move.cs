using UnityEngine;
using UnityEngine.AI;

public class UnitState_Move : UnitBaseState
{
    public override void OnEnter(UnitCtrl unit)
    {
        // 플래그 초기화
        unit.MoveTo(unit.transform.position);
    }

    public override void OnUpdate(UnitCtrl unit)
    {
        // 타겟 탐색 후 전투 상태로 변경
        if (unit.SearcForTarget())
        {
            unit.ChangeState(UnitState.Attack);
            return;
        }

        // 기본 이동 로직... 목표지 할당 방식은 고민해봐야 할 듯.
        //  맵을 일자형으로 할지, 라인 개념을 넣을지 등 기획에 따라 변경 해야 함.

        // 임시 방향 설정
        //if(unit.Faction == UnitFaction.Player)
        //{
        //    Vector3 testD1 = new Vector3 (1, 0, 0);
        //    unit.SetMovement(testD1.normalized);
        //}
        //else if(unit.Faction == UnitFaction.Enemy)
        //{
        //    Vector3 testD2 = new Vector3(-1, 0, 0);
        //    unit.SetMovement(testD2.normalized);
        //}
        //
        //Vector3 a_MoveDir = unit.MoveDir;
        //if(a_MoveDir.sqrMagnitude > 0.001f)
        //{
        //    unit.transform.position += a_MoveDir * unit.MoveSpeed * Time.deltaTime;
        //    unit.transform.forward = a_MoveDir;
        //}

        if (unit.CheckNavPath)
        {
            Transform targetPos = null;
            if (Combat_Mgr.inst != null)
            {
                targetPos = unit.Faction == UnitFaction.Player ?
                    Combat_Mgr.inst.enemyNexus : Combat_Mgr.inst.playerNexus;
            }

            if (targetPos == null)
                return;
            
            Vector3 dir = (targetPos.position - unit.transform.position).normalized;
            Vector3 nextPos = unit.transform.position + dir * 10f;

            NavMeshHit hit;
            if(NavMesh.SamplePosition(nextPos, out hit, 10f, NavMesh.AllAreas))
            {
                unit.MoveTo(hit.position);
            }

        }
    }



    public override void OnExit(UnitCtrl unit)
    {
        // 플래그 초기화
        unit.isMoving = false;
    }
}
