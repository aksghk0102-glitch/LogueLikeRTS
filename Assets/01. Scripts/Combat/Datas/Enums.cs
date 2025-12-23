using UnityEngine;

public enum UnitFaction
{
    Player,
    Enemy,
    Neutral,
}

// 유닛의 5종 클래스를 정의
[System.Serializable]
public enum UnitClassType
{
    Babarian,
    Knight,
    Rogue,
    Ranger,
    Mage,

    SK_Worrior,
    SK_Rogue,
    SK_Ranger,
    SK_Mage,
    SK_Minion,
}

// 스킬 애니메이션 매핑
public enum SkillMotionType
{
    Motion_1 =1,
    Motion_2 = 2,
    Motion_3 = 3,
}

// 능력치 연산 방식 정의
public enum StatCalcType
{
    Flat,
    PercentAdd,
}