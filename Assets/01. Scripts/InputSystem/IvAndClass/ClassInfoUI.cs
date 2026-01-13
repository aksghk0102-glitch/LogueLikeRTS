using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 클래스 인포 판넬에 붙여서 쓸 컴포넌트

public class ClassInfoUI : MonoBehaviour
{
    [Header("Current Target Class")]
    public UnitClassType curType = UnitClassType.Init;       // 현재 정보 출력 중인 클래스

    [Header("Reference")]
    public TextMeshProUGUI classNameText;
    public SkillSlotVisual activeSlotImg;             // 액티브 스킬 슬롯
    public SkillSlotVisual[] passiveSlotImg;          // 패시브 스킬 슬롯
    public Button exitBtn;

    private void Awake()
    {
        gameObject.SetActive(false); // 기본적으로는 꺼둠
        if (exitBtn != null)
            exitBtn.onClick.AddListener(Exit);
    }

    public void Open(UnitClassType type, UnitFaction faction = UnitFaction.Player)
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

        //Debug.Log($"[ClassInfoUI] Refreshing {curType}. Active: {data.ActiveID}, Passives: {string.Join(", ", data.PassiveID)}");

        // 액티브 스킬 표시 및 툴팁 갱신
        SkillData active = InventoryManager.inst.GetSkillData(data.ActiveID);
        activeSlotImg.SetVisual(active);
        if (activeSlotImg.TryGetComponent(out TooltipTrigger aTrigger))
            aTrigger.SetData(active);

        for (int i = 0; i < passiveSlotImg.Length; i++)
        {
            SkillData passive = InventoryManager.inst.GetSkillData(data.PassiveID[i]);
            passiveSlotImg[i].SetVisual(passive);

            if (passiveSlotImg[i].TryGetComponent(out TooltipTrigger pTrigger))
                pTrigger.SetData(passive);
        }
    }


    // OnClickActiveSlot과 OnClickPassiveSlot은
    // 하이어라키 상에서 버튼 이벤트로 연결되어 있음
    public void OnClickActiveSlot()
    {
        EquipManager.inst.UnequipActive(curType);
        Refresh();
    }

    public void OnClickPassiveSlot(int idx)
    {
        EquipManager.inst.UnequipPassive(curType, idx);
        Refresh();
    }

    // 닫기 버튼 클릭 시 호출
    public void Exit()
    {
        curType = UnitClassType.Init;
        gameObject.SetActive(false);
    }
}
