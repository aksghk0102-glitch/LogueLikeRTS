using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager inst { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    // 설정된 기본 경로
    private const string SFX_PATH = "SFX/";

    private Dictionary<string, AudioClip> sfxCache = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// SFX 재생: Resources/SFX/ 하위의 파일명만 입력
    /// </summary>
    public void PlaySFX(string fileName)
    {
        // 1. 캐시 확인 및 로드 (경로 합성)
        if (!sfxCache.TryGetValue(fileName, out AudioClip clip))
        {
            string fullPath = SFX_PATH + fileName;
            clip = Resources.Load<AudioClip>(fullPath);

            if (clip == null)
            {
                Debug.LogWarning($"[SoundManager] 사운드를 찾을 수 없습니다: {fullPath}");
                return;
            }
            sfxCache.Add(fileName, clip);
        }

        //// 2. 가벼운 중복 판별 (동일 클립 재생 중이면 무시) => 안하는 게 자연스러운 듯
        //if (sfxSource.clip == clip && sfxSource.isPlaying)
        //{
        //    return;
        //}

        // 3. 재생
        sfxSource.clip = clip;
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// BGM 재생: Resources/ 하위의 전체 경로 입력
    /// </summary>
    public void PlayBGM(string path, bool loop = true)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    public void ClearCache()
    {
        sfxCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}

/// <summary>
/// UI 파일 명 키 값 정리
/// 
/// 마우스 호버 시
/// Interface 2-2
/// 
/// 클릭 시
/// Interface 3-1
/// 취소 시
/// 
/// 건설 확정
/// 
/// 장비 장착
/// Interface 6-4
/// 
/// 장비 탈착
/// 
/// 
/// 가방 열 때
/// Bag Handle 1-5
/// 
/// 코스트 소모
/// Coin Bag 1-2
/// 
/// 
/// 피격 시
/// - 비무장 Rock Impact 37
/// -무장 Armor 1-2
/// - 스켈레톤 Armor 1-4
/// 
/// 
/// 전투 시작 시
/// BattleStart
/// </summary>
