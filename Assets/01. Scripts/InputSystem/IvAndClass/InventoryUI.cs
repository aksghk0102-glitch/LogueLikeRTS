using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject SlotPrefab;
    public Transform ContentParent;

    string curSortType = "ID";

    public void UpdateInventoryDisplay()
    {
        foreach (Transform child in ContentParent)
            Destroy(child.gameObject);

        var sorted = InventoryManager.inst.GetSortedList(curSortType);

        foreach (var item in sorted)
        {
            GameObject obj = Instantiate(SlotPrefab, ContentParent);
            obj.GetComponent<SkillSlotUI>().SetSlot(item.Key, item.Value);
        }
    }

    public void SetSortType(string type)
    {
        curSortType = type;
        UpdateInventoryDisplay();
    }
}
