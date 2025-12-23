using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 배럭의 정보를 보여주는 기능 UI
// 매니저 모음 집 내 UI CTRL에 있음

public class BarrackUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject panel;
    [SerializeField] Image progressBar;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Button requestSpawnBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] Button sellBtn;
    Barracks targetBarracks;

    private void Start()
    {
        if (closeBtn != null)
            closeBtn.onClick.AddListener(CloseUI);

        if (requestSpawnBtn != null)
            requestSpawnBtn.onClick.AddListener(OnCilckSpawnUnit);
    }

    public void OpenUI(Barracks barracks)
    {
        targetBarracks = barracks;
        costText.text = targetBarracks.Cost.ToString();
        panel.SetActive(true);
    }

    public void CloseUI()
    {
        targetBarracks = null;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (targetBarracks == null)
            return;

        // 생산 진행도 갱신
        progressBar.fillAmount = targetBarracks.GetProgressGague();

        // 대기열 이미지 갱신

    }

    public void OnCilckSpawnUnit()
    {
        if (targetBarracks == null)
            return;

        if (GoldManager.inst.SpendGold(targetBarracks.Cost))
            targetBarracks.RequestSpawnUnit();
    }
}
