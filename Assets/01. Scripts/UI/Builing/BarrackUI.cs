using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 배럭의 정보를 보여주는 기능 UI

public class BarrackUI : MonoBehaviour
{
    Barracks curBarracks;           // 현재 출력 중인 배럭의 인스턴스

    [Header("UI")]
    [SerializeField] GameObject Panel;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI upgradeText;

    [Header("Buttons")]
    [SerializeField] Button upgradeBtn;
    [SerializeField] Button sellBtn;
    [SerializeField] Button rallyModeBtn;

    private void Awake()
    {
        if (upgradeBtn != null)
            upgradeBtn.onClick.AddListener(OnClickUpgrade);
        if (sellBtn != null)
            sellBtn.onClick.AddListener(OnClickSell);
        if (rallyModeBtn != null)
            rallyModeBtn.onClick.AddListener(OnClickRallyMode);
    }

    public void SetUp(Barracks target)
    {
        if (target == null)
            return;

        // 기존 타겟에 선택 해제
        if (curBarracks != null)
            curBarracks.RallyOff();

        curBarracks = target;
        curBarracks.RallyOn();

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

    const string rallyMsg = "";
    void OnClickRallyMode()
    {
        InfoMassage.inst.ShowMessage("유닛의 첫 집결 위치를 설정하세요");
    }

    public void Close()
    {
        if (curBarracks != null)
            curBarracks.RallyOff();
        curBarracks = null;
        gameObject.SetActive(false);
    }
}
