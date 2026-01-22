using UnityEngine;
using UnityEngine.EventSystems;
//
// 범용 툴팁 기능을 부여.
// 인스펙터에서도 셋팅할 수 있고, 외부 스크립트에서도 수정 가능
//

public class TooltipsUI : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string title; // 툴팁에 표시될 내용
    [TextArea]
    public string content; // 툴팁에 표시될 내용

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 올라가면 매니저에게 내용 전달 및 표시 요청
        UIStateManager.inst.ShowTooltip(title, content);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 나가면 숨기기 요청
        UIStateManager.inst.HideTooltip();
    }
}
