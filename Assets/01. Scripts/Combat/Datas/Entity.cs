using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 전투 객체 베이스 데이터 정의
public class Entity : MonoBehaviour,
    IDamageable, IAttacker, ICastSkill, IStatCalc
{
    [Header("Information")]
    public UnitFaction Faction;     // 소속 분류
    public UnitClassType Class;     // 클래스 분류 (바 나 로 레 매)
    float radius;
    // 핸들러
    protected StatHandler statHandler = new StatHandler();
    protected ConditionHandler cdtHandler;

    // 현재 상태
    [Header("CombatData")]
    public float curHp;
    public float curMana;
    protected UnitStats baseStats;

    protected IDamageable curTarget;

    // 애니메이션
    protected Animator anim;

    protected readonly int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashDie = Animator.StringToHash("Die");
    protected readonly int hashVictory = Animator.StringToHash("Victory");


    // 프로퍼티
    public float Radius => radius;               // 유닛 충돌 반경
    public bool IsAlive => curHp > 0;
    public bool isSkillCasting { get; private set; }    // 스킬 모션 체크
    public bool isAttacking { get; private set; }       // 평타 모션 체크
    public bool isOccupying { get; set; }               // 점령 중 체크

    protected virtual void Awake()
    {
        cdtHandler = new ConditionHandler(this);
        anim = GetComponent<Animator>();
    }

    public void InitEntity(UnitStats stats, UnitFaction faction)
    {
        Faction = faction;
        baseStats = stats;

        statHandler.SetBase(baseStats);
        curHp = stats.maxHP;
        curMana = stats.startMana;

        // 반지름 캐싱 들어가야 함
        radius = 0.5f;

        statHandler.MarkDirty();

        if (UnitManager.Inst != null)
            UnitManager.Inst.RegistUnit(this);
    }
    // 매 프레임 체크
    public virtual void OnUpdate(float deltaTime)
    {
        if (!IsAlive) return;

        // 컨디션 업데이트 (도트 데미지, 지속시간 만료 등)
        cdtHandler.OnUpdate(deltaTime);
        UpdateMana(deltaTime);
        // 마나 100 도달 시 스킬 시즌
        
        if (curMana >= baseStats.maxMana && CanAction(ActionType.Skill))
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

    protected virtual void HandleAIProcess(float deltaTime)
    {
        if (curTarget != null && !curTarget.IsAlive)
            curTarget = null;

        // 다른 행동 중이면 리턴
        if (isOccupying || isSkillCasting || isAttacking)
            return;

        // 타겟 유효성 검사 및 재탐색
        if (curTarget == null || !curTarget.IsAlive)
            SearchTarget();

        // 타겟 지정 후 공격
        if(curTarget != null)
        {
            float dist = Vector3.Distance(transform.position,
                (curTarget as MonoBehaviour).transform.position);
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
        else
        {
            // 타겟이 없을 때에도 이동
            if (CanAction(ActionType.Move) && !isAttacking)
                Move(deltaTime);
        }
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
            curHp = Mathf.Min(amount, GetFinalStats().maxHP);
            return;
        }

        // 무적 체크
        if (cdtHandler.HasTag(CDT_Tag.Invincivle))
            return;

        // 컨디션 이벤트 개입
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                f.OnBattleEvent(this, ref dmg);

        // 체력 계산
        float finalDmg = CalculateDamage(dmg);
        curHp = Mathf.Clamp(curHp- finalDmg, 0, GetFinalStats().maxHP);

        // 사망 및 부활 체크
        if (curHp <= 0)
            OnDie();
    }

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

    public virtual void OnAttackEvent()
    {
        //if (curTarget == null || !curTarget.IsAlive)
        //{
        //    EndAttack();
        //    return;
        //}
        //
        //ProcessDamage(curTarget, dmg);
        CombatManager.Inst.EnqueueDamage(CreateDamagaInfo());

        //if (!curTarget.IsAlive)
        //    EndAttack();
    }

    protected DamageInfo CreateDamagaInfo()
    {
        var stats = GetFinalStats();
        DamageInfo dmg = new DamageInfo
        {
            Attker = this,
            Target = curTarget,
            Damage = stats.attack,
            Source = DamageSource.Default,
            type = DamageType.Physics,
            MetaData = new Dictionary<string, float>()
        };

        // 치명타 연산
        if (Random.value <= stats.critChance * 0.01f)
        {
            dmg.IsCritical = true;
            dmg.Damage = dmg.Damage * stats.critDamage;
        }

        Debug.Log(dmg.Damage);
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

        isAttacking = true;

        // 애니메이션 재생
        if(anim != null)
        {
            anim.SetInteger(hashAttack, 1);
            anim.SetFloat(hashMoveSpeed, 0f);
        }
    }
    public void EndAttack()
    {
        Debug.Log("Attack End");
        isAttacking = false;

        if (anim != null)
        {
            anim.SetInteger(hashAttack, 0);
        }
    }
    protected virtual void TryUseActiveSkill()
    {
        curMana = 0;
        StartSkillCast();
        
        // 애니메이션 재생
        if (anim != null)
        {
            // 스킬에 할당된 모션 나오게 수정 예정
            anim.SetInteger(hashAttack, 2);
            anim.SetFloat(hashMoveSpeed, 0f);
        }
    }

    // 임시 변수...
    protected Vector3 moveDir = Vector3.right;
    protected virtual void SearchTarget()
    {
        // 적 진영 리스트 받아오기
        var enemies = UnitManager.Inst.GetEnemyList(this.Faction);
        if (enemies == null || enemies.Count == 0)
            return;

        float sight = GetFinalStats().sight;
        Entity closest = null;
        float minDist = sight;
        Vector3 myPos = transform.position;

        // 모든 적 유닛 탐색 후 시야 내에 있는지 확인
        foreach(var enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;

            float dist = Vector3.Distance(myPos, enemy.transform.position);
            if(dist <= sight && dist < minDist)
            {
                minDist = dist;
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

        if(curTarget != null && (curTarget as MonoBehaviour) != null)
        {
            targetPos = (curTarget as MonoBehaviour).transform.position;
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

        // CC기(컨디션 하위 항목) 체크
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                if (!f.CheckAction(action))
                    return false;

        return true;
    }
    protected void OnDie()
    {   
        // 매니저에 리스팅 해제
        if (UnitManager.Inst != null)
            UnitManager.Inst.UnregistUnit(this);

        // 사망 애니메이션 출력
        if (anim != null)
            anim.SetTrigger(hashDie);
    }
}
