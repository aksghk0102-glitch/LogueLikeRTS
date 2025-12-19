using UnityEngine;

[CreateAssetMenu(fileName = "AIProfile", menuName = "Game/AIProfaile")]
public class AIProfile : ScriptableObject
{
    // 데이터 분기의 키 제공
    [Header("Targeting")]
    public TargetPriority Priority;

    [Header("Positioning")]
    public PostionPref PostionPref;

    [Header("Behavior Weights")]
    [Range(0f, 1f)] public float arression;
    [Range(0f, 1f)] public float mobility;
    [Range(0f, 1f)] public float survival;
}

public enum TargetPriority
{
    Closest,            // 가장 가까운 대상
    LowestHP,           // 사거리 내 낮은 체력
    HasStatusEff,       // 상태이상 보유
    BackLine,           // 후방 먼저 -> 밸런스 때문에 넣는 게 맞는지 모르겠음
    Random,             // 랜덤 타겟
}

public enum PostionPref
{
    Front,
    Midle,
    Back,
    Flank,
}