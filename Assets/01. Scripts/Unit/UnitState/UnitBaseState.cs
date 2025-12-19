using UnityEngine;

public class UnitBaseState : IUnitState
{
    // 해당 상태에 진입할 때 호출
    public virtual void OnEnter(UnitCtrl unit) {}

    // 해당 상태에서 빠져나올 때 호출
    public virtual void OnExit(UnitCtrl unit) {}

    // 해당 상태에서 매 프레임 호출
    public virtual void OnUpdate(UnitCtrl unit) {}
}
