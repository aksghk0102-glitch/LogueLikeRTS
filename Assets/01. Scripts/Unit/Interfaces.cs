using UnityEngine;
// 유닛, 건물 등 체력이 있어 피해를 받을 수 있는 오브젝트에 상속시킬 인터페이스

public interface ITargetable
{
    int CurHp { get; }              // 현재 체력
    int MaxHp { get; }              // 최대 체력
    float Radius { get; }           // 피격 거리 계산에 필요한 반지름 값
    float GetRadius() { return 0.5f; }  // 반지금 구하는 함수 설정
    void TakeDamage(int dmg) { }    // 실체 체력 감소 함수
    bool IsDie { get; }             // 사망 여부 체크
    Transform Transform { get; }    // 위치 값
    UnitFaction Faction { get; }    // 소속
}

public interface IAttackable
{
    int AttackPower { get; }        // 공격력
    float AttackSpeed { get; }      // 공격 속도
    float AttackRange { get; }      // 공격 범위
    float Sight { get; }            // 시야
    UnitFaction Faction { get; }    // 소속
}