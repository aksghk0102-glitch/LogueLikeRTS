using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class InventorySlot
{
    public string SkillID;
    public int Count;
    public bool IsEmpty => string.IsNullOrEmpty(SkillID) || Count <= 0;

    public void Clear()
    {
        SkillID = null;
        Count = 0;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager inst;

    // Json 파싱 후 임포트 한 데이터 베이스 참조
    public SkillDatabase database;

    // 슬롯을 배열로 관리
    [SerializeField] List<InventorySlot> slots = new List<InventorySlot>();

    const int ROW_COUNT = 8;
    const int INIT_SIZE = 24;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        slots.Clear();
        for (int i = 0; i < INIT_SIZE; i++)
            slots.Add(new InventorySlot());
    }

    private void Start()
    {
        // 테스트 스킬 추가{
        for (int i= 1; i <= 15; i++)
            AddSkill($"P_ALL_{i.ToString("D2")}");

        // 여기서 호출하는 이유는 초기화 순서 오류 방지
        InventoryUI.inst.UpdateInventory();
    }

    // 스킬 추가 시 호출
    public void AddSkill(string id, int amount = 1)
    {
        // 가지고 있는 스킬인지 확인
        var exSlot = slots.Find(x => x.SkillID == id);
        if (exSlot != null)
            exSlot.Count += amount;
        else if(amount > 0)
        {
            // 빈칸 조회 후 할당
            int emptyIdx = slots.FindIndex(x => x.IsEmpty);
            if (emptyIdx == -1) // 빈 칸이 없는 경우
            {
                // 확장
                ExpandInventory();
                // 재시도
                AddSkill(id, amount);
                return;
            }

            slots[emptyIdx].SkillID = id;
            slots[emptyIdx].Count = amount;
        }

        RefreshUI();
    }

    // 스킬 제거 시 호출
    public void RemoveSkill(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        var slot = slots[index];
        if (!slot.IsEmpty)
        {
            slot.Count--;
            if (slot.Count <= 0)
                slot.Clear();

            RefreshUI();
        }
    }

    void ExpandInventory()
    {
        for (int i = 0; i < ROW_COUNT; i++)
            slots.Add(new InventorySlot());
    }

    public void SortInventory(string sortType)
    {
        var items = slots.Where(x => !x.IsEmpty).ToList();

        if (sortType == "Name")
            items = items.OrderBy(x => GetSkillData(x.SkillID).Name)
                .ToList();
        else if (sortType == "Type")
            items = items.OrderBy(x => x.SkillID.StartsWith("A_"))
                .ThenBy(x => x.SkillID)
                .ToList();

        for (int i = 0; i<slots.Count; i++)
        {
            if (i < items.Count)
            {
                slots[i].SkillID = items[i].SkillID;
                slots[i].Count = items[i].Count;
            }
            else
                slots[i].Clear();
        }

        RefreshUI();
    }

    public void SwapSlot(int idxA, int idxB)
    {
        InventorySlot temp = slots[idxA];
        slots[idxA] = slots[idxB];
        slots[idxB] = temp;

        RefreshUI();
    }

    public int GetSlotCount() => slots.Count;
    public InventorySlot GetSlot(int idx) => (idx >= 0 && idx < slots.Count) ?
        slots[idx] : null;    
    public SkillData GetSkillData(string id)
        => database.GetSkillByID(id);
    

    void RefreshUI()
    {
        InventoryUI.inst.UpdateInventory();
    }
}
