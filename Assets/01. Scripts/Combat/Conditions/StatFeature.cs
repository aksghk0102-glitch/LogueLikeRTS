using System.Collections.Generic;

public class StatFeature : IConditionFeature
{
    struct StatData { public StatType type; public float value; }
    readonly List<StatData> cached = new List<StatData>();

    public StatFeature (Dictionary<string, float> paramDic)
    {
        foreach (var pair in paramDic)
        {
            StatType t = KeyConverter(pair.Key);
            if (t != StatType.None)
                cached.Add(new StatData { type = t, value = pair.Value });
        }
    }

    StatType KeyConverter(string key)
    {
        switch (key)
        {
            case "hp": return StatType.MaxHp;
            case "att": return StatType.Attack;
            case "def": return StatType.Defence;
            case "mAtt": return StatType.mAttack;
            case "res": return StatType.mResist;
            case "attSpeed": return StatType.AttSpeed;
            case "attRange": return StatType.AttRange;
            case "moveSpeed": return StatType.MoveSpeed;
            case "sight": return StatType.Sight;
            case "mGen": return StatType.ManaRegen;
            case "mGet": return StatType.ManaGet;
            case "critChance": return StatType.CritChance;
            case "critDmg": return StatType.CritDamage;
            case "lifeSteal": return StatType.LifeSteal;
            case "ten": return StatType.Tenacity;

            default: return StatType.None;
        }
    }


    public bool CheckAction(ActionType action)
    {
        return true;
    }

    public void OnBattleEvent(Entity owner, ref DamageInfo dmg)
    {
    }

    public void OnCalculateStats(Entity owner, ref UnitStats stats, int stackCount)
    {
        for (int i = 0; i < cached.Count; i++)
        {
            var data = cached[i];
            float finalVal = data.value;

            switch (data.type)
            {
                // ÇÕ¿¬
                case StatType.Attack: stats.attack += finalVal; break;
                case StatType.mAttack: stats.mAttack += finalVal; break;
                case StatType.Defence: stats.defense += finalVal; break;
                case StatType.mResist: stats.magicResist += finalVal; break;
                
                case StatType.AttRange: stats.attRange += finalVal; break;
                case StatType.Sight: stats.sight += finalVal; break;
                
                case StatType.ManaGet: stats.manaGet += finalVal; break;
                case StatType.ManaRegen: stats.manaRegen += finalVal; break;
                case StatType.CritChance: stats.critChance += finalVal; break;
                case StatType.CritDamage: stats.critDamage += finalVal; break;
                case StatType.LifeSteal: stats.lifeSteal += finalVal; break;
                
                // °ö¿¬
                case StatType.AttSpeed: stats.attSpeedRate += finalVal; break;
                case StatType.MoveSpeed: stats.moveSpeedRate += finalVal; break;
                case StatType.Tenacity: stats.tenacityRate += finalVal; break;
            }
        }
    }

    public void OnTick(Entity owner, int stackCount, float deltaTime)
    {
    }
}
