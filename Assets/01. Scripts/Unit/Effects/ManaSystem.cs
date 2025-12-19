using UnityEngine;

public static class ManaSystem
{
    public static void Regen(UnitRuntime unit, float deltaTime)
    {
        int regen = Mathf.RoundToInt(StatResolver.Get(unit, StatType.ManaRegen) * deltaTime);

        unit.curMP = Mathf.Clamp(unit.curMP + regen,
            0, unit.data.baseStats.maxMana);
    }
}
