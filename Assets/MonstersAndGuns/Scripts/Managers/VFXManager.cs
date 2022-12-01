using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VFXManager : MonoBehaviour
{
    [Serializable]
    public struct ParticleColor
    {
        public MonsterColor monsterColor;
        public ParticleSystem vfxPrefab;
    }

    [SerializeField] private ParticleColor[] monsterDeadParticleColors;
    [SerializeField] private ParticleColor[] monsterDamageParticleColors;

    Dictionary<MonsterColor, ParticleSystem> monsterDeadParticleColorsInstances;
    Dictionary<MonsterColor, ParticleSystem> monsterDamageParticleColorsInstances;

    private void OnEnable()
    {
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnMonsterDamage += MonsterDamageHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnMonsterDamage -= MonsterDamageHandler;
    }

    private void Start()
    {
        monsterDeadParticleColorsInstances = new Dictionary<MonsterColor, ParticleSystem>();
        monsterDamageParticleColorsInstances = new Dictionary<MonsterColor, ParticleSystem>();
        InstantiateParticles(monsterDeadParticleColors, monsterDeadParticleColorsInstances);
        InstantiateParticles(monsterDamageParticleColors, monsterDamageParticleColorsInstances);
    }

    void InstantiateParticles(ParticleColor[] particleColors, Dictionary<MonsterColor, ParticleSystem> particleColorsInstances)
    {
        for (int i = 0; i < particleColors.Length; i++)
        {
            var pc = particleColors[i];
            particleColorsInstances[pc.monsterColor] = Instantiate(pc.vfxPrefab);
        }
    }

    private void MonsterDamageHandler()
    {
        print("VFX MonsterDamageHandler");

    }

    private void MonsterDeadHandler(BaseMonsterController monsterDead)
    {
        var monsterColor = monsterDead.CurrentColor;
        var position = monsterDead.ExplosionPosition;
        var particles = monsterDeadParticleColorsInstances[monsterColor];
        particles.transform.position = position;
        particles.Play();
    }

   
}
