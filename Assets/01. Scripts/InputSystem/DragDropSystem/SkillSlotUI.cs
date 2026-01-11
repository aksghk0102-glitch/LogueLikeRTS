using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// 인벤토리 내의 스킬 슬롯 개별 UI

public class SkillSlotUI : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler
    , IBeginDragHandler, IDragHandler, IEndDragHandler
    , ITooltipHandler
{
    public TextMeshProUGUI countText;
    public SkillSlotVisual visual;
    string curSkillID;

    public int SlotIndex { get; set; }

    public void SetSlot(string id, int count)
    {
        curSkillID = id;
        SkillData data = InventoryManager.inst.GetSkillData(id);

        visual.SetVisual(data);


        // 카운트 갯수 표기 방식 고민 중... 1개만 있을 때는 굳이 안띄워도 될 거 같은데
        if (count <= 1)
        {
            countText.text = "";
        }
        else
        {
            countText.text = count.ToString();
        }
    }

    public void ClearSlot()
    {
        curSkillID = null;
        visual.Init();
        countText.text = "";
    }

    // 마우스를 올렸을 때 호출 (정보창 업데이트)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 스킬 설명 툴팁 출력
        UIStateManager.inst.tooltipUI.RequestShow(GetTooltipData());
    }

    // 마우스가 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 스킬 설명 툴팁 닫기
        UIStateManager.inst.tooltipUI.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log(curSkillID);
        if (string.IsNullOrEmpty(curSkillID))
            return;

        DragManager.inst.SetGhost(curSkillID);

        visual.SetAlpha();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(curSkillID))
            return;

        // 고스트의 위치를 마우스 위치로 갱신
        DragManager.inst.UpdateGhostPos(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 최종 드롭 처리 요청
        DragManager.inst.ExcuteDrop(eventData, SlotIndex);

        if (!string.IsNullOrEmpty(curSkillID))
        {
            // 색상 복구
            SkillData data = InventoryManager.inst.GetSkillData(curSkillID);
            visual.SetVisual(data);
        }
        else
            visual.Init();
    }
 
    // ITooltipHandler
    public TooltipData GetTooltipData()
    {
        if (string.IsNullOrEmpty(curSkillID))
            return null;

        SkillData data = InventoryManager.inst.GetSkillData(curSkillID);
        if (data == null)
            return null;

        return new TooltipData
        {
            name = data.Name,
            desc = data.Desc,
            targetClass = data.TargetClass.ToString(),
            ParamDic = data.ParamsDic
        };
    }
}
