using UnityEngine;
using UnityEngine.UI;

// 전투 내에서 자원 관리를 담당하는 매니저입니다.

public class GoldManager : MonoBehaviour
{
    public static GoldManager inst;

    [Header("Resource Settings")]
    [SerializeField] float gold = 0;

    [Header("UI")]
    [SerializeField] Text tempGoldText;

    float bonus = 0f;

    public float CurGold => gold;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        gold = 80f;
    }

    // 골드 소모
    public bool SpendGold(float amount)
    {
        if(gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    // 광산에서 호출해 추가 수입을 늘림
    public void AddBonusGold(float amount)
    {
        bonus += amount;
    }
}
