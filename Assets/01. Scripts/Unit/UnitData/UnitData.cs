using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Base Infomations")]
    public string unitId = 0;
    public UnitType unitType;

    [Header("Core Stats")]
    public UnitStats baseStats;

    public AIProfile aiProf;
    public int skillSlotCount = 2;
}

[System.Serializable]
public enum UnitType
{
    // 5종 유닛을 명시적으로 구분하기 위한 변수
    Babarian,
    Knight,
    Rogue,
    Ranger,
    Mage
}

[System.Serializable]
public struct UnitStats
{
    public int maxHP;
    public int attack;
    public int defense;

    public float moveSpeed;
    public float attSpeed;

    public float attRange;
    public float sight;

    public int maxMana;
    public int startMana;
    public int manaRegen;
}