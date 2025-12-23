using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TempEnemySpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<UnitClassType> availableTypes; // 생성할 유닛 클래스 목록
    [SerializeField] private Transform[] spawnPoints;           // 적이 나타날 위치들
    [SerializeField] private float spawnInterval = 15f;          // 생성 주기

    private void Start()
    {
        // 게임 시작과 함께 생성 루틴 가동
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnRandomUnit();
        }
    }

    private void SpawnRandomUnit()
    {
        if (UnitFactory.inst == null || availableTypes.Count == 0 || spawnPoints.Length == 0) return;

        // 1. 랜덤 타입 및 위치 선정
        UnitClassType randomType = availableTypes[Random.Range(0, availableTypes.Count)];
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 2. 팩토리를 통해 적 진영 유닛 생성
        Entity enemy = UnitFactory.inst.CreateUnit(randomType, randomPoint.position, UnitFaction.Enemy);

        if (enemy != null)
        {
            Debug.Log($"적군 {randomType} 생성됨");

            // 생성된 적 유닛은 Entity 내부의 SearchTarget/SearchMine 로직에 의해 
            // 자동으로 아군이나 광산을 찾아 움직입니다.
        }
    }
}