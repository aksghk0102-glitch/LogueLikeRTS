using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 클래스 인포 판넬에 붙여서 쓸 컴포넌트

public class ClassInfoUI : MonoBehaviour
{
    public static ClassInfoUI inst;     // 후에 UI 총괄 매니저에서 일괄 처리하기

    [Header("Current Target Class")]
    public UnitClassType curType;       // 현재 정보 출력 중인 클래스

    [Header("Reference")]
    public TextMeshProUGUI classNameText;
    public SkillSlotVisual activeSlotImg;             // 액티브 스킬 슬롯
    public SkillSlotVisual[] passiveSlotImg;          // 패시브 스킬 슬롯

    private void Awake()
    {
        if (inst == null) inst = this;
        gameObject.SetActive(false); // 기본적으로는 꺼둠
    }

    public void Open(UnitClassType type)
    {
        curType = type;
        classNameText.text = type.ToString();
        gameObject.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        // 장비 상태 관리자에서 정보 받아오기
        UnitSkillSet data = EquipManager.inst.GetUnitSkillSet(curType);

        // 스킬 표시
        activeSlotImg.SetVisual(InventoryManager.inst.GetSkillData(data.ActiveID));

        for (int i = 0; i < passiveSlotImg.Length; i++)
            passiveSlotImg[i].SetVisual(InventoryManager.inst.GetSkillData(data.PassiveID[i]));
    }


    // OnClickActiveSlot과 OnClickPassiveSlot은
    // 하이어라키 상에서 버튼 이벤트로 연결되어 있음
    public void OnClickActiveSlot()
    {
        EquipManager.inst.UnequipActive(curType);
        Debug.Log("OnClickActiveSlot");
    }

    public void OnClickPassiveSlot(int idx)
    {
        EquipManager.inst.UnequipPassive(curType, idx);
        Debug.Log("OnClickPassiveSlot");
    }
}
