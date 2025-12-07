using UnityEngine;

public class UnitState_Die : UnitBaseState
{
    float destroyTime = 1f;
    float curTimer = 0f;

    public override void OnEnter(UnitCtrl unit)
    {
        // 플래그 초기화
        unit.isMoving = false;
        unit.isAttacking = false;
    }

    public override void OnUpdate(UnitCtrl unit)
    {
        curTimer += Time.deltaTime;

        if (curTimer > destroyTime)
            unit.Die();
    }

    public override void OnExit(UnitCtrl unit)
    {

    }
}
