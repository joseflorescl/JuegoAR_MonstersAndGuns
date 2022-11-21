using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerManager : MonoBehaviour
{

    [SerializeField] private float spawnTimeBetweenEnemies = 0.2f;

    // Se crearán <count> enemigos usando el prefab <enemyPrefab>
    [Serializable]
    struct EnemyToSpawn
    {
        public GameObject enemyPrefab;
        public int count;
    }


    // Esta es la data de los enemigos a crear en un level en particular
    // Notar se puede usar un array de EnemyToSpawnData por lo que en un nivel se pueden crear enemigos de varios prefab
    [Serializable]
    struct EnemiesByLevel
    {
        public EnemyToSpawn[] initialEnemies;
        public GameObject bossEnemyPrefab;
    }

    // Cada elemento de este array es la data de los enemigos a crear en el level == index + 1
    [SerializeField] private EnemiesByLevel[] enemiesByLevels;


    WaitForSeconds waitBetweenEnemies;

    private void Start()
    {
        waitBetweenEnemies = new WaitForSeconds(spawnTimeBetweenEnemies);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnSpawning += OnSpawningHandler;
    }

  

    private void OnDisable()
    {
        GameManager.Instance.OnSpawning -= OnSpawningHandler;
    }


    private void OnSpawningHandler(int level, Vector3 position, Quaternion rotation)
    {
        StartCoroutine(EnemiesSpawningRoutine(level, position, rotation));
    }

    IEnumerator EnemiesSpawningRoutine(int level, Vector3 position, Quaternion rotation)
    {
        level = Mathf.Clamp(level, 1, enemiesByLevels.Length);
        var levelEnemies = enemiesByLevels[level - 1];
        var initialEnemies = levelEnemies.initialEnemies;

        for (int i = 0; i < initialEnemies.Length; i++)
        {
            var enemyPrefab = initialEnemies[i].enemyPrefab;
            var count = initialEnemies[i].count;

            for (int j = 0; j < count; j++)
            {
                // TODO: después de instanciar al monster se debería setear la property TimeToAttack
                // TODO: al instanciar se debería emitir un sonido: esto se podría hacer de esta forma
                /*
                 GameManager.Instance.MonsterCreated();
                y el GM llama a evento OnMonsterCreated?.Invoke();
                y el AudioManager se subscribe a ese evento con un método que emite el sonido de un Pop()
                 */
                Instantiate(enemyPrefab, position, rotation);
                yield return waitBetweenEnemies;
            }
        }
        
    }


}
