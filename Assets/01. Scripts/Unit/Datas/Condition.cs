using UnityEngine;
using System;
using System.Collections.Generic;

// 비트 플래그로 태그 설정 : 성능 저하 방지
[Flags]
public enum CDT_Tag
{
    None = 0,
    
    // 대분류
    Buff = 1 << 0,          // 이로운 효과
    Debuff = 1<< 1,         // 해로운 효과
    
    // 군중 제거
    CC = 1 << 3,            // 군중 제어기 판정용
    Stun = 1 << 4,          // 기절
    Root= 1 << 5,           // 속박
    Silence = 1 << 6,       // 침묵

    // 상태이상
    StatusEffect = 1<< 7,   // 상태이상 판정용 상위 태그
    Bleed = 1 << 8,         // 출혈
    Poison = 1 << 9,        // 중독
    Frost = 1 << 10,        // 동상
    Armorbreak = 1 << 11,   // 파열(방어력 감소)
    Slow = 1 << 12,         // 둔화
    LowHeal = 1<<13,        // 치유력 감소

    // 그 외
    Aura = 1 << 14,         // 오라 효과
    Invincivle = 1 << 15,   // 무적
    Revive = 1<<16,         // 부활
    Unstoppable = 1 << 17,  // 저지 불가
}

// 중첩 여부 판별
public enum CDT_StackType
{
    Refresh,        // 갱신 (기존 동일 컨디션을 대체)
    Additive,       // 추가 (중첩 허용)
    Unique          // 고유 (기존 효과 유지)
}

// 컨디션 적용 지점 설정
public interface IConditionFeature
{
    // 능력치 적용 시 : 능력 버프/디버프
    void OnCalculateStats(Entity owner, ref UnitStats stats, int stackCount);
    // 실시간 적용 : 중독 출혈
    void OnTick(Entity owner, int stackCount, float deltaTime);
    // 전투 판정 개입 : 흡혈, 데미지 변환, 온힛 등
    void OnBattleEvent(Entity owner, ref DamageInfo dmg);
    // 조건 체크
    bool CheckAction(ActionType action);
}

public class Condition
{
    public string ID;
    public CDT_Tag Tags;
    public CDT_StackType StackType;
    public float Duration;
    public int StackCount = 1;

    // 이 컨디션에 포함된 기능 리스트
    public List<IConditionFeature> Features = new List<IConditionFeature>();
    public bool HasTag(CDT_Tag tag) => (Tags & tag) != 0;
    public void Tick(Entity owner, float deltaTime)
    {
        Duration -= deltaTime;
        foreach (var f in Features)
            f.OnTick(owner, StackCount, deltaTime);
    }
}

// 실제 컨디션 관리 모듈
public class ConditionHandler
{
    Entity owner;
    List<Condition> actives = new List<Condition>();
    CDT_Tag immunityTags = CDT_Tag.None;        // 면역

    public List<Condition> ActiveCDTs => actives;
    public ConditionHandler(Entity a_Owner) => this.owner = a_Owner;

    public void AddCondition(Condition newCDT)
    {
        // 면역 체크
        if ((newCDT.Tags & immunityTags) != 0)
            return;

        // 중첩 여부 판별
        var ex = actives.Find(c=>c.ID == newCDT.ID);
        if(ex != null)
        {
            switch (newCDT.StackType)
            {
                case CDT_StackType.Refresh:
                    ex.Duration = newCDT.Duration;
                    return;

                case CDT_StackType.Unique:
                    return;

                case CDT_StackType.Additive:
                    actives.Add(newCDT);
                    return;
            }
        }
        else
        {
            // 추가
            actives.Add(newCDT);
        }

        owner.MarkDirty();
    }

    public void RemoveCondition(Condition cdt)
    {
        if(actives.Remove(cdt))
            owner.MarkDirty();
    }

    public void OnUpdate(float deltaTime)
    {
        for (int i = actives.Count - 1; i >= 0; i--)
        {
            actives[i].Tick(owner, deltaTime);

            if(actives[i].Duration <= 0)
            {
                actives.RemoveAt(i);
                owner.MarkDirty();
            }
        }
    }

    public bool HasTag(CDT_Tag tag)
    {
        foreach (var cdt in actives)
        {
            if (cdt.HasTag(tag))
                return true;
        }
        return false;
    }
}