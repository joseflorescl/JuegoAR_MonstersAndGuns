using UnityEngine;
using System;


[CreateAssetMenu(fileName = "New Spawner Manager Data", menuName = "Monsters n Guns/Spawner Manager Data")]
public class SpawnerManagerData : ScriptableObject
{    
    public float spawnTimeBetweenMonsters = 0.2f;
    public float delaySpawningBossMonster = 4f;
    public MonstersByLevel[] monstersByLevels; // Cada elemento de este array es la data de los monstruos a crear en el level == index + 1
}

// Se crearán <count> monstruos usando el prefab <monsterPrefab>
[Serializable]
public struct MonsterToSpawn
{
    public MonsterController monsterPrefab;
    public int count;
}

// Esta es la data de los monstruos a crear en un level en particular
// Notar se puede usar un array de MonsterToSpawn por lo que en un nivel se pueden crear monstruos de varios prefab
[Serializable]
public struct MonstersByLevel
{
    public MonsterToSpawn[] initialMonsters;
    public BossMonsterController bossMonsterPrefab;    
}


