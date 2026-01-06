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
            // 클래스 슬롯에 드랍하는 경우
            ClassSlotUI targetClassSlot = result.gameObject
                .GetComponentInParent<ClassSlotUI>();
            if(targetClassSlot != null)
            {
                SkillData data = InventoryManager.inst.GetSkillData(curSkillId);
                EquipManager.inst.EquipSkill(targetClassSlot.classType, data);
                isDropped = true;
                break;
            }

            // 인벤토리 슬롯에 드랍하는 경우
            SkillSlotUI targetIvSlot = result.gameObject
                .GetComponentInParent<SkillSlotUI>();
            if(targetIvSlot != null && targetIvSlot.SlotIndex != index)
            {
                InventoryManager.inst.SwapSlot(index, targetIvSlot.SlotIndex);
                isDropped = true;
                break;
            }
        }

        if (!isDropped)
        {
            Debug.Log("유효 하지 않은 위치에 드롭되었습니다.");
        }

        // 드랍이 끝나면 고스트 숨기기
        HideGhost();
    }

    public void SetGhost(string skillID)
    {
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
