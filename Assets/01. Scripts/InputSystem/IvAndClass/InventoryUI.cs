using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI inst;

    public GameObject SlotPrefab;
    public Transform ContentParent;

    //string curSortType = "ID";
    List<SkillSlotUI> uiSlots = new List<SkillSlotUI>();

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    public void UpdateInventory()
    {
        // 인벤토리 전체 화면 갱신
        int dataCount = InventoryManager.inst.GetSlotCount();

        while (uiSlots.Count < dataCount)
            CreateNewSlot();

        for (int i = 0; i < uiSlots.Count; i++)
        {
            var slotData = InventoryManager.inst.GetSlot(i);

            if(slotData != null && !slotData.IsEmpty)
            {
                uiSlots[i].SetSlot(slotData.SkillID, slotData.Count);
                uiSlots[i].gameObject.SetActive(true);
            }
            else
                uiSlots[i].ClearSlot();

            // 슬롯 정보를 UI에 부여 > 드래그 앤 드롭 시 조회
            uiSlots[i].SlotIndex = i;
        }
    }

    void CreateNewSlot()
    {
        GameObject obj = Instantiate(SlotPrefab, ContentParent);
        SkillSlotUI slotUI = obj.GetComponent<SkillSlotUI>();
        if(slotUI != null)
            uiSlots.Add(slotUI);
    }
}
