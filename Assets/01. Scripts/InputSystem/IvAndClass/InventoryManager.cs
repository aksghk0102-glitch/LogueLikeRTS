using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager inst;

    // Json 파싱 후 임포트 한 데이터 베이스 참조
    public SkillDatabase database;
    // 인벤토리 UI 판넬 내 스크롤뷰/콘텐츠 영역에 할당된 컴포넌트 참조
    public InventoryUI inventoryUI;


    // 보유 중인 스킬 ID와 수량을 딕셔너리로 관리
    Dictionary<string, int> inventory = new Dictionary<string, int>();


    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    // 스킬 추가 시 호출
    public void AddSkill(string id, int amount=1)
    {
        if (inventory.ContainsKey(id))
        {
            inventory[id] += amount;
            if (inventory[id] < 0)
                inventory[id] = 0;
        }
        else
            inventory.Add(id, amount);

        RefreshUI();
    }

    // 스킬 제거 시 호출
    public void RemoveSkill(string id)
    {
        if(inventory.ContainsKey(id) && inventory[id] > 0)
        {
            inventory[id]--;
            if (inventory[id] == 0)
                inventory.Remove(id);
        }

        RefreshUI();
    }

    public int GetSkillCount(string id)
    {
        if (inventory.TryGetValue(id, out int count))
            return count;
        return 0;
    }
    public SkillData GetSkillData(string id)
        => database.GetSkillByID(id);

    public Dictionary<string, int> GetInventory()
        => inventory;
    // 정렬 로직
    public List<KeyValuePair<string, int>> GetSortedList(string sortType)
    {
        var list = inventory.ToList();

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
