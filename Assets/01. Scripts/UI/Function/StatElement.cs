using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatElement : MonoBehaviour
{
    [Header("UI")]  // UI 참조 영역
    [SerializeField] Image img_UnitIcon;
    [SerializeField] TextMeshProUGUI txt_TotalValue;

    [Header("Bar Layout")]  // 바의 레이아웃 설정
    [SerializeField] RectTransform barContainer;    // 너비 조절용
    [SerializeField] LayoutElement[] segments;      // 들어갈 요소들 ex. 물리,마법,고정피해

    public void SetData(StatRecord record, float maxTotal, StatTab tab)
    {
        // 레코드 ID를 유닛 아이콘으로 반환
        img_UnitIcon.sprite = IconManager.inst.GetUnitIcon(record.UniqID);

        // 바의 내용을 갱신
        UpdateBar(record, maxTotal, tab);
    }

    void UpdateBar(StatRecord record, float maxTotal, StatTab tab)
    {

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].gameObject.SetActive(false);
        }

        float sum = 0f;
        switch (tab)
        {
            case StatTab.Damage:
                sum = record.Damage_Sum;
                SetSegment(0, record.Damage_Phys, ColorDefine.PhysicsDmg);
                SetSegment(1, record.Damage_Magic, ColorDefine.MagicDmg);
                SetSegment (2, record.Damage_True, ColorDefine.TrueDmg);
                break;

            case StatTab.Received:
                sum = record.Received_Sum;
                SetSegment(0, record.Received_Phys, ColorDefine.PhysicsDmg);
                SetSegment(1, record.Received_Magic, ColorDefine.MagicDmg);
                SetSegment(2, record.Received_True, ColorDefine.TrueDmg);
                SetSegment(3, record.Received_Protected, ColorDefine.ProtectedDmg);

                break;
            case StatTab.Healed:
                sum = record.TotalHealed;
                SetSegment(0, record.TotalHealed, ColorDefine.Heal);
                break;
        }

        txt_TotalValue.text = sum.ToString("N0");

        // 너비 조절
        float ratio = (maxTotal > 0) ? sum / maxTotal : 0f;
        barContainer.localScale = new Vector3(ratio, 1, 1);
    }


    void SetSegment(int index, float value, Color color)
    {
        if (index >= segments.Length)
            return;

        // 수치가 0보다 큰 경우에만 활성화
        bool isActive = value > 0f;
        segments[index].gameObject.SetActive(isActive);

        if (isActive)
        {
            segments[index].flexibleWidth = value;

            if (segments[index].TryGetComponent(out Image img))
                img.color = color;
        }
    }
}
