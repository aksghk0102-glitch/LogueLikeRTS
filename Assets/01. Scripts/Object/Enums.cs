// enum형을 정의해둔 스크립트입니다.


#region 전투 로직 관련 enum
[System.Serializable]
public enum UnitState
{
    MoveAndSearch,      // 이동 및 탐색
    Chase,              // 탐색된 적을 추적
    Attack,             // 적이 사거리 내에 있다면 공격 시도
    UseSkill,           // 스킬 구현 확장용 임시 플래그
    Die,                // 사망 처리

    Idle,               // 기본 상태
}
[System.Serializable]
public enum UnitFaction
{
    Player,
    Enemy,
}
#endregion