using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 데미지 정보를 담을 클래스
public class DamageBuffer
{
    public IAttackable from;        // 공격자 정보
    public ITargetable to;          // 방어자 정보
    // 추후 기획에 따라 추가 가능
    // (ex. 치명타, 데미지 유형(단발, Dot 등...), 속성 등)
    public DamageBuffer(IAttackable a_From, ITargetable a_To)
    {
        from = a_From;
        to = a_To;
    }
}

public class Combat_Mgr : MonoBehaviour
{
    public static Combat_Mgr inst;

    [Header("Nexus")]
    public Transform playerNexus;
    public Transform enemyNexus;

    float tick = 0.1f;      // 한 틱
    float updateTimer;      // queue에 들어온 정보를 일정 간격 동안 동일한 타이밍에 일괄 처리

    Queue<DamageBuffer> dmgQueue = new Queue<DamageBuffer>();

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        // 0.1초마다 일괄 수행
        updateTimer -= Time.deltaTime;
        if(updateTimer <= 0f)
        {
            updateTimer = tick;

            // 데미지 큐 처리
            ExcuteDamageQueue();
        }
    }

    // 피격 판정 발생 구간마다 호출
    public void EnqueueDamage(DamageBuffer buffer)
    {
        if (buffer != null)
        {
            dmgQueue.Enqueue(buffer);
        }
    }

    void ExcuteDamageQueue()
    {
        while (dmgQueue.Count > 0)
        {
            DamageBuffer buffer = dmgQueue.Dequeue();
            SetDamage(buffer);
        }
    }

    void SetDamage(DamageBuffer buffer)
    {
        if(buffer.from == null ||  buffer.to == null)
        {
            Debug.Log("데미지 큐 리턴 됨");
            return;
        }
        // 공격력 
        int finalDmg = buffer.from.AttackPower;

        // 데미지 계산
        // 방어력, 데미지 감소 로직, 멀티플라이어 등이 필요하면 여기에 추가

        // 데미지 적용
        buffer.to.TakeDamage(finalDmg);

        // 사망 시 효과, 로그 등이 필요하면 여기에 추가
        // if(buffer.to.IsDie)
    }
}
