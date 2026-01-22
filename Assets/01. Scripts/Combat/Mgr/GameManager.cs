using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

// 게임의 상태와 흐름을 관리합니다.

public enum GamePhase
{
    Ready,          // 건물 배치 및 강화
    Battle,         // 전투 시작
    Result,         // 턴 종료 후 결과 확인
    GameOver,       // 게임 종료
}

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    [Header("Game Settings")]
    [SerializeField] int turnCount = 1;
    [SerializeField] int initCost = 10;
    public float maxBattleTime = 90.5f;           // 기본 90초, 맵 스케일에 따라 변경

    public GamePhase curPhase;
    int curCost;
    float curTime;
    bool isTimeRunning = false;

    public int TurnCount => turnCount;
    public int CurCost => curCost;

    [Header ("UI Reference")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI timeText;
    public RectTransform costImage;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 턴 초기화 및 준비 단계로 설정
        turnCount = 1;
        SetPhase(GamePhase.Ready);
    }

    private void Update()
    {
        if (isTimeRunning)
            UpdateTimer();
    }

    // 상태를 전환하고 각 페이즈 별 알맞은 함수를 호출
    public void SetPhase(GamePhase targetPhase)
    {
        curPhase = targetPhase;

        switch (curPhase)
        {
            case GamePhase.Ready:
                EnterReadyPhase();
                break;
            case GamePhase.Battle:
                EnterBattlePhase();
                break;
            case GamePhase.Result:
                EnterResultPhase();
                break;
            case GamePhase.GameOver:
                EnterGameOverPhase();
                break;
        }
    }

    #region Phase

    const string ReadyMsg = "클래스 슬롯을 드래그 해 건물을 배치하세요";
    void EnterReadyPhase()
    {
        isTimeRunning = false;
        curTime = maxBattleTime;
        UpdateTimer();

        InfoMassage.inst.ShowPerMessage(ReadyMsg);
        AddCost(initCost);
    }

    void EnterBattlePhase()
    {
        InfoMassage.inst.ShowMessage("");

        // 사운드 호출
        SoundManager.inst.PlaySFX("BattleStart");

        // 데미지 통계 초기화
        CombatManager.Inst.ResetCombatStats();

        // 타이머 시작
        isTimeRunning = true;

        // 1. 모든 진영의 배럭 리스트를 안전하게 가져옴
        List<Barracks> targetBarracks = ObjectManager.Inst.GetAllBarracks();

        // 2. 수집된 복사본 리스트를 순회하므로, 
        // 내부에서 RegistObject가 발생해도 루프에 영향을 주지 않음
        foreach (var barracks in targetBarracks)
        {
            if (barracks != null)
            {
                barracks.SpawnUnit();
            }
        }
    }

    void EnterResultPhase()
    {
        // 잠시 대기...
        

        // 전투 결과창 출력
        UIStateManager.inst.CombatStatOpen();

        // 준비 페이즈로 복구
        SetPhase(GamePhase.Ready);
    }

    void EnterGameOverPhase()
    {
        isTimeRunning = false;
    }
    #endregion

    #region Timer System
    void UpdateTimer()
    {
        curTime -= Time.deltaTime;

        if(curTime <= 0)
        {
            curTime = 0;
            isTimeRunning = false;
            SetPhase(GamePhase.Result);

            // 남아 있는 유닛 처리
        }

        DisplayTimer();
    }

    void DisplayTimer()
    {
        if (timeText == null)
            return;

        int minutes = Mathf.FloorToInt(curTime / 60);
        int seconds = Mathf.FloorToInt(curTime % 60);
        timeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    #endregion

    #region Cost System
    Vector3 punchScale = new Vector3(0.15f, 0.15f, 0f);
    public void AddCost(int amount)
    {
        curCost += amount;

        // UI 업데이트
        UpdateCostUI();
    }

    const string tarinai = "코스트가 부족합니다.";
    public bool SpendCost(int amount)
    {
        if (curCost >= amount)
        {
            curCost -= amount;

            // UI 업데이트
            UpdateCostUI();
            return true;
        }

        InfoMassage.inst.ShowMessage(tarinai);
        return false;
    }

    void UpdateCostUI()
    {
        // 숫자 반영
        costText.text = curCost.ToString();

        // 펀칭
        if (costImage != null)
        {
            costImage.DOKill(true);
            costImage.DOPunchScale
            (punchScale, 0.2f, 1, 1f);
        }

    }


    #endregion
}
