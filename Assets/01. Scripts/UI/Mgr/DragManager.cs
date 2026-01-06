using UnityEngine;

public class DragManager : MonoBehaviour
{
    public static DragManager inst;

    // 미리 보기 오브젝트 관리
    [Header("Ghost")]
    public SkillSlotVisual ghostVisual;
    public RectTransform ghostRect;

    // 현재 드래그 중인 스킬의 ID 저장
    public string curSkillId { get; private set; }

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        ghostVisual.gameObject.SetActive(false);
        ghostVisual.Init();
    }

    public void SetGhost(string skillID)
    {
        curSkillId = skillID;

         SkillData data = InventoryManager.inst.GetSkillData(curSkillId);
        if(data != null)
        {
            ghostVisual.SetVisual(data);
            ghostVisual.SetAlpha();
            ghostVisual.gameObject.SetActive(true);
        }
    }

    public void HideGhost()
    {
        curSkillId = null;
        ghostVisual.gameObject.SetActive(false);
    }
}
