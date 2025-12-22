// 전투 관련 데이터 구조 모음
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

// 유닛 하나에 들어가는 능력치 구조체 묶음
public struct UnitStats
{
    public float maxHP; // 최대 체력
    public float attack; // 공격력
    public float defense; // 방어력 (데미지 산정 시 방어력 만큼 감소)
    public float magicResist; // 마법저항력

    public float moveSpeed; // 이동 속도
    public float attSpeed; // 공격 속도

    public float attRange; // 공격 사거리
    public float sight; // 시야 범위

    public float maxMana; // 최대 마나 = 스킬 발동에 필요한 마나
    public float startMana; // 시작 마나
    public float manaRegen; // 초당 마나 회복량
    public float manaGet; // 평타 공격 시 얻는 마나 회복량

    public float critChance; // 치명타 발동 확률 백분위 1 %
    public float critDamage; // 치명타 발동 시 피해량 증가폭

    public float lifeSteal;     // 생명력 흡수
    public float tenacity;      // 강인함
}

// 유닛의 행동 관리
public enum ActionType
{
    Move, Attack, Skill
}

// 데미지 수신자 : 유닛, 건물, 장애물 등
public interface IDamageable
{
    void TakeDamage(DamageInfo dmg);
    bool IsAlive { get; }
    float Radius { get; }
}
// 공격자 : 타격 성공 시점 호출 로직(온힛 효과, 마나 회복 등)
public interface IAttacker
{
    void OnHit(IDamageable target, DamageInfo info);    // 타격 성공 시 실행 함수
}

// 스탯 계산기 : 스킬 버프 등 최종 스탯 참조
public interface IStatCalc
{
    UnitStats GetFinalStats();
}
public struct DamageInfo
{
    public IAttacker Attker;    // 공격자
    public IDamageable Target;  // 피격자
    public float Damage;          // 데미지
    public bool IsCritical;     // 치명타 여부
    public DamageSource Source; // 데미지의 출처 확인
    public DamageType type;     // 물리 마법 고정 피해 분류

    // 특수한 정보 묶음을 전달할 데이터 딕셔러니
    public Dictionary<string, float> MetaData;
}

public enum DamageSource
{
    Default,        // 평타 등 기본값
    Skill,          // 스킬 피해
    Condition,      // 상태 이상 (출혈, 중독...)
    Environmet,     // 환경 요소
}
// 데미지의 유형을 정의
public enum DamageType
{
    Physics,        // 물리
    Magic,          // 마법
    True,           // 고정 데미지
    Heal,           // 힐
}
// 유닛이 스킬을 매끄럽게 사용할 수 있도록 상태 관리 및 제어
public interface ICastSkill
{
    bool isSkillCasting { get; }
    void StartSkillCast();      // 스킬 사용 시작 > 평타 금지 상태 진입
    void EndSkillCast();        // 스킬 사용 종료 > 평타 금지 상태 해제
}


// 액티브 스킬
// 유닛이 마나를 소모하면서 사용할 하나의 액티브 스킬.
public interface IActiveSkill
{
    string SkillName { get; }       // 스킬 이름
    SkillMotionType MotionInfo { get; } // 스킬 사용 시 호출될 모션 정보

    void UseSkill(IAttacker user, IDamageable target);
}

// 패시브 스킬
// 유닛에 여러 종류의 패시브 스킬을 셋팅 가능(아이템 개념)
public interface IPassiveSkill
{
    string SkillID{ get; }
    // 최종 스탯 계산 시 수치 보정
    void SetStats(ref UnitStats stats);
    // 타격 판정 시점에 특수 효과 적용
    void OnHit(IAttacker owner, IDamageable target, DamageInfo info);
}

public class StatHandler
{
    UnitStats baseStats;        // 패시브 스킬로 인한 효과가 적용된 수치
    UnitStats cacheStats;
    bool isDirty = true;        // 교체가 필요한 지 체크

    public void SetBase(UnitStats stats)
    {
        baseStats = stats;
        MarkDirty();
    }
    public void MarkDirty() => isDirty = true;

    public UnitStats GetFinalStats(List<Condition> actives, Entity owner)
    {
        if (isDirty)
        {
            cacheStats = baseStats;
            foreach (var cdt in actives)
                foreach (var f in cdt.Features)
                    f.OnCalculateStats(owner, ref cacheStats, cdt.StackCount);
            isDirty = false;
        }

        return cacheStats;
    }
}