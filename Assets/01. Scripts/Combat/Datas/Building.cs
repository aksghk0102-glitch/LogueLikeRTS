using UnityEngine;
using System;

public abstract class Building : MonoBehaviour, IDamageable
{
    [Header("Bulding Settings")]
    [SerializeField] protected UnitFaction faction;
    [SerializeField] protected float _maxHp = 500f;
    [SerializeField] protected float radius = 1.5f;

    [Header("Sound Key Settings")]
    protected string hitSfxKey = "Wood Impact 05";

    public float curHp { get; protected set; }
    public float maxHp => _maxHp;
    public Action OnDestroy;

    // IDamageable
    public bool IsAlive => curHp > 0;

    public float Radius => radius;
    public UnitFaction Faction => faction;
    public Vector3 WorldPosition => transform.position;

    // 이벤트 시스템
    public Action<float, float> OnHpChanged;    // curHp/maxHp 전달 : 체력 바 갱신용

    protected virtual void Awake()
    {
        curHp = maxHp;
    }

    // 설치 완료 시점에 초기화 로직 호출
    public virtual void Init(UnitFaction a_Faction)
    {
        faction = a_Faction;

        if (ObjectManager.Inst != null)
            ObjectManager.Inst.RegistObject(this);
    }

    public void TakeDamage(DamageInfo dmg)
    {
        if (!IsAlive) return;

        curHp -= dmg.Damage;
        OnHpChanged?.Invoke(curHp, maxHp);

        // 데미지 파티클 출력
        ParticleManager.inst.SpawnDmgTxt(dmg, transform.position);

        // 피격 사운드 출력
        SoundManager.inst.PlaySFX(hitSfxKey);
        OnHitEffect();

        if (curHp <= 0)
            OnDie();
    }

    const string particleKey = "Destroy_Barracks";
    public virtual void OnDie()
    {
        if (ObjectManager.Inst != null)
            ObjectManager.Inst.UnregistObject(this);

        // 파티클 출력
        ParticleManager.inst.SpawnParticle(particleKey, transform.position);

        // 건물을 지은 슬롯 초기화
        OnDestroy?.Invoke();
        OnDestroy = null;

        Destroy(gameObject);
    }

    protected virtual void OnHitEffect()
    {
        // 건물 피격 시 먼지 or 조각 파티클 생성 예정
    }
}
