using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventoryManager Manager;
    public GameObject SlotPrefab;
    public Transform ContentParent;

    string curSortType = "ID";

    public void UpdateInventoryDisplay()
    {
        foreach (Transform child in ContentParent)
            Destroy(child.gameObject);

        var sorted = Manager.GetSortedList(curSortType);

        foreach (var item in sorted)
        {
            SkillData data = Manager.database.GetSkillByID(item.Key);
            GameObject obj = Instantiate(SlotPrefab, ContentParent);

            obj.GetComponent<SkillSlotUI>().SetSlot(data, item.Value);
        }
    }

    public void SetSortType(string type)
    {
        curSortType = type;
        UpdateInventoryDisplay();
    }
}
