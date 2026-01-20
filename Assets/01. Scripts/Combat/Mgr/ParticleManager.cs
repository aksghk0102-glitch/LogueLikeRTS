using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager inst;

    [Header("Particles SO")]
    [SerializeField] ParticleDatabase db;

    [Header("Damage Text Prefab")]
    [SerializeField] GameObject dmgTextPrefab;


    // 데미지 텍스트 전용 풀
    Stack<DamageTextParticle> dmgTxtPool
        = new Stack<DamageTextParticle>();

    // DB 기반 파티클 전용 풀
    Dictionary<string, Stack<ParticleSystem>> particlePools
        = new Dictionary<string, Stack<ParticleSystem>>();

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);
    }

    // 데미지 텍스트 요청
    public void SpawnDmgTxt(DamageInfo info, Vector3 pos)
    {
        if (dmgTextPrefab == null) return;

        DamageTextParticle particle = GetDamageText();
        particle.transform.position = pos;
        particle.gameObject.SetActive(true);
        particle.Init(info);

        StartCoroutine(DmgTxtReturn(particle));
    }

    // OneShot/Duration 형
    public void SpawnParticle(string key, Vector3 pos, float durTime = -1f)
    {
        var data = db.GetParticle(key);
        if (data == null || data.Prefab == null)
            return;

        ParticleSystem ps = GetParticle(key, data.Prefab);
        ps.transform.position = pos;
        ps.gameObject.SetActive(true);

        // 초기화 후 실행
        var main = ps.main;
        ps.Stop();
        
        // OneShot
        if (durTime < 0f)
        {
            main.loop = false;
            ps.Play();
            StartCoroutine(OneShotReturn(key, ps));
        }
        else
        {
            main.loop = true;
            ps.Play();
            StartCoroutine(DurTypeReturn(key, ps, durTime));
        }

    }

    #region Pooling Utillity
    DamageTextParticle GetDamageText()
    {
        // 풀에 있으면 꺼냄
        while (dmgTxtPool.Count > 0)
        {
            var eff = dmgTxtPool.Pop();
            if (eff != null)
                return eff;
        }

        GameObject obj = Instantiate(dmgTextPrefab, transform);
        DamageTextParticle comp = obj.GetComponent<DamageTextParticle>();
        return comp;
    }

    ParticleSystem GetParticle(string key, GameObject prefab)
    {
        if (!particlePools.ContainsKey(key))
            particlePools.Add(key, new Stack<ParticleSystem>());
    
        while (particlePools[key].Count > 0)
        {
            ParticleSystem ps = particlePools[key].Pop();
            if (ps != null)
                return ps;
        }

        GameObject obj = Instantiate(prefab, transform);
        return obj.GetComponent<ParticleSystem>();
    }

    #endregion
    #region Return Courotine

    IEnumerator DmgTxtReturn(DamageTextParticle eff)
    {
        yield return new WaitUntil(()=> !eff.gameObject.activeSelf);
        dmgTxtPool.Push(eff);
    }

    IEnumerator OneShotReturn(string key, ParticleSystem ps)
    {
        yield return new WaitUntil(() => !ps.IsAlive(true));
        ps.gameObject.SetActive(false);
        particlePools[key].Push(ps);
    }

    IEnumerator DurTypeReturn(string key, ParticleSystem ps,
        float durTime)
    {
        yield return new WaitForSeconds(durTime);
        if(ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.gameObject.SetActive(false);
            particlePools[key].Push(ps);
        }
    }
    #endregion
}
