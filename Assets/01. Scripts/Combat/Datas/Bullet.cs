using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;

    DamageInfo dmgInfo;
    IDamageable target;
    bool isActive = false;
    float lifeTime = 0f;

    public void Init(IDamageable a_Target, DamageInfo a_DmgInfo)
    {
        // 데이터 초기화
        target = a_Target;
        dmgInfo = a_DmgInfo;
        if (speed <= 0f)        // 속도에 이상 값이 들어있으면 강제 초기화
            speed = 10f;

        // 투사체의 생명 주기 관리
        isActive = true;
        lifeTime = 5f;

        // 방향 회전 > 제대로 초기화가 안될 경우 여기에서 추가 로직 생성
    }

    // 매니저에서 호출하여 안정성 확보
    public bool OnUpdate(float deltaTime)
    {
        if(!isActive)
        {
            ActiveFalse();      // 방어코드
            return false;
        }

        lifeTime -= deltaTime;

        // 유효성 검사
        if(lifeTime <= 0 || target == null ||
            !target.IsAlive || (target as MonoBehaviour) == null)
        {
            ActiveFalse();
            return false;
        }

        // 이동 및 방향 설정
        Vector3 targetPos = (target as MonoBehaviour).transform.position + Vector3.up;
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.forward = dir;

        // 대상의 콜라이더의 반지름 내까지 진입하면 피격 판정 수행
        if (Vector3.Distance(transform.position, targetPos) < target.Radius)
        {
            CombatManager.Inst.EnqueueDamage(dmgInfo);
            ActiveFalse();
            return false;
        }

        return true;
    }

    void ActiveFalse()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
