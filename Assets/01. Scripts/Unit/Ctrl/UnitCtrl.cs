using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 변경해야 할 점 메모
// 1) UnitState_Move > 이동 방식 수정
// 2) 애니메이션 추가 부분(UnitCtrl에서 애니메이션 전환 함수 만들고 상태 전환 시 호출)
// 3) 스킬 효과 어떤 효과 어떻게 추가할지?
// 
// 

public class UnitCtrl : MonoBehaviour, ITargetable, IAttackable
{
    public UnitData baseData;

    // --- 유닛 데이터 캐싱용 변수 ---
    [HideInInspector] public string UnitName;
    [HideInInspector] public UnitType UnitType;
    [HideInInspector] public float MoveSpeed;
    [HideInInspector] int Index;
    int curHp, maxHp;
    int attackPower;
    float attackRange, attackSpeed, sight;
    // --- 유닛 데이터 캐싱용 변수 ---

    // 상태 플래그
    public bool isAttacking = false;
    public bool isMoving = false;
    public bool isDie = false;

    public IUnitState CurState { get; private set; }    // 읽기 전용
    Dictionary<UnitState, IUnitState> states;           // 상태 정의 캐싱
    [SerializeField]UnitState m_CurState = UnitState.MoveAndSearch;

    // 이동 관련 변수
    public Vector3 MoveDir = Vector3.zero;
    public Vector3 LastMoveDir = Vector3.zero;  // 애니메이션 적용 시 방향 조정에 활용할 변수
    NavMeshAgent navMeshAgent;

    // 타겟 설정을 위한 변수
    [HideInInspector]public ITargetable curTarget;
    LayerMask unitLayerMask;
    UnitFaction faction; // 소환될 때 SetFaction 함수를 호출해 설정
    Collider coll;      // 충돌 판정 컴포넌트
    float radius;       // 피격 판정 범위 캐싱

    // 렌더링  -> 후에 모델링 속성에 따라 수정
    Renderer[] renderers;       // 모델이 갖고 있는 렌더러 저장
    Color[] originColors;       // 렌더러의 원본 색 저장

    // ITargetAble 인터페이스 구현
    [HideInInspector] public int MaxHp => maxHp;
    [HideInInspector] public int CurHp => curHp;
    [HideInInspector] public float Radius => radius;
    [HideInInspector] public Transform Transform => this.transform;
    public bool IsDie => isDie;
    [HideInInspector] public int AttackPower => attackPower;
    [HideInInspector] public float AttackRange => attackRange;
    [HideInInspector] public float AttackSpeed => attackSpeed;
    [HideInInspector] public float Sight => sight;

    public UnitFaction Faction => faction;

    private void Awake()
    {
        InitBaseStats();
        RegisterStates();

        coll = GetComponent<Collider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if(navMeshAgent == null)
            navMeshAgent =gameObject.AddComponent<NavMeshAgent>();

        renderers = GetComponentsInChildren<Renderer>();
        originColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = new Material(renderers[i].material);
            originColors[i] = renderers[i].material.GetColor("_Color");
        }

        ChangeState(UnitState.MoveAndSearch);
    }

    void Update()
    {
        // 각 상태에 맞는 Update 로직 실행
        CurState?.OnUpdate(this);
    }

    #region 데이터 캐싱
    // SO에서 받아온 능력치를 초기화합니다.
    void InitBaseStats()
    {
        //// SO 파일 읽어오기
        //UnitName = baseData.unitName;
        //UnitType = baseData.unitType;
        //curHp = baseData.hp;
        //MoveSpeed = baseData.moveSpeed;
        //attackPower = baseData.attackPower;
        //attackSpeed = baseData.attackSpeed;
        //attackRange = baseData.attackRange;
        //sight = baseData.sight;
        //
        //// 타겟 레이어 설정
        //unitLayerMask = 1 << LayerMask.NameToLayer("Unit");
        //
        //// 피격 범위 계산
        //radius = GetRadius();
        //
        //// navMeshAgent에 설정값 할당
        //if (navMeshAgent!= null)
        //{
        //    navMeshAgent.speed = baseData.moveSpeed;
        //    navMeshAgent.radius = radius * 0.9f;
        //    navMeshAgent.stoppingDistance = AttackRange - 0.05f;
        //    navMeshAgent.updateRotation = true;
        //}
    }

    void RegisterStates()
    {
        states = new Dictionary<UnitState, IUnitState>() {
            { UnitState.MoveAndSearch, new UnitState_Move()},
            { UnitState.Chase, new UnitState_Chase()},
            { UnitState.Attack, new UnitState_Attack()},
            { UnitState.Die, new UnitState_Die()},
            { UnitState.Idle, new UnitState_Move()},
        };
    }
    #endregion

    #region 변수 설정
    public void ChangeState(UnitState newState)
    {
        // 중복 호출 회피
        if (m_CurState == newState && CurState != null)
            return;

        // 상태를 빠져나올 때 OnExit에 있는 로직 실행
        if (CurState != null)
            CurState.OnExit(this);

        // 유효하지 않은 키 값을 받으면 강제 초기화
        if (!states.ContainsKey(newState))
            newState = UnitState.MoveAndSearch;

        // 상태 변경 및 OnEnter 실행
        CurState = states[newState];
        m_CurState = newState;
        CurState.OnEnter(this);
    }

    //public void SetMovement(Vector3 dir)
    //{
    //    MoveDir = dir;
    //}

    public void StopNav()
    {
        // navMeshAgent를 정지
        if (isDie || navMeshAgent == null) return;

        navMeshAgent.isStopped = true;
        isMoving = false;
    }
    public void MoveTo(Vector3 destination)
    {
        // navMeshAgent의 목적지를 할당하고 이동 플래그를 활성화
        if (isDie || navMeshAgent == null) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
        isMoving = true;
    }

    public bool CheckNavPath
    {
        get
        {
            if (navMeshAgent == null)
                return true;
            return !navMeshAgent.pathPending &&
                navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
                (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
        }
    }
    public void Init(UnitFaction a_Faction)
    {
        // 메모
        // 빠른 구현을 위해 외부에서 설정하는 방식으로 만들었지만
        // 전투 매니저를 만들어 소속마다 리스트에 할당하는 방식으로 변경하면 좋을 듯 -> 성능 이슈 생기면 고려

        faction = a_Faction;
        //curHp = baseData.hp;
        ChangeState(UnitState.MoveAndSearch);
        isDie = false;
        coll.enabled = true;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetColor("_Color", originColors[i]);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        // 스폰 시 위치 할당
        if(navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.Warp(pos);
        }
        else
        {
            transform.position = pos;
        }

    }

    float GetRadius()
    {
        if (coll == null)
            return 0.5f;

        if (coll is SphereCollider sc)
            return sc.radius;
        if (coll is CapsuleCollider cc)
            return cc.radius;
        if (coll is BoxCollider bc)
            return Mathf.Max(bc.size.x, bc.size.z) / 2f;

        return 0.5f;        // 기본값
    }
    #endregion

    public void TakeDamage(int dmg)
    {
        // 이미 사망한 상태면 리턴
        if (isDie ||
            m_CurState == UnitState.Die)
            return;

        // 최소한만 구현
        curHp -= dmg;

        // 메모 - 성능과 안정성(동시에 공격을 보장하지 못한다던가...) 생각하면,
        // 전투 매니저에 이벤트를 호출시켜 Queue에 담아 일괄 처리하는 게 유리함

        // 피격 효과 (임시)
        StartCoroutine(HitEffect());

        if(curHp <= 0)
        {
            ChangeState(UnitState.Die);
        }
    }

    IEnumerator HitEffect()
    {
        // 색 변경
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetColor("_Color", Color.red);
        }

        float t = 0.25f;
        yield return new WaitForSeconds(t);

        // 색 복구
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetColor("_Color", originColors[i]);
        }
    }
    public void Die()
    {
        coll.enabled = false;
        isDie = true;

        // 사망 애니메이션 추가해야 함

        //Debug.Log(baseData.unitUniqIndex);
        //if (UnitSpawn_Mgr.inst != null)
        //    UnitSpawn_Mgr.inst.ReturnUnit(gameObject, baseData.unitUniqIndex);
        //else
        //    Destroy(gameObject);
    }
    // tools
    public bool SearcForTarget()
    {
        // 시야 범위 내 모든 콜라이더 탐색
        Collider[] colls = Physics.OverlapSphere(transform.position, Sight, unitLayerMask);

        ITargetable closestTarget = null;
        float closestDist = float.MaxValue;

        foreach (var coll in colls)
        {
            // 자기 자신은 제외
            if (coll.transform == this.transform)
                continue;

            ITargetable target = coll.GetComponent<ITargetable>();

            // 타겟이 없거나, Die 상태거나, 같은 소속이면 리턴
            if (target == null)
                continue;
            if (target.IsDie || faction == target.Faction)
                continue;

            // 최단 거리 계산
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.Transform.position;

            myPos.y = 0; targetPos.y = 0;       // 이 부분은 기획에 따라 달라질 것 같은데,
                                                // 일단은 성능을 고려해서 평면상의 거리만 검사

            float dist = Vector3.Distance(myPos, targetPos);

            if(dist < closestDist)
            {
                closestDist = dist;
                closestTarget = target;
            }
        }//foreach문 종료

        // 타겟을 할당 및 true 반환
        if(closestTarget != null)
        {
            curTarget = closestTarget;
            return true;
        }

        // 유효한 타겟이 없었으므로 false 반환
        curTarget = null;
        return false;
    }

    public float GetTargetDistace()
    {
        // 타겟과의 거리 계산
        Vector3 unitPos = transform.position;
        Vector3 targetPos = curTarget.Transform.position;

        // y축을 배제한 거리로 계산 (유닛 모델링 변경이나 설정 따라 다를 수 있으므로)
        unitPos.y = 0;
        targetPos.y = 0;

        // 경계와 경계 사이 거리 계산
        float dist_C2C = Vector3.Distance(unitPos, targetPos);  // Center to Center
        float dist_E2E = dist_C2C - curTarget.Radius - this.Radius;           // Center to Edge

        return dist_E2E;
    }

    public bool IsValidTarget()
    {
        // 타겟이 유효한 지 검사
        if (curTarget == null)
            return false;
        if (curTarget.IsDie || curTarget.Faction == faction)
            return false;

        return true;
    }
}
