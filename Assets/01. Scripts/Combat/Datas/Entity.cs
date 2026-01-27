using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
//
// 역할: 전투에 참여하는 유닛의 행동을 정의합니다. 인터페이스 상속으로 논리적 규칙을 강제합니다.
// 사용 방법: 유닛 오브젝트에 컴포넌트를 붙여 사용합니다.
// 
public class Entity : MonoBehaviour,
    IDamageable, IAttacker, ICastSkill, IStatCalc
{
    [Header("Information")]
    [SerializeField] UnitFaction faction;     // 소속 분류
    public UnitClassType Class;     // 클래스 분류 (바 나 로 레 매)
    float radius;

    // 핸들러
    protected StatHandler statHandler = new StatHandler();
    protected ConditionHandler cdtHandler;

    // 현재 상태
    [Header("CombatData")]
    public float curHp { get; protected set; }
    public float maxHp => GetFinalStats().maxHP;
    public float curMana { get; protected set; }
    //protected UnitStats baseStats;

    // 사운드 키 값 캐싱
    protected string hitSfxKey;
    protected string attSfxKey;
    protected string dieSfxKey;

    protected IDamageable curTarget { get; private set; }

    // 애니메이션
    protected Animator anim;
    Tween rotTween;

    protected readonly int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashDie = Animator.StringToHash("Die");
    protected readonly int hashVictory = Animator.StringToHash("Victory");

    bool canMove = false;

    // 공격 속도
    protected float lastAttackTime;

    // 스킬
    protected SkillData activeSkill;
    protected List<SkillData> passiveSkill = new List<SkillData>();

    // 프로퍼티
    public float Radius => radius;               // 유닛 충돌 반경
    public bool IsAlive => curHp > 0;
    public bool isSkillCasting { get; private set; }    // 스킬 모션 체크
    public bool isAttacking { get; private set; }       // 평타 모션 체크

    public UnitFaction Faction => faction;
    public Vector3 WorldPosition => transform.position;
    // 마나바 동기화를 위한 능력치 프로퍼티
    public float MaxMana => GetFinalStats().maxMana;
    public int ID { get; private set; }

    protected virtual void Awake()
    {
        cdtHandler = new ConditionHandler(this);
        anim = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void InitEntity(UnitDataSO data, UnitFaction a_Faction
        , int bLevel = 1, int a_ID = -1, UnitSkillSet skills = null)
    {
        faction = a_Faction;
        ID = a_ID;


        UnitStats a_Stats = data.stats;
        statHandler.SetBase(a_Stats, bLevel);       // 유닛 스탯 초기화

        UnitStats final = statHandler.GetStaticStats();
        curHp = final.maxHP;
        curMana = final.startMana;

        // 반지름 캐싱 들어가야 함
        radius = 0.5f;

        // 매터리얼 복구
        foreach (var r in renderers)
            foreach (var m in r.materials)
                m.DOFade(1f, 0.1f);

        // 사운드 키 값 캐싱
        hitSfxKey = data.hitSfxKey;
        attSfxKey = data.attSfxKey;
        dieSfxKey = data.dieSfxKey;
        
        // 스킬 할당
        if(skills != null)
            SetUpSkills(skills);

        // 스탯 초기화
        statHandler.MarkDirty();

        // 오브젝트 매니저에 등록해서 관리
        if (ObjectManager.Inst != null)
            ObjectManager.Inst.RegistObject(this);
    }

    void SetUpSkills(UnitSkillSet skillSet)
    {
        // 액티브 스킬 할당
        if (!string.IsNullOrEmpty(skillSet.ActiveID))
        {
            activeSkill = InventoryManager.inst.GetSkillData(skillSet.ActiveID);

            Debug.Log($"{gameObject.name} 장착됨 : {skillSet.ActiveID}");
        }

        // 패시브 스킬 할당
        passiveSkill.Clear();
        foreach(var p in skillSet.PassiveID)
        {
            if (string.IsNullOrEmpty(p))
                continue;

            var pData = InventoryManager.inst.GetSkillData(p);
            if(pData != null)
                passiveSkill.Add(pData);
        }
    }
    // 매 프레임 체크 => 오브젝트 매니저에서 호출
    public virtual void OnUpdate(float deltaTime)
    {
        if (!IsAlive) return;

        // 컨디션 업데이트 (도트 데미지, 지속시간 만료 등)
        cdtHandler.OnUpdate(deltaTime);


        UpdateMana(deltaTime);

        // 최대 마나 도달 시 스킬 시전 => 스킬 할당 로직 구현 한 후 활성화할 것...
        var stats = GetFinalStats();
        if (curMana >= stats.maxMana
            && CanAction(ActionType.Skill)
            && activeSkill != null)
        {
            TryUseActiveSkill();
            return;
        }

        //
        HandleAIProcess(deltaTime);
    }
    void UpdateMana(float deltaTime)
    {
        // 마나 회복 처리
        var stats = GetFinalStats();
        if (stats.manaRegen > 0
            && curMana < stats.maxMana)
        {
            curMana = Mathf.Min(curMana + (stats.manaRegen * deltaTime), stats.maxMana);
        }
    }
    #region Hangle AI
    protected virtual void HandleAIProcess(float deltaTime)
    {
        if (curTarget != null && !curTarget.IsAlive)
        {
            curTarget = null;
            isAttacking = false;
            isSkillCasting = false;
            if (anim != null)
                anim.SetInteger(hashAttack, 0);
        }

        // 다른 행동 중이면 리턴
        if (isSkillCasting || isAttacking)
            return;

        // 타겟 유효성 검사 및 재탐색
        if (curTarget == null)
            SearchTarget();
        
        // 타겟 지정 후 공격
        if (curTarget != null)
        {
            ExcuteCombat(deltaTime);
            return;
        }
        else
        {
            // 이동
            if (CanAction(ActionType.Move) && !isAttacking)
                Move(deltaTime);
        }
    }

    void ExcuteCombat(float deltaTime)
    {
        if (!isAttacking)
            SearchTarget();

        float dist = Vector3.Distance(transform.position,
                (curTarget.WorldPosition));
        float validRange = GetFinalStats().attRange + Radius
            + curTarget.Radius;

        if (dist <= validRange)
        {
            if (CanAction(ActionType.Attack))
                Attack();
        }
        else
        {
            if (CanAction(ActionType.Move) && !isAttacking)
                Move(deltaTime);
        }
    }
    public Vector3 moveDir => faction == UnitFaction.Player ?
        Vector3.right : Vector3.left;
    protected virtual void SearchTarget()
    {
        curTarget = null;

        // 적 진영 리스트 받아오기
        var enemies = ObjectManager.Inst.GetEnemyList(this.Faction);
        if (enemies == null || enemies.Count == 0)
            return;

        float sight = GetFinalStats().sight;
        IDamageable closest = null;
        float minDist = sight;
        Vector3 myPos = transform.position;

        // 모든 적 유닛 탐색 후 시야 내에 있는지 확인
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;

            float dist2Centor = Vector3.Distance(myPos, enemy.WorldPosition);
            float dist2Surface = dist2Centor - enemy.Radius;
            if (dist2Surface <= sight && dist2Surface < minDist)
            {
                minDist = dist2Surface;
                closest = enemy;
            }
        }

        // 타겟 할당
        curTarget = closest;
    }
    protected virtual void Move(float deltaTime)
    {
        float speed = GetFinalStats().moveSpeed;
        Vector3 targetPos;

        if (curTarget != null && curTarget.WorldPosition != null)
        {
            targetPos = curTarget.WorldPosition;
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += dir * speed * deltaTime;

            transform.forward = dir;
        }
        else
        {
            transform.position += moveDir * speed * deltaTime;
            transform.forward = moveDir;
        }

        // 애니메이션 처리
        if (anim != null)
            anim.SetFloat(hashMoveSpeed, speed);
    }
    #endregion

    float CalculateDamage(DamageInfo dmg)
    {
        var stats = GetFinalStats();
        float finalDmg = dmg.Damage;

        switch (dmg.type)
        {
            case DamageType.Physics:
                finalDmg -= stats.defense;
                break;
            case DamageType.Magic:
                finalDmg -= stats.magicResist;
                break;
            case DamageType.True:
                break;
        }

        return Mathf.Max(1, finalDmg);
    }

    public void TakeDamage(DamageInfo dmg)
    {
        // 사망 상태 체크
        if (!IsAlive)
            return;

        // 힐 체크 + 치감 적용
        if(dmg.type == DamageType.Heal)
        {
            float amount = dmg.Damage;
            if (cdtHandler.HasTag(CDT_Tag.LowHeal))
                amount *= 0.5f;
            curHp = Mathf.Min(curHp+amount, GetFinalStats().maxHP);
        
            return;
        }

        // 무적 체크
        if (cdtHandler.HasTag(CDT_Tag.Invincivle))
            return;

        // 피격 사운드 출력
        if (!string.IsNullOrEmpty(hitSfxKey))
            SoundManager.inst.PlaySFX(hitSfxKey);

        // 데미지 파티클, 피격 파티클 출력
        ParticleManager.inst.SpawnDmgTxt(dmg, transform.position);
        ParticleManager.inst.SpawnParticle("hit_physics", transform, 0.5f);

        // 컨디션 이벤트 개입
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                f.OnBattleEvent(this, ref dmg);

        // 체력 계산
        float finalDmg = CalculateDamage(dmg);
        curHp -= finalDmg;

        // 통계 기록
        if (CombatManager.Inst != null)
        {
            if(finalDmg > 0)
            {
                // DamageInfo를 통해 들어온 공격자 정보 기록
                CombatManager.Inst.RecordDealt(dmg.Attker.ID, finalDmg, dmg.type);

                // 이 유닛이 받은 피해량 기록 : 이 유닛의 ID, 원본 데미지, 최종 받은 데미지
                CombatManager.Inst.RecordRecieved(ID, dmg.Damage, finalDmg, dmg.type);
            }

        }

        // 사망 및 부활 체크
        if (curHp <= 0)
            OnDie();
    }

  
    public virtual void OnVictory()
    {
        if (anim != null)
            anim.SetTrigger(hashVictory);
    }
    // IStatCalc
    public UnitStats GetFinalStats()
    {
        return statHandler.GetFinalStats(cdtHandler.ActiveCDTs, this);
    }
    // 스탯 재계산 시 호출
    public void MarkDirty() => statHandler.MarkDirty();

    //IAttacker
    public void OnHit(IDamageable target, DamageInfo info)
    {
        // 마나 수급
        var stats = GetFinalStats();
        curMana = Mathf.Min(curMana + stats.manaGet, stats.maxMana);

        // 온힛 효과 적용
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                f.OnBattleEvent(this, ref info);
    }

    #region Animation Event
    public virtual void OnAttackEvent()
    {
        // 애니메이션 이벤트에서 호출
        CombatManager.Inst.EnqueueDamage(CreateDamagaInfo());
    }
    public void EndAttack()
    {
        //Debug.Log("Attack End");
        isAttacking = false;

        if (isSkillCasting)
            return;

        if (anim != null)
        {
            anim.speed = 1f;
            anim.SetInteger(hashAttack, 0);
        }
    }

    public virtual void OnSkillEvent()
    {
        if (activeSkill != null && activeSkill.LogicInstance != null)
        {
            Debug.Log("스킬 적용!");
            activeSkill.LogicInstance.Execute(this, curTarget);
        }
    }
    public void EndSkill()
    {
        Debug.Log("Skill End");
        EndSkillCast();

        if(anim != null)
        {
            anim.SetInteger(hashAttack, 0);
            anim.speed = 1f;
        }
    }

    protected virtual void TryUseActiveSkill()
    {
        if (activeSkill == null || isSkillCasting)
        {
            Debug.Log($"{gameObject.name} : 사용할 수 있는 스킬이 없습니다.");
            return;
        }

        curMana = 0;
        StartSkillCast();
        Debug.Log("시전 시작");

        if (curTarget != null)
            LookAtTarget(curTarget.WorldPosition);

        // 애니메이션 재생
        if (anim != null)
        {
            Debug.Log(activeSkill.MotionType);
            if (1 <= activeSkill.MotionType && activeSkill.MotionType <= 4)
               anim.SetInteger(hashAttack, activeSkill.MotionType);
    
            anim.SetFloat(hashMoveSpeed, 0f);
        }

        // 디버그용 임시 코드
        DOVirtual.DelayedCall(2f, () => {
            Debug.Log("강제 종료 테스트");
            EndSkill();
        });
    }
    #endregion
    protected DamageInfo CreateDamagaInfo()
    {
        var stats = GetFinalStats();
        DamageInfo dmg = new DamageInfo
        {
            Attker = this,
            Target = curTarget,
            Damage = stats.attack,
            Source = DamageSource.Default,
            type = Class == UnitClassType.Mage ? DamageType.Magic : DamageType.Physics,
            MetaData = new Dictionary<string, float>()
        };

        // 치명타 연산
        if (Random.value <= stats.critChance * 0.01f)
        {
            dmg.IsCritical = true;
            dmg.Damage = dmg.Damage * stats.critDamage;
        }

        //Debug.Log(dmg.Damage);
        return dmg;
    }

    public void AttackerCDT(ref DamageInfo dmg)
    {
        // 공격 시 컨디션 개입
        // 컴뱃 매니저에서 호출해줘야 함!
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                f.OnBattleEvent(this, ref dmg);
    }

    protected virtual void Attack()
    {
        if (isAttacking) return;

        // 타겟 방향을 회전
        if (curTarget != null)
        {
            Vector3 targetPos = curTarget.WorldPosition;
            LookAtTarget(targetPos);
        }

        // 공격 속도 계산
        var stats = GetFinalStats();
        float attDelay = 1f / Mathf.Max(0.01f, stats.attSpeed);

        if (Time.time - lastAttackTime < attDelay)
            return;

        isAttacking = true;
        lastAttackTime = Time.time;

        // 공격 사운드 출력
        if (!string.IsNullOrEmpty(attSfxKey))
            SoundManager.inst.PlaySFX(attSfxKey);

        // 애니메이션 재생
        if (anim != null)
        {
            // 공격 속도에 따라 공격 모션 속도 제어
            anim.speed = Mathf.Max(1f, stats.attSpeed);

            anim.SetInteger(hashAttack, 1);
            anim.SetFloat(hashMoveSpeed, 0f);
        }
    }

    float rotTime = 0.3f;
    protected void LookAtTarget(Vector3 targetPos)
    {
        // 닷트윈을 활용해 타겟을 향해 부드럽게 회전
        Vector3 dir = (targetPos - transform.position).normalized;
        if(dir != Vector3.zero)
        {
            dir.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            rotTween?.Kill();
            rotTween = transform
                .DORotate(targetRot.eulerAngles, rotTime)
                .SetEase(Ease.OutQuad);
        }
    }

    // 임시 변수...
    

    // ICastSkill
    public void StartSkillCast() => isSkillCasting = true;
    public void EndSkillCast() => isSkillCasting = false;

    // 행동 가능 여부 체크
    public bool CanAction(ActionType action)
    {
        // 사망 시
        if (!IsAlive)
            return false;

        // 스킬 시전 중 평타/이동 금지
        if (isSkillCasting &&
            (action == ActionType.Attack || action == ActionType.Move))
            return false;

        if (!canMove)
            return false;

        // CC기(컨디션 하위 항목) 체크
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                if (!f.CheckAction(action))
                    return false;

        return true;
    }



    float fadeTime = 4f;
    Renderer[] renderers;
    public void OnDie()
    {
        if (curHp > 0)
            return;

        curHp = -1;         // 확실한 사망 판정을 위해서

        // 매니저에 리스팅 해제
        if (ObjectManager.Inst != null)
            ObjectManager.Inst.UnregistObject(this);

        curTarget = null;
        isAttacking = false;
        isSkillCasting = false;
        canMove = false;

        // 사망 사운드 출력
        if (!string.IsNullOrEmpty(dieSfxKey))
            SoundManager.inst.PlaySFX(dieSfxKey);

        // 사망 애니메이션 출력
        if (anim != null)
        {
            anim.speed = 1f;
            anim.SetTrigger(hashDie);
        }

        // 사망 연출
        foreach (var r in renderers)
        {
            foreach (var m in r.materials)
                m.DOFade(0f, fadeTime);
        }

        DOVirtual.DelayedCall(fadeTime, () =>
        {
            gameObject.SetActive(false);
        });
        
        // 닷트윈 정리
        rotTween?.Kill();
    }

    public void SetMoveable()
    {
        canMove = true;
    }
}
