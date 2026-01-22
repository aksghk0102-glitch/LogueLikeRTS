using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    public GamePhase curPhase;
    int curCost;

    public int TurnCount => turnCount;
    public int CurCost => curCost;

    //temp
    public TextMeshProUGUI text;

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
        InfoMassage.inst.ShowPerMessage(ReadyMsg);

        AddCost(10);
    }

    void EnterBattlePhase()
    {
        InfoMassage.inst.ShowMessage("");

        // 사운드 호출
        SoundManager.inst.PlaySFX("BattleStart");

        // 데미지 통계 초기화
        CombatManager.Inst.ResetCombatStats();

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

        // 전투 통계 표시를 위한 감시자 작동하게 해야 함

    }

    void EnterResultPhase()
    {
        // 잠시 대기...

        // 전투 결과창 출력
        UIStateManager.inst.CombatStatOpen();

        // 준비 페이즈로 복구
        EnterReadyPhase();
    }

    void EnterGameOverPhase()
    {

    }
    #endregion

    #region Cost System
    public void AddCost(int amount)
    {
        curCost += amount;

        // UI 업데이트 : 임시
        text.text = curCost.ToString();
    }

    const string tarinai = "코스트가 부족합니다.";
    public bool SpendCost(int amount)
    {
        if (curCost >= amount)
        {
            curCost -= amount;

            // UI 업데이트 : 임시
            text.text = curCost.ToString();
            return true;
        }

        InfoMassage.inst.ShowMessage(tarinai);
        return false;
    }

    #endregion
}
