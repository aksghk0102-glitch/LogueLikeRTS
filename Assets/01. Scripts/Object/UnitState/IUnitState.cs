public interface IUnitState
{
    void OnEnter(UnitCtrl unit);
    void OnUpdate(UnitCtrl unit);
    void OnExit(UnitCtrl unit);
}
