using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 배럭의 정보를 보여주는 기능 UI

public class BarrackUI : MonoBehaviour
{
    Barracks curBarracks;           // 현재 출력 중인 배럭의 인스턴스

    [Header("UI")]
    [SerializeField] GameObject Panel;
    [SerializeField] Image curImage;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI upgradeText;

    [Header("Buttons")]
    [SerializeField] Button upgradeBtn;
    [SerializeField] Button sellBtn;
    [SerializeField] Button showInfoBtn;

    private void Awake()
    {
        if (upgradeBtn != null)
            upgradeBtn.onClick.AddListener(OnClickUpgrade);
        if (sellBtn != null)
            sellBtn.onClick.AddListener(OnClickSell);
    }

    public void SetUp(Barracks target)
    {
        if (target == null)
            return;

        curBarracks = target;

        Refresh();
        gameObject.SetActive(true);
    }

    public void Refresh()
    {
        // 
        if (curBarracks == null)
            return;

        levelText.text = $"Lv. {curBarracks.CurLevel}";
        upgradeText.text = $"";     // 업그레이드 기획 완료 후 추가

    }

    void OnClickUpgrade()
    { 
        if (curBarracks == null)
            return;
     
        // 강화 시도
        curBarracks.Upgrade();
        Refresh();
    }

    void OnClickSell()
    {
        if (curBarracks == null)
            return;

        curBarracks.Sell();
        Close();
    }

    public void Close()
    {
        curBarracks = null;
        gameObject.SetActive(false);
    }
}
