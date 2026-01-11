using UnityEngine;
using UnityEngine.UI;

// 스킬 슬롯 구성 시 중복되는 부분이 많아 별개 스크립트로 묶음
// 부모 객체에 붙인 뒤 자식 객체로 프레임 이미지, 아이콘 이미지 생성 후
// 해당 컴포넌트에 참조시켜서 사용

// 비주얼 외의 기능은 없어야 함.

// 구체적인 사용처 : SkillSlotUI (인벤토리 내 개별 슬롯),
// ClassInfoUI (클래스 별 정보 팝업 시 슬롯),
// GhostSlot (슬롯 드래그 시 미리보기 슬롯)

public class SkillSlotVisual : MonoBehaviour
{
    public Image frame;
    public Image icon;

    public void SetVisual(SkillData data)
    {
        if(data == null)
        {
            Init();
            return;
        }

        icon.sprite = data.Icon;
        icon.color = ColorDefine.DefautColor;
        icon.enabled = true;

        // 추후 등급 추가 시 프레임 색상은 확장
        frame.color = ColorDefine.DefautColor;
    }
    
    public void Init()
    {
        icon.sprite = null;
        icon.color = ColorDefine.Clear;
        icon.enabled = false;
        
        frame.color = ColorDefine.DefautColor;  
    }

    // 고스트 생성 후 드래그 시 투명도 조절
    public void SetAlpha()
    {
        icon.color = ColorDefine.Ghost;
        frame.color = ColorDefine.Ghost;
    }

}
