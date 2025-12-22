using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public enum BulletKey       // 직렬화되어 있으므로 리스트 중간에 넣지 말것!!
{
    HM_Range_Attack,        // 인간 레인저 평타 투사체
    HM_Mage_Attack,         // 인간 메이지 평타 투사체
    SK_Range_Attack,        // 스켈레톤 레인저
    SK_Mage_Attack,         // 스켈레톤 메이지

    Range_Arrow_1,
    Mage_FireBall_1,
}

public class BulletManager : MonoBehaviour
{
    public static BulletManager inst;

    [System.Serializable]
    public struct BulletData
    {
        public BulletKey key;
        public GameObject prefab;
    }

    [Header("Projectiles")]
    [SerializeField] private List<BulletData> bulletDatas;

    Dictionary<BulletKey, List<Bullet>> pools = new Dictionary<BulletKey, List<Bullet>>();
    Dictionary<BulletKey, GameObject> prefabs = new Dictionary<BulletKey, GameObject>();

    // 활성화 된 투사체 관리용 리스트
    List<Bullet> actives = new List<Bullet>();

    void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        foreach(var data in bulletDatas)
        {
            if (data.prefab == null || prefabs.ContainsKey(data.key))
                continue;

            prefabs[data.key] = data.prefab;
            pools[data.key] = new List<Bullet>();
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        for(int i = actives.Count - 1; i >= 0; i--)
        {
            if (!actives[i].OnUpdate(deltaTime))
                actives.RemoveAt(i);
        }
    }


    // 엔티티에서 투사체 생성 요청
    public void SpawnBullet(BulletKey key, Vector3 firePoint
        , IDamageable target, DamageInfo dmgInfo)
    {
        if (!prefabs.ContainsKey(key))
            return;

        Bullet bullet = GetBullet(key);

        // 위치/방향 설정
        bullet.transform.position = firePoint;
        Vector3 targetPos = (target as MonoBehaviour).transform.position;
        Vector3 dir = (targetPos - firePoint).normalized;
        if (dir != Vector3.zero)
            bullet.transform.forward = dir;

        bullet.Init(target, dmgInfo);

        if (!actives.Contains(bullet))
            actives.Add(bullet);
    }

    // 풀에서 투사체를 반환, 풀에 없다면 새로 생성
    Bullet GetBullet(BulletKey key)
    {
        List<Bullet> list = pools[key];

        for(int i = 0; i < list.Count; i++)
        {
            if(list[i] != null && !list[i].gameObject.activeSelf)
            {
                list[i].gameObject.SetActive(true);
                return list[i];
            }
        }

        GameObject go = Instantiate(prefabs[key], transform);
        Bullet newBullet = go.GetComponent<Bullet>();
        list.Add(newBullet);
        return newBullet;
    }
}
