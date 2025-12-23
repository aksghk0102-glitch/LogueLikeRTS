using UnityEngine;
using System.Collections.Generic;

// 유닛의 원본 데이터와 프리팹을 매칭합니다.

public class UnitFactory : MonoBehaviour
{
    public static UnitFactory inst { get; private set; }

    [System.Serializable]
    public struct UnitPrefabData
    {
        public UnitClassType classType;
        public Entity prefab;
        public UnitDataSO dataSO;
    }

    [Header("Unit Datas")]
    [SerializeField] List<UnitPrefabData> unitDatas;    // 인스펙터에서 할당 
                                                        // 딕셔너리로 전환해 무결성 보장

    Dictionary<UnitClassType, UnitPrefabData> unitDict
        = new Dictionary<UnitClassType, UnitPrefabData>();

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        foreach (var data in unitDatas)
            if (!unitDict.ContainsKey(data.classType))
                unitDict.Add(data.classType, data);
    }

    public Entity CreateUnit(UnitClassType type, Vector3 position,
        UnitFaction faction)
    {
        if (!unitDict.TryGetValue(type, out UnitPrefabData data))
            return null;

        // 유닛 생성
        Entity newUnit = Instantiate(data.prefab, position, Quaternion.identity);

        // 유닛 초기화
        newUnit.InitEntity(data.dataSO.stats, faction);

        return newUnit;
    }
}
