using System.Collections.Generic;
using UnityEngine;

//
// 역할: 건물의 미리보기 기능을 구현. 비주얼 제어 전용 클래스이므로 실제 드래그/드롭 액션은
//       외부 매니저에서 작업할 것.
// 사용방법: 건물 고스트 오브젝트의 부모에게 할당하여 사용
//

public class BuildGhost : MonoBehaviour
{
    [System.Serializable]
    public struct GhostMapping      // 유닛 클래스와 해당하는 건물 오브젝트를 매핑
    {
        public UnitClassType type;  // 무결성을 위해 실제 유닛 클래스와 매칭
        public GameObject modleObj;
    }

    [Header("Ghost Selection")]
    [SerializeField] private List<GhostMapping> ghostMappings;  // 인스펙터에서 매핑해서 오브젝트 등록
    Dictionary<UnitClassType, GameObject> ghostDict = new Dictionary<UnitClassType, GameObject>();

    // 현재 보여주고 있는 오브젝트의 정보 저장용 변수
    MeshRenderer[] curRederers;
    GameObject curModel;

    private void Awake()
    {
        // 매핑된 정보를 리스트로 전환
        foreach (var mapping in ghostMappings)
        {
            if(mapping.modleObj != null)
            {
                ghostDict[mapping.type] = mapping.modleObj;
                mapping.modleObj.SetActive(false);
            }
        }

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        gameObject.SetActive(false);
    }

    // 특정 클래스의 모델을 활성화
    public void Show(UnitClassType type)
    {
        if (curModel != null)
            curModel.SetActive(false);

        if(ghostDict.TryGetValue(type, out GameObject target))
        {
            curModel = target;
            curModel.SetActive(true);
            curRederers = curModel.GetComponentsInChildren<MeshRenderer>();
        }

        gameObject.SetActive(true);
    }

    // 위치와 외형 업데이트
    public void UpdateGhost(Vector3 pos, bool isValid)
    {
        transform.position = pos;

        // 셰이더 색상 조절
        Color targetColor = isValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        if(curRederers != null) 
            foreach(var r in curRederers)
                r.material.SetColor("_Color", targetColor);
                    // ※ 현재 오브젝트는 단일 메쉬로 되어있지만, 확장성을 위해..

    }

    public void Hide()
    {
        if (curModel != null)
            curModel.SetActive(false);

        curModel = null;
        gameObject.SetActive(false);
    }
}
