using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// 스킬 슬롯 개별 UI

public class SkillSlotUI : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI CountText;
    public Image IconImage;
    public Image IconFrame;
    SkillData data;

    public void SetSlot(SkillData a_data, int count)
    {
        data = a_data;
        CountText.text = $"×{count}";
        
        // 아이콘 할당 추가
        //IconImage.sprite = ico
    }

    // 마우스를 올렸을 때 호출 (정보창 업데이트)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (data == null) return;

        // 고정 정보창(UIManager/TooltipUI)에 데이터 전달
        // TooltipUI.Instance.Show(data);
    }

    // 마우스가 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // TooltipUI.Instance.Hide();
    }
}
