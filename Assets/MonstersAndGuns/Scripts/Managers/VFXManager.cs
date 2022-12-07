using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem monsterDeadParticleColorsPrefab;
    [SerializeField] private ParticleSystem bossDeadParticleColorsPrefab;
    [SerializeField] private ParticleSystem damageParticleColorsPrefab;
    [SerializeField] private ParticleSystem missileExplosionParticleColorsPrefab;
    [SerializeField] private ParticleSystem attackParticleColorsPrefab;

    ParticleSystem monsterDeadParticleColorsInstance;
    ParticleSystem bossDeadParticleColorsInstance;
    ParticleSystem damageParticleColorsInstance;
    ParticleSystem missileExplosionParticleColorsInstance;
    ParticleSystem attackParticleColorsInstance;

    private void OnEnable()
    {
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead += BossMonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDamage += BossMonsterDamageHandler;
        GameManager.Instance.OnMissileDead += MissileDeadHandler;
        GameManager.Instance.OnMonsterAttacking += MonsterAttackingHandler;
    }    

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead -= BossMonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDamage -= BossMonsterDamageHandler;
        GameManager.Instance.OnMissileDead -= MissileDeadHandler;
        GameManager.Instance.OnMonsterAttacking -= MonsterAttackingHandler;
    }

    private void Start()
    {
        monsterDeadParticleColorsInstance = Instantiate(monsterDeadParticleColorsPrefab);
        bossDeadParticleColorsInstance = Instantiate(bossDeadParticleColorsPrefab);
        damageParticleColorsInstance = Instantiate(damageParticleColorsPrefab);
        missileExplosionParticleColorsInstance = Instantiate(missileExplosionParticleColorsPrefab);
        attackParticleColorsInstance = Instantiate(attackParticleColorsPrefab);
    }

    private void MonsterAttackingHandler(BaseMonsterController monster)
    {
        PlayParticleColorsInstance(attackParticleColorsInstance, monster.CurrentColor, monster.ExplosionPosition);
    }

    private void BossMonsterDeadHandler(IVFXEntity bossMonster)
    {
        PlayParticleColorsInstance(bossDeadParticleColorsInstance, bossMonster.CurrentColor, bossMonster.ExplosionPosition);
    }


    private void MissileDeadHandler(IVFXEntity missil)
    {
        print("MissileDeadHandler");
        PlayParticleColorsInstance(missileExplosionParticleColorsInstance, missil.CurrentColor, missil.ExplosionPosition);
    }

    private void BossMonsterDamageHandler(IVFXEntity monsterDamage)
    {
        PlayParticleColorsInstance(damageParticleColorsInstance, monsterDamage.CurrentColor, monsterDamage.ExplosionPosition);
    }

    private void MonsterDeadHandler(IVFXEntity monsterDead)
    {
        PlayParticleColorsInstance(monsterDeadParticleColorsInstance, monsterDead.CurrentColor, monsterDead.ExplosionPosition);
    }

    void PlayParticleColorsInstance(ParticleSystem particleColorsInstance, Color color, Vector3 position)
    {                
        var main = particleColorsInstance.main;
        var gradientColor = main.startColor;
        gradientColor.color = color;
        main.startColor = gradientColor;

        particleColorsInstance.transform.position = position;
        particleColorsInstance.Play();
    }

}
