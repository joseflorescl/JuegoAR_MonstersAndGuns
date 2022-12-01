using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VFXManager : MonoBehaviour
{
    [Serializable]
    public struct ParticleColor
    {
        public MonsterVFXColor monsterVFXColor;
        public ParticleSystem vfxPrefab;
    }

    [SerializeField] private ParticleColor[] monsterDeadParticleColors;
    [SerializeField] private ParticleColor[] monsterDamageParticleColors;

    Dictionary<MonsterVFXColor, ParticleSystem> monsterDeadParticleColorsInstances;
    Dictionary<MonsterVFXColor, ParticleSystem> monsterDamageParticleColorsInstances;

    private void OnEnable()
    {
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead += MonsterDeadHandler; // Se reutiliza la misma función para monster y boss
        GameManager.Instance.OnBossMonsterDamage += MonsterDamageHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDamage -= MonsterDamageHandler;
    }

    private void Start()
    {
        monsterDeadParticleColorsInstances = new Dictionary<MonsterVFXColor, ParticleSystem>();
        monsterDamageParticleColorsInstances = new Dictionary<MonsterVFXColor, ParticleSystem>();
        InstantiateParticles(monsterDeadParticleColors, monsterDeadParticleColorsInstances);
        InstantiateParticles(monsterDamageParticleColors, monsterDamageParticleColorsInstances);
    }

    void InstantiateParticles(ParticleColor[] particleColors, Dictionary<MonsterVFXColor, ParticleSystem> particleColorsInstances)
    {
        for (int i = 0; i < particleColors.Length; i++)
        {
            var pc = particleColors[i];
            particleColorsInstances[pc.monsterVFXColor] = Instantiate(pc.vfxPrefab);
        }
    }

    private void MonsterDamageHandler(BaseMonsterController monsterDamage)
    {
        PlayParticleColorsInstance(monsterDamageParticleColorsInstances, monsterDamage);
    }

    private void MonsterDeadHandler(BaseMonsterController monsterDead)
    {
        PlayParticleColorsInstance(monsterDeadParticleColorsInstances, monsterDead);
    }

    void PlayParticleColorsInstance(Dictionary<MonsterVFXColor, ParticleSystem> particleColorsInstances, BaseMonsterController monster)
    {
        var monsterColor = monster.CurrentColor;
        var position = monster.ExplosionPosition;
        var particles = particleColorsInstances[monsterColor];
        particles.transform.position = position;
        particles.Play();
    }


}
