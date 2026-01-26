using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public static EquipManager inst;

    [Header("UI")]
    public ClassSlotUI[] classSlots;    // 화면 중앙 하단, 5개의 클래스별 슬롯

    Dictionary<UnitClassType, UnitSkillSet> unitSkillDatas
        = new Dictionary<UnitClassType, UnitSkillSet>();

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        InitSkillSets();
    }

    void InitSkillSets()
    {
        // 5종 클래스 별 빈 데이터 생성

        UnitClassType[] targetType =
        {
            UnitClassType.Babarian,
            UnitClassType.Knight,
            UnitClassType.Rogue,
            UnitClassType.Ranger,
            UnitClassType.Mage
        };

        foreach (var t in targetType)
        {
            unitSkillDatas.Add(t, new UnitSkillSet(t));
            RefreshSlot(t);
        }
    }

    // 스킬 장착 : 인벤토리 -> 클래스 별 슬롯 할당
    const string classErrorMsg = "해당 클래스는 착용할 수 없습니다.";
    const string equipErrorMsg = "더 착용할 수 없습니다. 스킬을 제거해주세요.";

    public void EquipSkill(UnitClassType targetType, SkillData skilldata)
    {
        // 방어 코드
        if (!unitSkillDatas.ContainsKey(targetType) || skilldata == null)
            return;

        // 타겟 클래스 검사
        if (!skilldata.TargetClassList_enum.Contains(targetType))
        {
            UIStateManager.inst.ShowClassInfo(targetType);

            // 안내 메세지 출력 : 해당 스킬은 장착할 수 없습니다.
            InfoMassage.inst.ShowMessage(classErrorMsg);
            return;
        }

        UnitSkillSet data = unitSkillDatas[targetType];

        // 스킬 슬롯 검사 및 할당 : 기본 원칙 - 비어있을 때만 장착 가능, 비우는 건 외부에서
        if (skilldata.SkillType == SkillType.Active)
        {
            // 액티브 스킬인 경우...
            if (string.IsNullOrEmpty(data.ActiveID))
            {
                data.ActiveID = skilldata.ID;
                InventoryManager.inst.AddSkill(skilldata.ID, - 1);
                Debug.Log($"{skilldata.ID} -> {targetType}에 장착 됨");
            }
            else
            {
                // 안내 메세지 출력 :
                // ex. 공간이 없습니다. 먼저 우측 클래스 정보 패널에서 스킬을 제거해주세요.
                InfoMassage.inst.ShowMessage(equipErrorMsg);
                return;
            }
        }
        else if (skilldata.SkillType == SkillType.Passive)
        {
            // 빈 슬롯 검사
            int emptyIndex = -1;
            for (int i = 0; i < data.PassiveID.Length; i++)
            {
                if (string.IsNullOrEmpty(data.PassiveID[i]))
                {
                    emptyIndex = i;
                    break;
                }
            }

            // 빈칸이 있을 때 할당
            if (emptyIndex != -1)
            {
                data.PassiveID[emptyIndex] = skilldata.ID;
                InventoryManager.inst.AddSkill(skilldata.ID, -1);

                Debug.Log($"{skilldata.ID} -> {targetType}에 장착 됨");
            }
            else
            {
                // 안내 메세지 출력 :
                InfoMassage.inst.ShowMessage(equipErrorMsg);
                return;
            }
        }

        RefreshSlot(targetType);
        UIStateManager.inst.ShowClassInfo(targetType);
    }

    // 탈착
    public void UnequipActive(UnitClassType targetType)
    {
        UnitSkillSet data = unitSkillDatas[targetType];
        if (string.IsNullOrEmpty(data.ActiveID))
            return;

        InventoryManager.inst.AddSkill(data.ActiveID, 1);
        data.ActiveID = "";
        RefreshSlot(targetType);
    }

    public void UnequipPassive(UnitClassType targetType, int index)
    {
        UnitSkillSet data = unitSkillDatas[targetType];
        if (index < 0 || index >= data.PassiveID.Length)
            return;
        if (string.IsNullOrEmpty(data.PassiveID[index]))
            return;

        InventoryManager.inst.AddSkill(data.PassiveID[index], 1);
        data.PassiveID[index] = "";
        RefreshSlot(targetType);
    }

    void RefreshSlot(UnitClassType type)
    {
        foreach (var slot in classSlots)
        {
            if(slot.classType == type)
            {
                slot.RefreshSlotUI(unitSkillDatas[type]);
                break;
            }
        }
    }


    public UnitSkillSet GetUnitSkillSet(UnitClassType type)
    {
        if(unitSkillDatas.ContainsKey(type))
            return unitSkillDatas[type];

        return null;
    }
}

