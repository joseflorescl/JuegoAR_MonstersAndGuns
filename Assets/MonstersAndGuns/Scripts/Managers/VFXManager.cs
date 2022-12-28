using System.Collections;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem monsterDeadParticleColorsPrefab;
    [SerializeField] private ParticleSystem bossDeadParticleColorsPrefab;
    [SerializeField] private ParticleSystem damageParticleColorsPrefab;
    [SerializeField] private ParticleSystem missileExplosionParticleColorsPrefab;
    [SerializeField] private ParticleSystem attackParticleColorsPrefab;
    [SerializeField] private ParticleSystem portalParticleColorsPrefab;
    [SerializeField] private float delayDamageMaterial = 0.1f;

    ParticleSystem monsterDeadParticleColorsInstance;
    ParticleSystem bossDeadParticleColorsInstance;
    ParticleSystem damageParticleColorsInstance;
    ParticleSystem missileExplosionParticleColorsInstance;
    ParticleSystem attackParticleColorsInstance;
    ParticleSystem portalParticleColorsInstance;


    private void OnEnable()
    {
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead += BossMonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDamage += BossMonsterDamageHandler;
        GameManager.Instance.OnMonsterDamage += MonsterDamageHandler;
        GameManager.Instance.OnMissileDead += MissileDeadHandler;
        GameManager.Instance.OnMonsterAttacking += MonsterAttackingHandler;
        GameManager.Instance.OnPortalCreated += PortalCreatedHandler;
        GameManager.Instance.OnRestart += RestartHandler;
        GameManager.Instance.OnMonsterCreated += MonsterCreatedHandler;

    }

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDead -= BossMonsterDeadHandler;
        GameManager.Instance.OnBossMonsterDamage -= BossMonsterDamageHandler;
        GameManager.Instance.OnMonsterDamage -= MonsterDamageHandler;
        GameManager.Instance.OnMissileDead -= MissileDeadHandler;
        GameManager.Instance.OnMonsterAttacking -= MonsterAttackingHandler;
        GameManager.Instance.OnPortalCreated -= PortalCreatedHandler;
        GameManager.Instance.OnRestart -= RestartHandler;
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;
    }

    private void Start()
    {
        monsterDeadParticleColorsInstance = Instantiate(monsterDeadParticleColorsPrefab);
        bossDeadParticleColorsInstance = Instantiate(bossDeadParticleColorsPrefab);
        damageParticleColorsInstance = Instantiate(damageParticleColorsPrefab);
        missileExplosionParticleColorsInstance = Instantiate(missileExplosionParticleColorsPrefab);
        attackParticleColorsInstance = Instantiate(attackParticleColorsPrefab);
        portalParticleColorsInstance = Instantiate(portalParticleColorsPrefab);
    }

    private void MonsterCreatedHandler(IVFXEntity monster)
    {
        if (monster.NormalMaterial)
            UseMaterialOnVFXEntity(monster.NormalMaterial, monster);
    }


    private void RestartHandler()
    {
        portalParticleColorsInstance.Stop();
    }

    private void PortalCreatedHandler()
    {
        var position = GameManager.Instance.Portal.transform.position;
        portalParticleColorsInstance.transform.position = position;
        portalParticleColorsInstance.Play();
    }

    private void MonsterAttackingHandler(IVFXEntity monster)
    {
        PlayParticleColorsInstance(attackParticleColorsInstance, monster.CurrentColor, monster.ExplosionPosition);
        
        if (monster.AttackMaterial)
            UseMaterialOnVFXEntity(monster.AttackMaterial, monster);
    }

    private void MonsterDamageHandler(IVFXEntity monster)
    {
        PlayParticleColorsInstance(damageParticleColorsInstance, monster.CurrentColor, monster.ExplosionPosition);
    }

    private void BossMonsterDeadHandler(IVFXEntity bossMonster)
    {
        PlayParticleColorsInstance(bossDeadParticleColorsInstance, bossMonster.CurrentColor, bossMonster.ExplosionPosition);
    }


    private void MissileDeadHandler(IVFXEntity missil)
    {
        PlayParticleColorsInstance(missileExplosionParticleColorsInstance, missil.CurrentColor, missil.ExplosionPosition);
    }

    private void BossMonsterDamageHandler(IVFXEntity monsterDamage)
    {
        PlayParticleColorsInstance(damageParticleColorsInstance, monsterDamage.CurrentColor, monsterDamage.ExplosionPosition);

        if (monsterDamage.DamageMaterial)
            StartCoroutine(UseMaterialOnVFXEntityAndRevertRoutine(monsterDamage.DamageMaterial, monsterDamage, delayDamageMaterial));
    }

    IEnumerator UseMaterialOnVFXEntityAndRevertRoutine(Material material, IVFXEntity vfxEntity, float delay)
    {        
        UseMaterialOnVFXEntity(material, vfxEntity);
        yield return new WaitForSeconds(delay);
        // Condición de borde: validar que el objeto siga activo antes de volver a setear su material
        if (vfxEntity.Renderers[0] != null)
            UseMaterialOnVFXEntity(vfxEntity.NormalMaterial, vfxEntity);               
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

    void UseMaterialOnVFXEntity(Material material, IVFXEntity vfxEntity)
    {
        for (int i = 0; i < vfxEntity.Renderers.Length; i++)
        {
            var rend = vfxEntity.Renderers[i];
            rend.material = material;
        }
    }

}
