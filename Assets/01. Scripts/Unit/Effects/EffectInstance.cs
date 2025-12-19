using UnityEngine;

// 런타임의 실제 구현부

public class EffectInstance
{
    public EffectData data;
    public UnitRuntime source;

    public float remainTime;
    public int stackCount;

    public EffectInstance(EffectData eData, UnitRuntime unit)
    {
        data = eData;
        source= unit;

        remainTime = data.duration;
        stackCount = 1;
    }

    public void Tick(float deltaTime)
    {
        if (data.duration > 0f)
            remainTime -= deltaTime;
    }

    public bool IsExpired => data.duration > 0f && remainTime <= 0f;
    public void ApplyStatModifier(StatType type, ref float add, ref float mul)
    {
        foreach(var mod in data.statModifiers)
        {
            if (mod.statType != type)
                continue;

            add += mod.add * stackCount;
            mul += mod.multiply * stackCount;
        }
    }
}
