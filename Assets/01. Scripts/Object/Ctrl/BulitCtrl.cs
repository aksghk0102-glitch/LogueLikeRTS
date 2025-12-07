using System;
using UnityEngine;

public class BulitCtrl : MonoBehaviour, ITargetable
{
    [SerializeField]UnitFaction faction;

    [SerializeField] int curHp = 100;
    [SerializeField] int maxHp = 100;
    bool isDie = false;
    [SerializeField]float radius = 1f;

    Collider coll;

    // ITargetable
    public int CurHp => curHp;
    public int MaxHp => maxHp;
    public float Radius => radius;
    public bool IsDie => isDie;
    public Transform Transform => this.transform;
    public UnitFaction Faction => faction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coll = GetComponent<Collider>();
        radius = GetRadius();
    }

    float GetRadius()
    {
        if (coll == null)
            return 1f;    // 기본값

        if (coll is SphereCollider sc)
            return sc.radius * transform.lossyScale.x;
        if (coll is CapsuleCollider cc)
            return cc.radius * transform.lossyScale.x;
        if (coll is BoxCollider bc)
            return Mathf.Max(bc.size.x * transform.localScale.x,
                bc.size.z * transform.localScale.y) / 2f;

        return 1f;        // 기본값
    }
    public void TakeDamage(int dmg)
    {
        // 이미 사망한 상태면 리턴
        if (isDie || curHp <= 0)
            return;

        // 최소한만 구현
        curHp -= dmg;

        if(curHp <= 0 && Game_Mgr.Inst != null)
        {
            // 파괴 되면 게임 종료... 임시 호출
            Game_Mgr.Inst.IsGameOver();
        }
    }
}
