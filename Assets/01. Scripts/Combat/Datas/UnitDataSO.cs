using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitDataSO : ScriptableObject
{
    public UnitStats stats;
    public UnitClassType unitClass;

    [Header("Sound Keys")]
    public string hitSfxKey = "Armor 1-4";       // 피격 사운드 *기본값은 스켈레톤 용임
    public string attSfxKey = "";       // 공격 사운드
    public string dieSfxKey = "";       // 사망 사운드
}
