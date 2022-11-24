using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnerManager : MonoBehaviour
{

    [SerializeField] private float spawnTimeBetweenMonsters = 0.2f;

    // Se crearán <count> monstruos usando el prefab <monsterPrefab>
    [Serializable]
    struct MonsterToSpawn
    {
        public MonsterController monsterPrefab;
        public int count;
    }


    // Esta es la data de los monstruos a crear en un level en particular
    // Notar se puede usar un array de MonsterToSpawn por lo que en un nivel se pueden crear monstruos de varios prefab
    [Serializable]
    struct MonstersByLevel
    {
        public MonsterToSpawn[] initialMonsters;
        public GameObject bossMonsterPrefab;
    }

    // Cada elemento de este array es la data de los monstruos a crear en el level == index + 1
    [SerializeField] private MonstersByLevel[] monstersByLevels;


    WaitForSeconds waitBetweenMonsters;
    List<MonsterController> monsters;

    private void Start()
    {
        waitBetweenMonsters = new WaitForSeconds(spawnTimeBetweenMonsters);
        monsters = new List<MonsterController>();
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
        StartCoroutine(MonstersSpawningRoutine(level, position, rotation));
    }

    IEnumerator MonstersSpawningRoutine(int level, Vector3 position, Quaternion rotation)
    {
        level = Mathf.Clamp(level, 1, monstersByLevels.Length);
        var levelMonsters = monstersByLevels[level - 1];
        var initialMonsters = levelMonsters.initialMonsters;

        for (int i = 0; i < initialMonsters.Length; i++)
        {
            var monsterPrefab = initialMonsters[i].monsterPrefab;
            var count = initialMonsters[i].count;

            for (int j = 0; j < count; j++)
            {
                // TODO: después de instanciar al monster se debería setear la property TimeToAttack
                // TODO: al instanciar se debería emitir un sonido: esto se podría hacer de esta forma
                /*
                 GameManager.Instance.MonsterCreated();
                y el GM llama a evento OnMonsterCreated?.Invoke();
                y el AudioManager se subscribe a ese evento con un método que emite el sonido de un Pop()
                 */
                
                var monster = Instantiate(monsterPrefab, position, rotation);
                monsters.Add(monster);
                yield return waitBetweenMonsters;
            }
        }

        GameManager.Instance.MonstersSpawned(monsters);
    }


}
