using System;
using System.Collections.Generic;
using UnityEngine;

// Json 파일의 데이터를 담을 구조

[Serializable]
public class SkillData
{
    public string ID;
    public string Name;
    public string Desc;
    public SkillType SkillType;

    // 엑셀의 "Knight/Barbarian"을 분리해서 저장할 리스트
    public string TargetClass;
    public List<string> TargetClassList = new List<string>();

    // 게임 내에서 직접 사용할 클래스 리스트
    public List<UnitClassType> TargetClassList_enum = new List<UnitClassType>();

    public int MotionType;
    public string LogicClass;
    public string EffectID;

    public string BestClass;
    public string SubClass;
    public int Weight;
    // 가변 파라미터 (JSON 문자열을 나중에 딕셔너리로 변환하여 사용)
    public Dictionary<string, float> Params = new Dictionary<string, float>();

    public Sprite Icon;

    // 문자열로 된 TargetClass를 리스트로 변환하는 편의 함수
    public void ParseTargetClasses()
    {
        TargetClassList.Clear();
        TargetClassList_enum.Clear();


        // 스틸 타입 파싱
        if (!string.IsNullOrEmpty(ID))
        {
            if (ID.StartsWith("A_"))
                SkillType = SkillType.Active;
            else if (ID.StartsWith("P_"))
                SkillType = SkillType.Passive;
            else
                SkillType = SkillType.None;
        }

        // 타겟 클래스 파싱
        if (string.IsNullOrEmpty(TargetClass)) return;
        string[] split = TargetClass.Split('/');
        foreach (var s in split)
        {
            string trim = s.Trim();
            if (!string.IsNullOrEmpty(trim))
            {
                TargetClassList.Add(trim);

                if (Enum.TryParse(trim, out UnitClassType result))
                    TargetClassList_enum.Add(result);
                else
                    Debug.Log($"{ID} 스킬의 {trim} 클래스명이 Enum형과 일치하지 않습니다.");
            }
        }
    }
}


// 유닛별 스킬 데이터 세트
[System.Serializable]
public class UnitSkillSet
{
    public UnitClassType Class;     // 클래스 종류
    public int Level = 1;               // 클래스 레벨(=건물 강화 레벨)

    public string ActiveID = "";         // 액티브 스킬 하나 -> 0번 점에 대응
    public string[] PassiveID = new string[3];      // 패시브 스킬 최대 3개 -> 1~3번 점에 대응

    public UnitSkillSet(UnitClassType a_Class)
    {
        Class = a_Class;
        Level = 1;
        ActiveID = "";
        PassiveID = new string[3] { "", "", "" };
    }
}

[System.Serializable]
public enum SkillType
{
    Active,
    Passive,
    None,
}