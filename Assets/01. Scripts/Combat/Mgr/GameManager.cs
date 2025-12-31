using UnityEngine;
using System;
using System.Collections;

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
    [Header("Game Settings")]
    [SerializeField] int turnCount = 1;
    [SerializeField] int initCost = 10;

    GamePhase curPhase;
    int curCost;

    private void Start()
    {
        // 턴 초기화 및 준비 단계로 설정
        turnCount = 1;
        SetPhase(GamePhase.Ready);
    }

    // 상태를 전환하고 각 페이즈 별 알맞은 함수를 호출
    void SetPhase(GamePhase targetPhase)
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
    void EnterReadyPhase()
    {
        AddCost(10);
    }

    void EnterBattlePhase()
    {

    }

    void EnterResultPhase()
    {

    }

    void EnterGameOverPhase()
    {

    }
    #endregion

    #region Cost System
    public void AddCost(int amount)
    {
        curCost += amount;

        // UI 업데이트
    }

    public bool SpendCost(int amount)
    {
        if (curCost >= amount)
        {
            curCost -= amount;
            return true;
        }
        return false;
    }

    #endregion
}
