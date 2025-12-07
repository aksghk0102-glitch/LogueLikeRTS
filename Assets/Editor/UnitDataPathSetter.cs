using UnityEngine;
using UnityEditor;
using System.IO;

public class UnitDataPathSetter
{
    // 빌드 전 SO파일로 작성되어 있는 UnitData의 경로를 자동으로 할당합니다.
    // 올바른 경로에 프리팹만 두면 자동으로 풀링이 가능하도록 하기 위한 코드입니다.

    // 유닛 데이터SO와 프리팹이 위치한 Resouces 폴더 내의 경로
    const string UnitPrefab_Path = "Assets/Resources/UnitPrefab";
    const string UnitDataSO_Path = "Assets/Resources/UnitDataSO";

    const string PrePoolPath = "UnitPrefab/";

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void OnScriptsReloaded()
    {
        // 필수 폴더 체크
        if (!Directory.Exists(UnitPrefab_Path))
        {
            Debug.LogWarning("유닛 자동 설정에 필요한 SO 폴더가 없습니다.");
            return;
        }
        if (!Directory.Exists(UnitDataSO_Path))
        {
            Debug.LogWarning("유닛 자동 설정에 필요한 SO 폴더가 없습니다.");
            return;
        }

        // UnitData 타입 에셋 검색
        string[] datas = AssetDatabase.FindAssets("t:UnitData", new[] {UnitDataSO_Path });

        foreach(string d in datas)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(d);
            UnitData unitData = AssetDatabase.LoadAssetAtPath<UnitData>(assetPath);

            if(unitData != null)
            {
                SetObjPoolPath(unitData, assetPath);
            }
        }
    }

    static void SetObjPoolPath(UnitData unitData, string soPath)
    {
        // SO파일의 이름 추출
        string soFileName = Path.GetFileNameWithoutExtension(soPath);

        // SO 파일과 같은 이름의 프리팹 검색
        string filter = $"t:Prefab {soFileName}";
        string[] prefabs = AssetDatabase.FindAssets(filter, new[] {UnitPrefab_Path});

        string targetPath = "";
        
        foreach (string p in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(p);
            if(Path.GetFileNameWithoutExtension(path) == soFileName)
            {
                targetPath = path;
                break;
            }
        }

        // 최종 경로 지정
        string newPath = "";
        if (!string.IsNullOrEmpty(targetPath))
        {
            string finalName = Path.GetFileNameWithoutExtension(targetPath);
            newPath = PrePoolPath + finalName;
        }


        // UnitData에 적용합니다.
        if(unitData.objPoolPath != newPath)
        {
            unitData.objPoolPath = newPath;
            EditorUtility.SetDirty(unitData);
        }
        AssetDatabase.SaveAssets();

    }
}
