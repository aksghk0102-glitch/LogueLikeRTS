using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Barracks : Building
{
    [Header("Barracks Settings")]
    [SerializeField] UnitClassType unitType;
    public UnitClassType UnitType => unitType;

    [Header("Infomations")]
    [SerializeField] float spawnTime = 15f; // 유닛 생성 시간
    [SerializeField] int maxQueueCount = 5; // 최대 대기열 수
    [SerializeField] Transform spawnPoint;  // 스폰 위치

    int curQueueCount = 0;      // 예약된 유닛 수 > 굳이 Queue가 필요 없는 듯
    float curProgress = 0.0f;   // 현재 유닛 진행도
    bool isSpawn = false;       // 유닛이 생성 중인지

    [Header("UI")]
    [SerializeField] Image fillGague;

    public float Cost => 100f;      // 임시 수치
    // 외부에서 요청
    public bool RequestSpawnUnit()
    {
        if (curQueueCount >= maxQueueCount)
        {
            // 안내 메세지 하나 출력
            return false;
        }

        curQueueCount++;

        if (!isSpawn)
        {
            isSpawn = true;
            curProgress = 0f;
        }
        return true;
    }

    private void Update()
    {
        if (!isSpawn)
            return;

        UpdateSpawn(Time.deltaTime) ;
    }

    void UpdateSpawn(float deltaTime)
    {
        curProgress += deltaTime;

        if(curProgress >= spawnTime)
        {
            SpawnUnit();
            curQueueCount--;

            if (curQueueCount > 0)
                curProgress = 0f;
            else
            {
                isSpawn = false;
                curProgress = 0f;
            }
        }
    }

    void SpawnUnit()
    {
        if(UnitFactory.inst != null)
        {
            Entity spawnUnit = UnitFactory.inst.CreateUnit(unitType,
                spawnPoint.position, Faction);

            if (ObjectManager.Inst != null)
                ObjectManager.Inst.RegistObject(spawnUnit);
        }
    }

    public int GetQueueCount() => curQueueCount;
    public float GetProgressGague() => Mathf.Clamp01(curProgress / spawnTime);
}
