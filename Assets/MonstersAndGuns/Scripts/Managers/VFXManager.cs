using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public enum VFXColor { White, Red, Green, Blue, Yellow, BossRed } // Nota: aquí se encuentra útil poder crear los enums usando SO

public class VFXManager : MonoBehaviour
{
    [Serializable]
    public struct ParticleColor
    {
        public VFXColor vfxColor;
        public ParticleSystem vfxPrefab;
    }

    [SerializeField] private ParticleColor[] deadParticleColors;
    [SerializeField] private ParticleColor[] damageParticleColors;

    Dictionary<VFXColor, ParticleSystem> deadParticleColorsInstances;
    Dictionary<VFXColor, ParticleSystem> damageParticleColorsInstances;

    private void OnEnable()
    {
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead += MonsterDeadHandler; // Se reutiliza la misma función para monster y boss
        GameManager.Instance.OnBossMonsterDamage += MonsterDamageHandler;
        GameManager.Instance.OnMissileDead += MissileDeadHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDamage -= MonsterDamageHandler;
        GameManager.Instance.OnMissileDead -= MissileDeadHandler;
    }

    private void Start()
    {
        deadParticleColorsInstances = new Dictionary<VFXColor, ParticleSystem>();
        damageParticleColorsInstances = new Dictionary<VFXColor, ParticleSystem>();
        InstantiateParticles(deadParticleColors, deadParticleColorsInstances);
        InstantiateParticles(damageParticleColors, damageParticleColorsInstances);
    }

    void InstantiateParticles(ParticleColor[] particleColors, Dictionary<VFXColor, ParticleSystem> particleColorsInstances)
    {
        for (int i = 0; i < particleColors.Length; i++)
        {
            var pc = particleColors[i];
            particleColorsInstances[pc.vfxColor] = Instantiate(pc.vfxPrefab);
        }
    }

    private void MissileDeadHandler(IVFXEntity missil)
    {
        PlayParticleColorsInstance(damageParticleColorsInstances, missil.CurrentColor, missil.ExplosionPosition);
    }

    private void MonsterDamageHandler(IVFXEntity monsterDamage)
    {
        PlayParticleColorsInstance(damageParticleColorsInstances, monsterDamage.CurrentColor, monsterDamage.ExplosionPosition);
    }

    private void MonsterDeadHandler(IVFXEntity monsterDead)
    {
        PlayParticleColorsInstance(deadParticleColorsInstances, monsterDead.CurrentColor, monsterDead.ExplosionPosition);
    }

    void PlayParticleColorsInstance(Dictionary<VFXColor, ParticleSystem> particleColorsInstances, VFXColor color, Vector3 position)
    {
        var particles = particleColorsInstances[color];
        particles.transform.position = position;
        particles.Play();
    }

}
