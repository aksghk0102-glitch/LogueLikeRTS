using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// 인벤토리 내의 스킬 슬롯 개별 UI

public class SkillSlotUI : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI countText;
    public SkillSlotVisual visual;
    string curSkillID;

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

    // 아마 호출할 일이 없을 거 같다만 혹시 몰라 추가
    public void ClearSlot()
    {
        curSkillID = null;
        visual.Init();
        countText.text = "";
    }

    // 마우스를 올렸을 때 호출 (정보창 업데이트)
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    // 마우스가 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
