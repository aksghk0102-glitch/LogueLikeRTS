using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SkillDataImporter
{
    [MenuItem("Tools/Import Skill Data")]
    public static void Import()
    {
        string jsonPath = Application.dataPath + "/Resources/Data/SkillDataSheet.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + jsonPath);
            return;
        }

        string jsonText = File.ReadAllText(jsonPath);

        // JSON을 List<SkillData>로 변환
        List<SkillData> importedSkills = JsonConvert.DeserializeObject<List<SkillData>>(jsonText);

        // ScriptableObject 로드 또는 생성
        SkillDatabase db = AssetDatabase.LoadAssetAtPath<SkillDatabase>("Assets/Resources/Data/SkillDatabase.asset");
        
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<SkillDatabase>();
            AssetDatabase.CreateAsset(db, "Assets/Resources/Data/SkillDatabase.asset");
        }

        db.Skills.Clear();
        foreach (var skill in importedSkills)
        {
            skill.ParseData(); // 다중 타겟 클래스, 가변변수 파싱

            // 아이콘 데이터 파싱
            if (!string.IsNullOrEmpty(skill.IconPath))
            {
                string path = "Assets/Resources/Icons/" + skill.IconPath + ".png";
                Sprite iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (iconSprite != null)
                    skill.Icon = iconSprite;
                else
                    Debug.Log($"아이콘을 찾을 수 없습니다 : {path} : id {skill.ID}");
            }

            db.Skills.Add(skill);
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log("스킬 데이터베이스 업데이트 완료!");
    }
}