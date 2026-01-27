using System.Collections.Generic;
using UnityEngine;

public class StatFeature : IConditionFeature
{

    public bool CheckAction(ActionType action)
    {
        throw new System.NotImplementedException();
    }

    public void OnBattleEvent(Entity owner, ref DamageInfo dmg)
    {
    }

    public void OnCalculateStats(Entity owner, ref UnitStats stats, int stackCount)
    {
    }

    public void OnTick(Entity owner, int stackCount, float deltaTime)
    {
    }
}
