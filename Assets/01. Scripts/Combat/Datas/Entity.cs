using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    public float curHp;
    public float curMana;
    protected UnitStats baseStats;

    [Header("Mine")]
    public Mine curTargetMine;

    protected IDamageable curTarget;

    // 애니메이션
    protected Animator anim;
    Tween rotTween;

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

    public UnitFaction Faction => faction;
    public Vector3 WorldPosition => transform.position;
    protected virtual void Awake()
    {
        cdtHandler = new ConditionHandler(this);
        anim = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void InitEntity(UnitStats a_Stats, UnitFaction a_Faction)
    {
        faction = a_Faction;
        baseStats = a_Stats;

        statHandler.SetBase(baseStats);
        curHp = a_Stats.maxHP;
        curMana = a_Stats.startMana;

        // 반지름 캐싱 들어가야 함
        radius = 0.5f;

        // 매터리얼 복구
        foreach (var r in renderers)
        {
            foreach (var m in r.materials)
                m.DOFade(1f, 0.1f);
        }

        statHandler.MarkDirty();

        if (ObjectManager.Inst != null)
            ObjectManager.Inst.RegistObject(this);
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
    #region Hangle AI
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
            curTargetMine = null;
            ExcuteCombat(deltaTime);
            return;
        }

        if (curTargetMine == null)
            SeachMine();

        if(curTargetMine != null)
        {
            ExcuteOccupy(deltaTime);
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

        if (curTarget != null && (curTarget as MonoBehaviour) != null)
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

    protected virtual void SeachMine()
    {
        var mines = ObjectManager.Inst.GetMineList();
        if (mines == null || mines.Count == 0) return;

        float sight = GetFinalStats().sight;
        Mine closest = null;
        float minDist = sight;

        foreach (var mine in mines)
        {
            if (mine.curFaction == this.Faction)
                continue;

            float dist = Vector3.Distance(transform.position,
                mine.transform.position);
            if(dist <= sight && dist < minDist)
            {
                minDist = dist;
                closest = mine;
            }
        }
        curTargetMine = closest;
    }

    void ExcuteOccupy(float deltaTime)
    {
        float dist = Vector3.Distance(transform.position,
            curTargetMine.transform.position)
            - curTargetMine.Radius  // 광산 반지름 빼고
            - Radius;               // 유닛 반지름 뺀 값이
        if(dist <= 0.01f)           // 0에 가까우면 점령
        {
            Vector3 targetPos = curTargetMine.transform.position;
            LookAtTarget(targetPos);

            // 점령 중에는 Idle 모션 출력
            if (anim != null)
                anim.SetFloat(hashMoveSpeed, 0f);
            // 점령 시도
            if (curTargetMine.TryOccupy(this))
                isOccupying = true;
            if (curTargetMine.curFaction == Faction)
                curTargetMine = null;

        }
        else
        {
            if (CanAction(ActionType.Move))
            {
                Vector3 dir = (curTargetMine.transform.position
                    - transform.position).normalized;
                transform.position += dir * GetFinalStats().moveSpeed * deltaTime;
                transform.forward = dir;
                if (anim != null)
                    anim.SetFloat(hashMoveSpeed, GetFinalStats().moveSpeed);
            }
        }
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

        Vector3 targetPos = (curTarget as MonoBehaviour).
            transform.position;
        LookAtTarget(targetPos);

        // 애니메이션 재생
        if (anim != null)
        {
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

        // CC기(컨디션 하위 항목) 체크
        foreach (var cdt in cdtHandler.ActiveCDTs)
            foreach (var f in cdt.Features)
                if (!f.CheckAction(action))
                    return false;

        return true;
    }



    float fadeTime = 1.5f;
    Renderer[] renderers;
    public void OnDie()
    {   
        // 매니저에 리스팅 해제
        if (ObjectManager.Inst != null)
            ObjectManager.Inst.UnregistObject(this);

        //// 콜라이더 비활성화
        //if (TryGetComponent(out Collider c))
        //    c.enabled = false;

        // 사망 애니메이션 출력
        if (anim != null)
            anim.SetTrigger(hashDie);

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
}
