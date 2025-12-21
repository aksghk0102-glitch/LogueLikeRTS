using UnityEngine;
using System.Collections.Generic;

//
// 전장의 모든 유닛을 관리합니다.
// 

public class UnitManager : MonoBehaviour
{
    public static UnitManager Inst { get; private set; }

    Dictionary<UnitFaction, List<Entity>> allUnits = new Dictionary<UnitFaction, List<Entity>>();

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);

        // 진영 초기화 : Faction을 더 늘리더라도 대응 가능하도록
        foreach (UnitFaction f in System.Enum.GetValues(typeof(UnitFaction)))
        {
            allUnits[f] = new List<Entity>();
        }
    }

    public void RegistUnit(Entity unit)
    {
        if (!allUnits[unit.Faction].Contains(unit))
            allUnits[unit.Faction].Add(unit);
    }
    public void UnregistUnit(Entity unit)
    {
        if (allUnits[unit.Faction].Contains(unit))
            allUnits[unit.Faction].Remove(unit);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // 모든 유닛을 순회하며 업데이트 갱신
        foreach(var factions in allUnits.Values)
        {
            for (int i = factions.Count - 1; i >= 0; i--)
            {
                var unit = factions[i];
                if (unit != null && unit.IsAlive)
                    unit.OnUpdate(deltaTime);
            }
        }
    }

    // 유닛의 시야 탐색 등... 상대 진영의 유닛 리스트를 반환하는 함수
    public List<Entity> GetEnemyList(UnitFaction myFaction)
    {
        foreach (var pair in allUnits)
        {
            if (pair.Key != myFaction)
                return pair.Value;
        }
        return null;
    }
}
