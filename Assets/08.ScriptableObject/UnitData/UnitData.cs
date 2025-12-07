using UnityEngine;

[System.Serializable]
public enum UnitType
{
    // 일단은 최소 기능만 구현. 
    Melee,
    Range,
}

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [HideInInspector]public string objPoolPath = "";

    [Header("Base Infomations")]
    public string unitName = "";
    public int unitUniqIndex = 0;
    public UnitType unitType;

    [Header("Core Stats")]
    public int hp;
    public int cost;
    public int attackPower;

    [Header("Stats")]
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed;
    public float sight;
}
