using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// 광산 오브젝트 관리 > 자원은 GoldManager

public class Mine : MonoBehaviour
{
    [Header("Faction")]
    public UnitFaction curFaction = UnitFaction.Neutral;
    UnitFaction curOccFaction = UnitFaction.Neutral;
    [SerializeField] Image fillGague;
    
    float occTime = 10.0f;      // 점령에 필요한 시간
    float progress = 0.0f;      // 점령 진행도
    bool isOccupying = false;       // 현재 점령 시도 중인지
    Entity curOccUnit = null;       // 현재 점령 중인 유닛

    [Header("Resource Settings")]
    public float goldAmount = 100f; // 골드 생성량
    public float goldBonus = 3f;
    public float term = 10f;         // 수급 주기
    [SerializeField] GoldGetText effPrefab;     // 텍스트 프리펩
    [SerializeField] Transform effCanvas;     // 이펙트 생성 위치
    [SerializeField] int poolSize = 3;

    [Header("Mines")]
    [SerializeField] List<BuildSlot> buildSlots = new List<BuildSlot>();

    float radius = 1.5f;
    public float Radius => radius + 0.1f;

    List<GoldGetText> effPool = new List<GoldGetText>();
    Coroutine mainCoroutine;

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
            CreateNewEffect();

        if (curFaction != UnitFaction.Neutral)
            mainCoroutine = StartCoroutine(ProduceGold());

        if (ObjectManager.Inst != null)
            ObjectManager.Inst.RegistMine(this);

        CapsuleCollider cc = GetComponent<CapsuleCollider>();
        if (cc != null)
            radius = cc.radius;

        UpdateBuildSlots();
    }

    public bool TryOccupy(Entity entity)
    {
        if (entity == null) return false;
        if (curFaction == entity.Faction
            && progress >= occTime) return false;
        if (curOccUnit != null)  return false;

        curOccUnit = entity;
        curOccFaction = entity.Faction;
        curOccUnit.isOccupying = true;

        if (!isOccupying)
        {
            isOccupying = true;
            StartCoroutine(Occupy());
        }
        return true;
    }

    IEnumerator Occupy()
    {
        while (isOccupying)
        {
            if (curOccUnit == null)
            {
                yield break;
            }

            if (curFaction == UnitFaction.Neutral || curOccFaction == curFaction)
                progress += Time.deltaTime;
            else
                progress -= Time.deltaTime;

            progress = Mathf.Clamp(progress, 0, occTime);

            // 점령도 UI 업데이트
            fillGague.fillAmount = progress / occTime;

            fillGague.color = (curOccFaction == UnitFaction.Player)
                ? ColorDefine.Player : ColorDefine.Enemy;

            if(progress >= occTime)
            {
                CompleateOccupy();
                yield break;
            }
            else if(progress <= 0.0f)
            {
                curFaction = UnitFaction.Neutral;
            }
            yield return null;
        }
    }

    void CompleateOccupy()
    {
        curFaction = curOccFaction;
        isOccupying = false;
        curOccUnit.isOccupying = false;

        if (mainCoroutine != null)
            StopCoroutine(mainCoroutine);

        mainCoroutine = StartCoroutine(ProduceGold());
        GetGold();

        if (curOccUnit != null)
            curOccUnit = null;

        UpdateBuildSlots();
    }

    void UpdateBuildSlots()
    {
        if (buildSlots.Count > 0)
            foreach (var slot in buildSlots)
                slot.UpdateFaction(curFaction);
    }

    #region
    void CreateNewEffect()
    {
        GoldGetText ggt = Instantiate(effPrefab, effCanvas);
        ggt.gameObject.SetActive(false);
        effPool.Add(ggt);
    }

    IEnumerator ProduceGold()
    {
        while (curFaction != UnitFaction.Neutral)
        {
            yield return new WaitForSeconds(term);

            if(curFaction == UnitFaction.Player)
            {
                GetGold();
            }
            else if (curFaction == UnitFaction.Enemy)
            {
                // 적 자원 관리
            }
        }
    }

    void GetGold()
    {
        GoldManager.inst.AddBonusGold(goldBonus);
        ShowGoldTextEffect($"+{goldAmount}G");
    }

    void ShowGoldTextEffect(string text)
    {
        GoldGetText targetEffect =
            effPool.Find(ggt => !ggt.gameObject.activeSelf);
        if(targetEffect == null)
        {
            CreateNewEffect();
            targetEffect = effPool[effPool.Count - 1];
        }

        targetEffect.Init(text);
    }
    #endregion
}
