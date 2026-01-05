using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager inst;

    public SkillDatabase database;

    // 보유 중인 스킬 ID와 수량을 딕셔너리로 관리
    Dictionary<string, int> skills = new Dictionary<string, int>();

    // 화면 갱신용 스크립트 참조
    public InventoryUI inventoryUI;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    // 스킬 추가 시 호출
    public void AddSkill(string id)
    {
        if (skills.ContainsKey(id))
            skills[id]++;
        else
            skills.Add(id, 1);

        RefreshUI();
    }

    // 스킬 제거 시 호출
    public void RemoveSkill(string id)
    {
        if(skills.ContainsKey(id) && skills[id] > 0)
        {
            skills[id]--;
            if (skills[id] == 0)
                skills.Remove(id);
        }

        RefreshUI();
    }

    // 정렬 로직
    public List<KeyValuePair<string, int>> GetSortedList(string sortType)
    {
        var list = skills.ToList();

        switch (sortType)
        {
            case "Name":
                return list.OrderBy(x =>
                database.GetSkillByID(x.Key).Name)
                    .ToList();
            case "Type":
                return list.OrderBy(x =>
                x.Key.StartsWith("P_")).ThenBy(x => x.Key)
                .ToList();
            default:
                return list.OrderBy(x => x.Key).ToList();
        }
    }

    void RefreshUI()
    {
        if (inventoryUI != null)
            inventoryUI.UpdateInventoryDisplay();
    }
}
