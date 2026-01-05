using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

// 하단부 클래스 슬롯에 들어갈 UI 컴포넌트

public class ClassSlotUI : MonoBehaviour
{
    [Header("Class")]
    public UnitClassType classType;

    [Header("UI Reference")]
    public Image[] stateDots;       // 스킬 상태 표시용
    public TextMeshProUGUI levelText;   // 강화 레벨 표시 텍스트

    public void RefreshSlotUI(UnitSkillSet data)
    {
        if(data.Class != classType)
        {
            // 안내 메세지 띄우기 ?
            return;
        }

        levelText.text = data.Level.ToString();

        stateDots[0].color = string.IsNullOrEmpty(data.ActiveID) ?
            ColorDefine.Empty : ColorDefine.Active;

        for (int i = 0; i < 3; i++)
        {
            stateDots[i+1].color = string.IsNullOrEmpty(data.PassiveID[i]) ?
                ColorDefine.Empty : ColorDefine.Passive;
        }
    }
}
