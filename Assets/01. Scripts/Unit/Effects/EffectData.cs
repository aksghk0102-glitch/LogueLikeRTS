using UnityEngine;

// 각종 효과, 버프/디버프/상태이상/상태변화 등 구조 정의

[CreateAssetMenu(fileName = "EffectData", menuName = "Game/EffectData")]
public class EffectData : ScriptableObject
{
    public string effectId;
    public EffectTag[] tags;

    public float duration;
    public bool stackable;

    public StatModifier[] statModifiers;
}

// 이펙트의 종류 구분 > 여러 태그 들어갈 수 있음
public enum EffectTag
{
    Buff,
    Debuff,
    
    Slow,
    
    Bleed,
    ArmorBreak,

    
}

public struct StatModifier
{
    public StatType statType;
    public float add;
    public float multiply;
}