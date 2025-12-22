using UnityEngine;

public enum UnitFaction
{
    Player,
    Enemy,
}

// 유닛의 5종 클래스를 정의
[System.Serializable]
public enum UnitClassType
{
    Babarian,
    Knight,
    Rogue,
    Ranger,
    Mage
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