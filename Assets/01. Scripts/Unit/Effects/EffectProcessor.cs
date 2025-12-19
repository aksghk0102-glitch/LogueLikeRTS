using UnityEngine;

public static class EffectProcessor
{
    public static void AddEffect(UnitRuntime target, EffectData data, UnitRuntime source)
    {
        if (!data.stackable)
        {
            foreach (var e in target.activeEffects)
            {
                if(e.data == data)
                {
                    e.remainTime = data.duration;
                    return;
                }
            }
        }

        target.activeEffects.Add(new EffectInstance(data, source));
    }

    public static void Update(UnitRuntime unit, float deltaTime)
    {
        for (int i = unit.activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = unit.activeEffects[i];
            effect.Tick(deltaTime);

            if(effect.IsExpired)
                unit.activeEffects.RemoveAt(i);
        }

    }

}
