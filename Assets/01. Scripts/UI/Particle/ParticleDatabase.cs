using System.Collections.Generic;
using UnityEngine;

//
// 모든 파이클 데이터를 중앙 관리할 에셋 파일입니다.
// 매니저에 등록하여 이 SO를 기반으로 한 파티클들을 호출합니다.
//

[CreateAssetMenu(fileName = "ParticleDatabase", menuName = "SO/ParticleDatabase")]
public class ParticleDatabase : ScriptableObject
{
    public List<ParticleData> particles = new List<ParticleData>();

    public ParticleData GetParticle(string key)
    {
        return particles.Find(p => p.Key == key);
    }
}

public enum ParticleType
{
    OneShot,      // 한 번 재생 후 자동 반환
    Duration,     // 지속 시간 유지
    DamageText    // DamageInfo 데이터를 받아 처리
}

[System.Serializable]
public class ParticleData
{
    public string Key;
    public ParticleType Type;
    public GameObject Prefab;
}
