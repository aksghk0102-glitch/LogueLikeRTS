using UnityEngine;
using System.Collections.Generic;

//
// 전장의 모든 유닛과 건물을 관리합니다.
// 

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Inst { get; private set; }

    Dictionary<UnitFaction, List<IDamageable>> allObjects = new Dictionary<UnitFaction, List<IDamageable>>();

    private void Awake()
    {
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);

        // 진영 초기화 : Faction을 더 늘리더라도 대응 가능하도록
        foreach (UnitFaction f in System.Enum.GetValues(typeof(UnitFaction)))
            allObjects[f] = new List<IDamageable>();
    }

    public void RegistObject(IDamageable unit)
    {
        if (!allObjects[unit.Faction].Contains(unit))
            allObjects[unit.Faction].Add(unit);
    }
    public void UnregistObject(Entity unit)
    {
        if (allObjects[unit.Faction].Contains(unit))
            allObjects[unit.Faction].Remove(unit);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // 오브젝트 중 실시간 업데이트가 필요한 경우 찾아서 갱신
        foreach(var factions in allObjects.Values)
        {
            for (int i = factions.Count - 1; i >= 0; i--)
            {
                var obj = factions[i];
                if (obj is Entity unit && unit.IsAlive)
                    unit.OnUpdate(deltaTime);

            }
        }
    }

    // 유닛의 시야 탐색 등... 상대 진영의 유닛 리스트를 반환하는 함수
    public List<IDamageable> GetEnemyList(UnitFaction myFaction)
    {
        foreach (var pair in allObjects)
        {
            if (pair.Key != myFaction)
                return pair.Value;
        }
        return null;
    }
}
