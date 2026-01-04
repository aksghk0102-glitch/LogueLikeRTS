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
    public string IconPath;

    // 엑셀의 "Knight/Barbarian"을 분리해서 저장할 리스트
    public string TargetClass;
    public List<string> TargetClassList = new List<string>();

    public int MotionType;
    public string LogicClass;
    public string EffectID;

    public string BestClass;
    public string SubClass;
    public int Weight;

    // 가변 파라미터 (JSON 문자열을 나중에 딕셔너리로 변환하여 사용)
    public Dictionary<string, float> Params = new Dictionary<string, float>();

    // 문자열로 된 TargetClass를 리스트로 변환하는 편의 함수
    public void ParseTargetClasses()
    {
        TargetClassList.Clear();
        if (string.IsNullOrEmpty(TargetClass)) return;

        string[] split = TargetClass.Split('/');
        foreach (var s in split)
        {
            string trim = s.Trim();
            if(!string.IsNullOrEmpty(trim))
                TargetClassList.Add(trim);
        }
    }
}