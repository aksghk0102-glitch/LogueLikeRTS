using UnityEngine;
using System;

public abstract class Building : MonoBehaviour, IDamageable
{
    [Header("Bulding Settings")]
    [SerializeField] protected UnitFaction faction;
    [SerializeField] protected float maxHp = 500f;
    [SerializeField] protected float radius = 1.5f;

    public float curHp { get; protected set; }
    public Action OnDestroy;

    // IDamageable
    public bool IsAlive => curHp > 0;

    public float Radius => radius;
    public UnitFaction Faction => faction;
    public Vector3 WorldPosition => transform.position;

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

        curHp = Mathf.Max(0, curHp - dmg.Damage);
        OnHitEffect();

        if (curHp <= 0)
            OnDie();
    }

    public virtual void OnDie()
    {
        if (ObjectManager.Inst != null)
            ObjectManager.Inst.UnregistObject(this);

        // 건물을 지은 슬롯 초기화
        OnDestroy?.Invoke();
        OnDestroy = null;

        // 풀링 들어가면 수정
        Destroy(gameObject);
    }

    protected virtual void OnHitEffect()
    {
        // 건물 피격 시 먼지 or 조각 파티클 생성 예정
    }
}
