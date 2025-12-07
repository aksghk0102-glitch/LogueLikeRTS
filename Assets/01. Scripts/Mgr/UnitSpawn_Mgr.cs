using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// =====================================================
//
// 유닛을 생성하고 동적으로 풀링 작업을 실행합니다.
// Resouces 폴더 내에 프리팹을 배치하고,
// ★프리팹의 UnitData 내에서 'Units/(오브젝트 이름)'으로
// 경로를 지정하면 자동으로 풀링이 가능합니다.

// ※ 사용 시 메모 ※ 
// GetUnit() 함수 호출로 유닛을 생성 + 자동으로 풀링 합니다.
// 유닛 사망 등 풀에 되돌릴 때는 ReturnUnit()을 호출합니다.
//
// =====================================================

public class UnitSpawn_Mgr : MonoBehaviour
{
    public static UnitSpawn_Mgr inst;

    // 유닛 풀
    [SerializeField] Transform unitPoolParent;
    Dictionary<int, Queue<GameObject>> unitPools = new Dictionary<int, Queue<GameObject>>();
    Dictionary<int, UnitData> objDataDic = new Dictionary<int, UnitData>();

    // 드래그 앤 드랍 시 미리보기 효과(고스트)용 풀
    [SerializeField] Transform ghostPoolParent;
    [SerializeField] Material ghotsMaterial;
    Dictionary<int, Queue<GameObject>> ghostUnitPools = new Dictionary<int, Queue<GameObject>>();

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else { Destroy(gameObject); return; }

        InitObjDic();
    }

    // 테스트 코드
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnUnit(0, UnitFaction.Player, new Vector3(-8, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnUnit(1, UnitFaction.Enemy, new Vector3(8, 0, 0));
        }
    }

    #region 유닛 프리펩 초기화/생성/반환
    void InitObjDic()
    {
        // 데이터 리스트 초기화
        UnitData[] allUnitData = Resources.LoadAll<UnitData>("UnitDataSo");

        foreach (UnitData data in allUnitData)
        {
            // 중복된 인덱스는 추가하지 않음
            if (objDataDic.ContainsKey(data.unitUniqIndex))
                continue;

            // 딕셔너리 추가
            objDataDic.Add(data.unitUniqIndex, data);
            Debug.Log($"[UnitSpawn_Mgr] 유닛 데이터 등록 완료 index :{data.unitUniqIndex}, name : {data.name}, path : {data.objPoolPath}");
        }
        Debug.Log($"[UnitSpawn_Mgr] 등록된 유닛 수 : {objDataDic.Count}");
    }

    public void SpawnUnit(int index, UnitFaction a_Faction, Vector3 Pos = new Vector3() )
    {
        if (Game_Mgr.Inst?.gameState != GameState.GS_IsPlaying)
            return;

        // 플레이어의 경우 코스트 체크
        if(a_Faction == UnitFaction.Player)
        {
            // 코스트 소모
            if (!objDataDic.TryGetValue(index, out var unitData))
                return;
            int needCost = unitData.cost;
            if (!Cost_Mgr.Inst.TrySpendCost(needCost))
                return;

        }
        
        // 유닛 생성
        UnitCtrl spawn = GetUnit(index)?.GetComponent<UnitCtrl>();

        if (spawn == null)
            return;

        spawn.Init(a_Faction);
        spawn.SetPosition(Pos);
    }

    public GameObject GetUnit(int index)
    {
        // 매개변수로 유닛마다 할당되어있는 인덱스(ObjectStats에서 정의)를 받습니다
        // 인덱스 값을 키로 활용하여 풀에서 해당 오브젝트를 반환합니다.

        // 유효하지 않은 인덱스는 리턴 null
        if (!objDataDic.TryGetValue(index, out var targetData))
        {
            Debug.Log("등록되지 않은 인덱스입니다.");
            return null;
        }

        // 해당 인덱스의 유닛 풀이 없을 경우 동적 생성
        if (!unitPools.TryGetValue(index, out var pool))
        {
            pool = new Queue<GameObject>();
            unitPools.Add(index, pool);
        }

        if (pool.Count == 0)
        {
            // ※프리팹이 UnitData에 지정되어 있는 경로에 있어야 함
            GameObject originPrefab = Resources.Load<GameObject>(targetData.objPoolPath);

            // 프리팹 로드 실패
            if (originPrefab == null)
            {
                Debug.Log($"프리팹 로드 실패 : 목표 인덱스 {index}");
                return null;
            }

            // 받아온 정보로 새로운 게임 오브젝트 생성 후 풀에 추가
            GameObject newUnit = Instantiate(originPrefab, unitPoolParent);
            newUnit.SetActive(false);
            pool.Enqueue(newUnit);
        }

        // 찾은 유닛을 리턴
        GameObject unit = pool.Dequeue();
        unit.SetActive(true);
        return unit;
    }

    public void ReturnUnit(GameObject go, int index)
    {
        go.SetActive(false);
        go.transform.SetParent(unitPoolParent);

        // 풀링이 되지 않은 오브젝트가 반환되어 오면 풀을 새로 추가
        if (!unitPools.ContainsKey(index))
            unitPools.Add(index, new Queue<GameObject>());

        unitPools[index].Enqueue(go);
    }
    #endregion

    #region 유닛 미리보기용 풀
    public GameObject GetGhost(int index)
    {
        // 유효하지 않은 인덱스는 리턴 null
        if (!objDataDic.TryGetValue(index, out var targetData))
        {
            Debug.Log("등록되지 않은 인덱스입니다.");
            return null;
        }

        // 해당 인덱스의 유닛 풀이 없을 경우 동적 생성
        if (!ghostUnitPools.TryGetValue(index, out var pool))
        {
            pool = new Queue<GameObject>();
            ghostUnitPools.Add(index, pool);
        }

        if (pool.Count == 0)
        {
            // ※프리팹이 UnitData에 지정되어 있는 경로에 있어야 함
            GameObject originPrefab = Resources.Load<GameObject>(targetData.objPoolPath);

            // 프리팹 로드 실패
            if (originPrefab == null)
            {
                Debug.Log($"프리팹 로드 실패 : 목표 인덱스 {index}");
                return null;
            }

            // 받아온 정보로 새로운 게임 오브젝트 생성 후 풀에 추가
            GameObject newUnit = Instantiate(originPrefab, ghostPoolParent);

            SetupGhostVisual(newUnit);

            newUnit.SetActive(false);
            pool.Enqueue(newUnit);
        }

        // 찾은 유닛을 리턴
        GameObject unit = pool.Dequeue();
        unit.SetActive(true);
        return unit;
    }
    public void ReturnGhost(GameObject go, int index)
    {
        go.SetActive(false);
        go.transform.SetParent(ghostPoolParent);

        // 풀링이 되지 않은 오브젝트가 반환되어 오면 풀을 새로 추가
        if (!ghostUnitPools.ContainsKey(index))
            ghostUnitPools.Add(index, new Queue<GameObject>());

        ghostUnitPools[index].Enqueue(go);
    }

    void SetupGhostVisual(GameObject ghost)
    {
        // 매터리얼 적용
        if (ghotsMaterial != null)
        {
            Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.material = ghotsMaterial;
        }

        // 컨트롤러 비활성화
        UnitCtrl uc = ghost.GetComponent<UnitCtrl>();
        if (uc != null)
            uc.enabled = false;

        // 충돌 판정 비활성화
        Collider coll = ghost.GetComponent<Collider>();
        if (coll != null)
            coll.enabled = false;

        // 물리 판정 비활성화
        Rigidbody rb = ghost.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // navMesh 비활성화
        NavMeshAgent nma = ghost.GetComponent<NavMeshAgent>();
        if(nma != null)
            nma.enabled = false;
    }
    #endregion
}
