using UnityEngine;
using System.Collections.Generic;
using System;

// 스킬 데이터를 읽어와 실제 데이터로 변환합니다.

public static class SkillFactory
{
    static readonly Dictionary<string, Type> cachedTypes = new Dictionary<string, Type>();

    public static SkillBase CreateSkillInstance(SkillData skillData)
    {
        // Logic Class 정보를 기점으로 데이터를 분류
        string className = skillData.LogicClass;

        if (string.IsNullOrEmpty(className) || className == "default")
            return null;

        if(!cachedTypes.TryGetValue(className, out Type type))
        {
            type = Type.GetType(className);
            if(type != null )
                cachedTypes[className] = type;
        }

        if(type != null)
        {
            SkillBase skillInst = Activator.CreateInstance(type) as SkillBase;
            skillInst?.Init(skillData);

            return skillInst;
        }

        return null;
    }
}
