using UnityEngine;
using TMPro;

public class UnitInfoUI : MonoBehaviour
{
    [Header("UI Ref")]
    public TextMeshProUGUI tmp_Hp;
    public TextMeshProUGUI tmp_Att;
    public TextMeshProUGUI tmp_Def;
    public TextMeshProUGUI tmp_Res;

    public TextMeshProUGUI tmp_MoveSpeed;
    public TextMeshProUGUI tmp_AttSpeed;
    public TextMeshProUGUI tmp_AttRange;
    public TextMeshProUGUI tmp_Sight;

    public TextMeshProUGUI tmp_MaxMana;
    public TextMeshProUGUI tmp_StartMana;
    public TextMeshProUGUI tmp_ManaGen;
    public TextMeshProUGUI tmp_ManaGet;
    
    public TextMeshProUGUI tmp_CritChance;
    public TextMeshProUGUI tmp_CritDmg;
    public TextMeshProUGUI tmp_LifeSteal;
    public TextMeshProUGUI tmp_Tenacity;

    StatHandler vStatHandler = new StatHandler();

    public void Open(UnitClassType type, int level)
    {
        // 원본 스탯 참조
        var unitData = UnitFactory.inst.GetOriginUnitStat(type);

        // 가상의 스탯 핸들러로 시뮬레이션0
        vStatHandler.SetBase(unitData, level);

        // 배럭 레벨 반영
        UnitStats staticStats = vStatHandler.GetStaticStats();

        // 아이템 능력치 반영은 추후 추가

        tmp_Hp.text = staticStats.maxHP.ToString("F0");
        tmp_Att.text = staticStats.attack.ToString("F0");
        tmp_Def.text = staticStats.defense.ToString("F0");
        tmp_Res.text = staticStats.magicResist.ToString("F0");

        tmp_MoveSpeed.text = staticStats.moveSpeed.ToString("F1");
        tmp_AttSpeed.text = staticStats.attSpeed.ToString("F1");
        tmp_AttRange.text = staticStats.attRange.ToString("F1");
        tmp_Sight.text = staticStats.sight.ToString("F1");

        tmp_MaxMana.text = staticStats.maxMana.ToString("F0");
        tmp_StartMana.text = staticStats.startMana.ToString("F0");
        tmp_ManaGen.text = staticStats.manaRegen.ToString("F1");
        tmp_ManaGet.text = staticStats.manaGet.ToString("F0");

        tmp_CritChance.text = $"{staticStats.critChance}%";
        tmp_CritDmg.text = $"{staticStats.critDamage * 100f}%";
        tmp_LifeSteal.text = $"{staticStats.lifeSteal}%";
        tmp_Tenacity.text = $"{staticStats.tenacity}%";

        gameObject.SetActive(true);
    }
}
