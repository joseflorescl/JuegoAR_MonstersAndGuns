using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private SpawnerManagerData data;   

    private void OnEnable()
    {
        GameManager.Instance.OnSpawning += SpawningHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSpawning -= SpawningHandler;
    }


    private void SpawningHandler(int level, Vector3 position, Quaternion rotation)
    {
        StartCoroutine(MonstersSpawningRoutine(level, position, rotation));
    }

    IEnumerator MonstersSpawningRoutine(int level, Vector3 position, Quaternion rotation)
    {
        WaitForSeconds waitBetweenMonsters = new WaitForSeconds(data.spawnTimeBetweenMonsters);
        level = Mathf.Clamp(level, 1, data.monstersByLevels.Length);
        var levelMonsters = data.monstersByLevels[level - 1];
        var initialMonsters = levelMonsters.initialMonsters;

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
