using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragManager : MonoBehaviour
{
    public static DragManager inst;

    // 미리 보기 오브젝트 관리
    [Header("Ghost")]
    public SkillSlotVisual ghostVisual;
    public RectTransform ghostRect;

    // 현재 드래그 중인 스킬의 ID 저장
    public string curSkillId { get; private set; }

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        ghostVisual.gameObject.SetActive(false);
        ghostVisual.Init();
    }

    public void UpdateGhostPos(Vector2 mousePos)
    {
        ghostRect.position = mousePos;
    }

    public void ExcuteDrop(PointerEventData eventData, int index)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool isDropped = false;

        foreach(var result in results)
        {
            Debug.Log(result.gameObject.name);

            // 클래스 슬롯에 드랍하는 경우
            ClassSlotUI targetClassSlot = result.gameObject
                .GetComponentInParent<ClassSlotUI>();
            if(targetClassSlot != null)
            {
                // 사운드 호출
                SoundManager.inst.PlaySFX("Interface 6-4");

                SkillData data = InventoryManager.inst.GetSkillData(curSkillId);
                //Debug.Log(data.Name);
                EquipManager.inst.EquipSkill(targetClassSlot.classType, data);
                isDropped = true;
                break;
            }

            // 클래스 정보 판넬에 직접 드랍
            ClassInfoUI targetInfoUI = result.gameObject
                .GetComponentInParent<ClassInfoUI>();
            if (targetInfoUI != null)
            {
                // 사운드 호출
                SoundManager.inst.PlaySFX("Interface 6-4");

                SkillData data = InventoryManager.inst.GetSkillData(curSkillId);
                //Debug.Log(data.Name);
                EquipManager.inst.EquipSkill(targetInfoUI.curType, data);
                isDropped = true;
                break;
            }

            // 인벤토리 슬롯에 드랍하는 경우
            SkillSlotUI targetIvSlot = result.gameObject
                .GetComponentInParent<SkillSlotUI>();
            if(targetIvSlot != null && targetIvSlot.SlotIndex != index)
            {
                // 사운드 호출
                SoundManager.inst.PlaySFX("Interface 6-4");

                //Debug.Log("스킬 슬롯에 드롭됨");
                InventoryManager.inst.SwapSlot(index, targetIvSlot.SlotIndex);
                isDropped = true;
                break;
            }
        }

        if (!isDropped)
        {
            // 사운드 호출
            SoundManager.inst.PlaySFX("Interface 6-5");
        }

        // 드랍이 끝나면 고스트 숨기기
        HideGhost();

        // 인벤토리 갱신
        InventoryUI.inst.UpdateInventory();
    }

    public void SetGhost(string skillID)
    {
        //Debug.Log("SetGhost");
        curSkillId = skillID;

        SkillData data = InventoryManager.inst.GetSkillData(curSkillId);
        if(data != null)
        {
            ghostVisual.SetVisual(data);
            ghostVisual.SetAlpha();
            ghostVisual.gameObject.SetActive(true);
        }
    }

    public void HideGhost()
    {
        curSkillId = null;
        ghostVisual.gameObject.SetActive(false);
    }
}
