using UnityEngine;

public class Projectile : MonoBehaviour
{
    DamageInfo dmgInfo;
    IDamageable target;
    [SerializeField] float speed = 10f;
    bool isInit = false;

    public void Init(IDamageable a_Target, DamageInfo a_DmgInfo, float a_Speed)
    {
        target = a_Target;
        dmgInfo = a_DmgInfo;
        speed = a_Speed;
        isInit = true;

        // 임시 코드 => 풀링 구현 후 수정
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInit || target == null || !target.IsAlive)
        {
            // 여기도 풀링 구현 후 수정
            Destroy(gameObject);
            return;
        }

        // 타겟 추적
        Vector3 targetPos = (target as MonoBehaviour).transform.position + Vector3.up;
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;

        // 거리 기반
        if(Vector3.Distance(transform.position, targetPos) < target.Radius)
        {
            CombatManager.Inst.EnqueueDamage(dmgInfo);
            Destroy(gameObject);        // 여기도 풀링
        }
    }
}
