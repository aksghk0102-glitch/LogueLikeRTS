using UnityEngine;
using System.Collections.Generic;

//
// 전장의 모든 유닛과 건물을 관리합니다.
// 

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Inst { get; private set; }

    // 전투 시 조회를 위한 딕셔너리
    Dictionary<UnitFaction, List<IDamageable>> allObjects
        = new Dictionary<UnitFaction, List<IDamageable>>();

    // 배럭 UI 조회 시 배럭 인스턴스를 참조하기 위한 딕셔너리
    Dictionary<UnitClassType, Barracks> allBarracks
        = new Dictionary<UnitClassType, Barracks>();

    [Header("HP Bar")]
    [SerializeField] GameObject hpbarPrefab;

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

    public void RegistObject(IDamageable obj)
    {
        if (!allObjects[obj.Faction].Contains(obj))
        {
            allObjects[obj.Faction].Add(obj);
            Debug.Log(obj.Faction +" " + obj);

            if (obj is MonoBehaviour m)
                CreateHpBar(m.gameObject);

            if (obj is Barracks b)
                allBarracks.Add(b.UnitType, b);
        }
    }
    public void UnregistObject(IDamageable obj)
    {
        if (allObjects[obj.Faction].Contains(obj))
        {
            allObjects[obj.Faction].Remove(obj);
            
            if(obj is Barracks b)
                allBarracks.Remove(b.UnitType);
        }
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

    public List<Barracks> GetAllBarracks()
    {
        List<Barracks> barracksList = new List<Barracks>();

        foreach (var factionList in allObjects.Values)
        {
            // 리스트 순회 중 변형 에러 방지를 위해 단순 for문 사용
            for (int i = 0; i < factionList.Count; i++)
            {
                if (factionList[i] is Barracks barracks)
                {
                    barracksList.Add(barracks);
                }
            }
        }
        return barracksList;
    }

    public Barracks GetBarracks(UnitClassType type)
    {
        if (allBarracks.TryGetValue(type, out var barracks))
            return barracks;

        return null;
    }

    void CreateHpBar(GameObject target)
    {
        if (hpbarPrefab == null) return;

        GameObject ui = Instantiate(hpbarPrefab);

        ObjHpBar bar = ui.GetComponent<ObjHpBar>();
        if (bar != null)
            bar.SetTarget(target);
    }
}
