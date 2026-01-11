using UnityEngine;
using UnityEngine.EventSystems;

// 마우스 호버 시 스킬 툴팁을 출력하게 하는 트리거
// 클래스 정보 판넬, 스테이지 클리어 보상 획득 시 정보 확인 등에 활용

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipHandler
{
    SkillData cachedData;
    
    public void SetData(SkillData data)
    {
        cachedData = data;
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
    public TooltipData GetTooltipData()
    {
        if (cachedData == null)
            return null;

        return new TooltipData
        {
            name = cachedData.Name,
            desc = cachedData.Desc,
            targetClass = cachedData.TargetClass.ToString(),
            ParamDic = cachedData.ParamsDic
        };
    }
}
