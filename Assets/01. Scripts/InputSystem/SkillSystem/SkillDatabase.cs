using System.Collections.Generic;
using UnityEngine;

// 게임 내 스킬 데이터를 담고 있을 SO 파일

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Data/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public List<SkillData> Skills = new List<SkillData>();

    public SkillData GetSkillByID(string id)
    {
        return Skills.Find(s => s.ID == id);
    }
}