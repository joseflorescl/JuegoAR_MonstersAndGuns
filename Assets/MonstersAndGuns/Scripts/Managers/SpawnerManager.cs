using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private SpawnerManagerData data;

    int currentLevel;
    MonstersByLevel currentMonstersByLevel;



    private void OnEnable()
    {
        GameManager.Instance.OnSpawning += SpawningHandler;
        GameManager.Instance.OnBossBattle += BossBattleHandler;
    }    

    private void OnDisable()
    {
        GameManager.Instance.OnSpawning -= SpawningHandler;
        GameManager.Instance.OnBossBattle -= BossBattleHandler;
    }


    private void SpawningHandler(int level, Vector3 position, Quaternion rotation)
    {
        currentLevel = level;
        StartCoroutine(MonstersSpawningRoutine(position, rotation));
    }

    private void BossBattleHandler()
    {
        StartCoroutine(BossBattleRoutine());
    }

    IEnumerator BossBattleRoutine()
    {
        yield return new WaitForSeconds(data.delaySpawningBossMonster);
        Instantiate(currentMonstersByLevel.bossMonsterPrefab, GameManager.Instance.Portal.position, GameManager.Instance.Portal.rotation);
        GameManager.Instance.BossMonsterSpawned();
    }

    IEnumerator MonstersSpawningRoutine(Vector3 position, Quaternion rotation)
    {
        WaitForSeconds waitBetweenMonsters = new WaitForSeconds(data.spawnTimeBetweenMonsters);
        currentLevel = Mathf.Clamp(currentLevel, 1, data.monstersByLevels.Length);
        currentMonstersByLevel = data.monstersByLevels[currentLevel - 1];

        var initialMonsters = currentMonstersByLevel.initialMonsters;

        for (int i = 0; i < initialMonsters.Length; i++)
        {
            var monsterPrefab = initialMonsters[i].monsterPrefab;
            var count = initialMonsters[i].count;

            for (int j = 0; j < count; j++)
            {               
                Instantiate(monsterPrefab, position, rotation);                
                yield return waitBetweenMonsters;
            }
        }

        GameManager.Instance.MonstersSpawned();
    }


}
