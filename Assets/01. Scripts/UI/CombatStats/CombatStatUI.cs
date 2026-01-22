using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public enum StatTab
{
    Damage = 0,
    Received = 1,
    Healed = 2
}

//
// 통계 판넬 부모에 붙이고 쓰는 컴포넌트
//
// 각 버튼은 부모 오브젝트 참조해서 버튼 쪽에서 연결되어 있음
// 판넬을 열고 끄는 것은 UIStateManager에서
// (통계 버튼 오브젝트는 SlotGroup에 묶여있음 => 닫을 일 없는 항목이라.)
// 

public class CombatStatUI : MonoBehaviour
{
    [Header("UI ref")]
    [SerializeField] Transform contentRoot;
    [SerializeField] StatElement elementPrefab;
    [SerializeField] Image[] Img_Tabs;

    // 생성된 구성 요소의 리스트
    List<StatElement> elements = new List<StatElement>();
    StatTab curTab = StatTab.Damage;        // 현재 활성화 된 탭 : 기본값은 데미지로 초기화

    public void Open()
    {
        gameObject.SetActive(true);
        OnTabChanged(0);

        Refresh();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnTabChanged(int tabindex)
    {
        curTab = (StatTab)tabindex;
        for (int i = 0; i < Img_Tabs.Length; i++)
        {
            if (i == tabindex)
                Img_Tabs[i].color = new Color(1f, 1f, 1f, 1f);
            else
                Img_Tabs[i].color = new Color(0.6f, 0.6f, 0.6f, 0.7f);

        }
        Refresh();
    }

    void Refresh()
    {
        // 컴뱃 매니저에서 누적된 통계를 가져오기
        var allStats = CombatManager.Inst.GetAllStats().ToList();
         
        // 데이터가 없는 경우 리턴
        if(allStats == null || allStats.Count == 0)
        {
            Clear();
            return;
        }

        float maxVal = 0;
        var sorted = SortStats(allStats, out maxVal);

        if(maxVal <= 0)
        {
            Clear();
            return;
        }

        // 통계 구성요소 생성 및 갱신
        for (int i = 0; i < sorted.Count; i++)
        {
            // 데이터 생성
            StatElement element = GetElement(i);
            element.gameObject.SetActive(true);

            // 데이터 주입
            element.SetData(sorted[i], maxVal, curTab);

            // 하이어라키 순서 정렬
            element.transform.SetSiblingIndex(i);
        }

        // 사용하지 않는 UI 숨기기
        for (int i = sorted.Count; i < elements.Count; i++)
        {
            elements[i].gameObject.SetActive(false);
        }
    }

    List<StatRecord> SortStats(List<StatRecord> stats, out float maxVal)
    {
        List<StatRecord> sorted;
        switch (curTab)
        {
            case StatTab.Damage:
                sorted = stats.OrderByDescending(s => s.Damage_Sum).ToList();
                break;
            case StatTab.Received:
                sorted = stats.OrderByDescending(s => s.Received_Sum).ToList();
                break;
            case StatTab.Healed:
                sorted = stats.OrderByDescending(s => s.TotalHealed).ToList();
                break;
            default:
                sorted = stats;
                break;
        }

        maxVal = sorted.Count > 0 ? GetSum(sorted[0], curTab) : 0;
        return sorted;
    }

    float GetSum(StatRecord record, StatTab tab)
    {
        return tab switch
        {
            StatTab.Damage => record.Damage_Sum,
            StatTab.Received => record.Received_Sum,
            _ => record.TotalHealed
        };
    }

    StatElement GetElement(int index)
    {
        // 이미 활성화 된 요소는 재사용
        if(index < elements.Count)
            return elements[index];

        // 없으면 생성
        StatElement e = Instantiate(elementPrefab, contentRoot);
        elements.Add(e);
        return e;
    }

    void Clear()
    {
        foreach (var element in elements)
            if (element != null) element.gameObject.SetActive(false);
    }
}
