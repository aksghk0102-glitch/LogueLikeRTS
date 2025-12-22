using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitDataSO : ScriptableObject
{
    public UnitStats stats;
    public UnitClassType unitClass;
}
