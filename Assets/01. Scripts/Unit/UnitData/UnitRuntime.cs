using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// 유닛의 전투 중 상태를 정의합니다.

public class UnitRuntime : MonoBehaviour
{
    public UnitStats baseStats;

    [Header("Runtime Stats")]
    public int curHP;
    public int curMP;

    [Header("State")]
    public UnitRuntime curTarget;

    [Header("Effects")]
    public List<EffectInstance> activeEffects = new List<EffectInstance>();

    public bool IsAlive => curHP > 0;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        curHP = baseStats.maxHP;
        curMP = baseStats.startMana;
    }

    public void GainMana(int amount)
    {

    }
}

public enum StatType
{
    MaxHP,
    Attack,
    Defense,

    MoveSpeed,
    AttackSpeed,

    AttackRange,
    Sight,

    ManaRegen,
}

public class StatResolver
{
    public static float Get(UnitRuntime unit, StatType type)
    {
        float baseValue = GetBaseValue(unit, type);

        float add = 0f;
        float mul = 1f;

        foreach (var effect in unit.activeEffects)
            effect.ApplyStatModifier(type, ref add, ref mul);

        return (baseValue + add) * mul;
    }

    static float GetBaseValue(UnitRuntime unit, StatType type)
    {
        UnitStats us = unit.data.baseStats;

        return type switch
        {
            StatType.MaxHP => us.maxHP,
            StatType.Attack => us.attack,
            StatType.Defense => us.defense,
            StatType.MoveSpeed => us.moveSpeed,
            StatType.AttackSpeed => us.attSpeed,
            StatType.AttackRange => us.attRange,
            StatType.Sight => us.sight,
            StatType.ManaRegen => us.manaRegen,
            _ => 0f
        };
    }
}
