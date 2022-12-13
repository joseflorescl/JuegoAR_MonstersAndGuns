using System.Collections;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private SpawnerManagerData data;
    [SerializeField] private MonsterController[] monstersPrefabs;
    [SerializeField] private BossMonsterController[] bossMonstersPrefabs;
    [SerializeField] private int incrementMonstersCountByNewLevel = 5;

    int currentLevel;
    MonstersByLevel currentMonstersByLevel;
    int monstersCount;



    private void OnEnable()
    {
        GameManager.Instance.OnSpawning += SpawningHandler;
        GameManager.Instance.OnBossBattle += BossBattleHandler;
        GameManager.Instance.OnGameOver += GameOverHandler;
    }

    
    private void OnDisable()
    {
        GameManager.Instance.OnSpawning -= SpawningHandler;
        GameManager.Instance.OnBossBattle -= BossBattleHandler;
        GameManager.Instance.OnGameOver -= GameOverHandler;
    }

    private void GameOverHandler(float delay)
    {
        StopAllCoroutines();
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

        if (currentLevel <= data.monstersByLevels.Length)
        {
            //currentLevel = Mathf.Clamp(currentLevel, 1, data.monstersByLevels.Length);
            currentMonstersByLevel = data.monstersByLevels[currentLevel - 1];

            var initialMonsters = currentMonstersByLevel.initialMonsters;

            monstersCount = 0;
            for (int i = 0; i < initialMonsters.Length; i++)
            {
                var monsterPrefab = initialMonsters[i].monsterPrefab;
                var count = initialMonsters[i].count;

                for (int j = 0; j < count; j++)
                {
                    monstersCount++;
                    Instantiate(monsterPrefab, position, rotation);
                    yield return waitBetweenMonsters;
                }
            }
        }
        else
        {
            // Este nivel no está configurado en el Spawner Data, por lo que se creará aleatoriamente
            monstersCount += incrementMonstersCountByNewLevel;
            for (int i = 0; i < monstersCount; i++)
            {
                int idx = Random.Range(0, monstersPrefabs.Length - 1);
                var monsterPrefab = monstersPrefabs[idx];
                Instantiate(monsterPrefab, position, rotation);
                yield return waitBetweenMonsters;
            }
        }

        GameManager.Instance.MonstersSpawned();
    }


}
