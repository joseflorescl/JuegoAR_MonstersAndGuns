using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseAudioManager
{
    [SerializeField] private AudioSource SFXAudioSource;
    [SerializeField] private AudioSource SFXVoiceAudioSource;

    [SerializeField] private AudioManagerData data;

    

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreationHandler;
        GameManager.Instance.OnBattling += BattleHandler;
        GameManager.Instance.OnMonsterCreated += MonsterCreatedHandler;        
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnPlayerFired += PlayerFiredHandler;
        GameManager.Instance.OnPlayerDamage += PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
        GameManager.Instance.OnGameOver += GameOverHandler;
        GameManager.Instance.OnMonstersSpawned += MonstersSpawnedHandler;
        GameManager.Instance.OnSpawning += SpawningHandler;
        GameManager.Instance.OnBossBattle += BossBattleHandler;
        GameManager.Instance.OnBossMonsterSpawned += BossMonsterSpawnedHandler;
        GameManager.Instance.OnBossMonsterDamage += BossMonsterDamageHandler;
        GameManager.Instance.OnBossMonsterDead += BossMonsterDeadHandler;
        GameManager.Instance.OnMonsterFired += MonsterFiredHandler;
        GameManager.Instance.OnMissileDead += MissileDeadHandler;
        GameManager.Instance.OnMonsterAttacking += MonsterAttackingHandler;
        GameManager.Instance.OnWinLevel += WinLevelHandler;
        GameManager.Instance.OnScoreIncrementing += ScoreIncrementingHandler;
        GameManager.Instance.OnScoreIncremented += ScoreIncrementedHandler;
    }   

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreationHandler;
        GameManager.Instance.OnBattling -= BattleHandler;
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;        
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnPlayerFired -= PlayerFiredHandler;
        GameManager.Instance.OnPlayerDamage -= PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
        GameManager.Instance.OnGameOver -= GameOverHandler;
        GameManager.Instance.OnMonstersSpawned -= MonstersSpawnedHandler;
        GameManager.Instance.OnSpawning -= SpawningHandler;
        GameManager.Instance.OnBossBattle -= BossBattleHandler;
        GameManager.Instance.OnBossMonsterSpawned -= BossMonsterSpawnedHandler;
        GameManager.Instance.OnBossMonsterDamage -= BossMonsterDamageHandler;
        GameManager.Instance.OnBossMonsterDead -= BossMonsterDeadHandler;
        GameManager.Instance.OnMonsterFired -= MonsterFiredHandler;
        GameManager.Instance.OnMissileDead -= MissileDeadHandler;
        GameManager.Instance.OnMonsterAttacking -= MonsterAttackingHandler;
        GameManager.Instance.OnWinLevel -= WinLevelHandler;
        GameManager.Instance.OnScoreIncrementing -= ScoreIncrementingHandler;
        GameManager.Instance.OnScoreIncremented -= ScoreIncrementedHandler;
    }


    private void ScoreIncrementedHandler()
    {
        SFXAudioSource.loop = false;
        SFXAudioSource.Stop();
    }

    private void ScoreIncrementingHandler()
    {
        PlayRandomSoundWhitLoop(data.scoreIncrementSound, SFXAudioSource);
    }

    private void WinLevelHandler()
    {
        PlayWinLevelMusic();
    }

    private void MonsterAttackingHandler()
    {
        PlayRandomSound(data.monsterAttacking, SFXAudioSource);
    }

    private void MissileDeadHandler(MissileController obj)
    {
        PlayRandomSound(data.missileExplosions, SFXAudioSource);
    }

    private void MonsterFiredHandler()
    {
        PlayRandomSound(data.monsterFired, SFXAudioSource);
    }

    private void BossMonsterDeadHandler(BaseMonsterController obj)
    {
        PlayRandomSound(data.bossMonsterExplosions, SFXAudioSource);
    }

    private void BossMonsterDamageHandler(BaseMonsterController obj)
    {
        PlayRandomSound(data.bossMonsterDamage, SFXAudioSource);
    }

    private void BossMonsterSpawnedHandler()
    {
        PlayRandomSound(data.bossMonsterSpawned, SFXAudioSource);
    }

    private void BossBattleHandler()
    {
        PlayBossBattleMusic();
    }

    private void SpawningHandler(int arg1, Vector3 arg2, Quaternion arg3)
    {
        StopGameMusic();
    }

    private void MonstersSpawnedHandler()
    {
        PlayBattleMusic();
    }

    private void BattleHandler(List<MonsterController> arg1, int arg2)
    {
        PlayRandomSound(data.goSound, SFXAudioSource);
    }

    private void GameOverHandler()
    {
        PlayGameOverMusic();
    }

    private void PlayerDeadHandler()
    {
        StopGameMusic();
        PlayRandomSoundWithDelay(data.playerDead, SFXVoiceAudioSource, data.delayPlayerDeadSound, randomPitch: false, volumeScale: data.volumeScalePlayerDead);
    }

    private void PlayerDamageHandler(float obj)
    {        
        PlayRandomSoundWithDelay(data.playerDamage, SFXVoiceAudioSource, data.delayPlayerDamageSound, randomPitch: true, volumeScale: data.volumeScalePlayerDamage);
    }

    private void PlayerFiredHandler(int obj)
    {
        PlayRandomSound(data.playerFired, SFXAudioSource, randomPitch: false, volumeScale: data.volumeScalePlayerFired);
    }

    private void MonsterDeadHandler(BaseMonsterController obj)
    {
        PlayRandomSound(data.monsterExplosions, SFXAudioSource);
    }

    private void MonsterCreatedHandler()
    {
        PlayRandomSound(data.monsterSpawned, SFXAudioSource, randomPitch: true);
    }

    private void MainMenuHandler()
    {
        PlayMainMenuMusic();
    }


    private void PortalCreationHandler()
    {
        BGMAudioSource.Stop();
        SFXAudioSource.Stop();
        float duration = PlayRandomSound(data.pressStartGame, SFXAudioSource);

        StopAudioRoutine();
        audioRoutine = StartCoroutine(PlayMainMenuMusicWithDelayRoutine(duration));

    }

    private IEnumerator PlayMainMenuMusicWithDelayRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayMainMenuMusic();
    }

    private void PlayMainMenuMusic() => PlayRandomMusic(data.mainMenuMusic, true);
    
    private void PlayBattleMusic() => PlayRandomMusic(data.battleMusic, true);
    
    private void PlayBossBattleMusic() => PlayRandomMusic(data.bossBattleMusic, true);

    private void PlayWinLevelMusic()
    {
        StartCoroutine(PlayWinLevelMusicRoutine());

    }

    IEnumerator PlayWinLevelMusicRoutine()
    {
        float duration = PlayRandomMusic(data.winMusic, false);
        yield return new WaitForSeconds(duration);
        GameManager.Instance.EndWinLevelMusic();
    }


    private void PlayGameOverMusic() => PlayRandomMusic(data.gameOverMusic, false);
    

}
