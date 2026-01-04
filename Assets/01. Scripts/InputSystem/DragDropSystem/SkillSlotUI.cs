using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// 스킬 슬롯 개별 UI

public class SkillSlotUI : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI CountText;
    public Image IconImage;
    public Image IconOutline;
    SkillData data;

    public void SetSlot(SkillData a_data, int count)
    {
        data = a_data;
        NameText.text = data.Name;
        CountText.text = $"×{count}";
        
        // 아이콘 할당 추가

    }

    // 스킬 설명을 키 값으로 수정
    string GetDynamicDesc()
    {
        string desc = data.Desc;
        foreach (var p in data.Params)
        {
            //if()
            desc = desc.Replace("{"+p.Key+"}", p.Value.ToString());
        }
        return desc;
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
