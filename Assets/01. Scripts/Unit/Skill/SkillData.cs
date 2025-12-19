using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Game/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillId;

    public float coolDown;
    public float range;

    public EffectData[] applyEffects;
    public SkillTargetType targetType;
}

public enum SkillTargetType
{
    Self,
    Enemy,
    Ally,
    Area,
}
