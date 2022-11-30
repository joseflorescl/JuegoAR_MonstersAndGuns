using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioSource SFXAudioSource;
    [SerializeField] private AudioSource SFXVoiceAudioSource;
    [SerializeField] private float pitchVariation = 0.2f;

    [Header("BGM Sounds")]
    public AudioClip[] mainMenuMusic;
    public AudioClip[] battleMusic;
    public AudioClip[] gameOverMusic;

    [Space(10)]
    [Header("SFX Sounds")]
    [SerializeField] private AudioClip[] pressStartGame;
    [SerializeField] private AudioClip[] popSound;
    [SerializeField] private AudioClip[] monsterExplosions;
    [SerializeField] private AudioClip[] gunFired;
    [SerializeField] private AudioClip[] goSound;
    [SerializeField] private float volumeScaleGunFired = 0.25f;
    [SerializeField] private AudioClip[] playerDamage;
    [SerializeField] private float volumeScalePlayerDamage = 1f;
    [SerializeField] private float delayPlayerDamageSound = 0.2f;
    [SerializeField] private AudioClip[] playerDead;
    [SerializeField] private float volumeScalePlayerDead = 1f;
    [SerializeField] private float delayPlayerDeadSound = 0.2f;


    HashSet<AudioClip> clipsPlayedThisFrame;
    Coroutine audioRoutine;

    private void Awake() => clipsPlayedThisFrame = new HashSet<AudioClip>();

    private void LateUpdate() => clipsPlayedThisFrame.Clear();

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreationHandler;
        GameManager.Instance.OnBattling += BattleHandler;
        GameManager.Instance.OnMonsterCreated += MonsterCreatedHandler;        
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnGunFired += GunFiredHandler;
        GameManager.Instance.OnPlayerDamage += PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
        GameManager.Instance.OnGameOver += GameOverHandler;
        GameManager.Instance.OnMonstersSpawned += MonstersSpawnedHandler;
        GameManager.Instance.OnSpawning += SpawningHandler;



    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreationHandler;
        GameManager.Instance.OnBattling -= BattleHandler;
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;        
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnGunFired -= GunFiredHandler;
        GameManager.Instance.OnPlayerDamage -= PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
        GameManager.Instance.OnGameOver -= GameOverHandler;
        GameManager.Instance.OnMonstersSpawned -= MonstersSpawnedHandler;
        GameManager.Instance.OnSpawning -= SpawningHandler;
    }

    private void SpawningHandler(int arg1, Vector3 arg2, Quaternion arg3)
    {
        StopGameMusic();
    }

    private void MonstersSpawnedHandler()
    {
        print("Battle Music");
        PlayBattleMusic();
    }

    private void BattleHandler(List<MonsterController> arg1, int arg2)
    {
        PlayRandomSound(goSound, SFXAudioSource);
    }


    private void GameOverHandler()
    {
        PlayGameOverMusic();
    }

    private void PlayerDeadHandler()
    {
        StopGameMusic();
        PlayRandomSoundWithDelay(playerDead, SFXVoiceAudioSource, delayPlayerDeadSound, randomPitch: false, volumeScale: volumeScalePlayerDead);
    }

    private void PlayerDamageHandler(float obj)
    {        
        PlayRandomSoundWithDelay(playerDamage, SFXVoiceAudioSource, delayPlayerDamageSound, randomPitch: true, volumeScale: volumeScalePlayerDamage);
    }




    private void GunFiredHandler(int obj)
    {
        PlayRandomSound(gunFired, SFXAudioSource, randomPitch: false, volumeScale: volumeScaleGunFired);
    }

    private void MonsterDeadHandler(MonsterController obj)
    {
        PlayRandomSound(monsterExplosions, SFXAudioSource);
    }

    

    private void MonsterCreatedHandler()
    {
        PlayRandomSound(popSound, SFXAudioSource, randomPitch: true);
    }



    private void MainMenuHandler()
    {
        PlayMainMenuMusic();
    }


    private void PortalCreationHandler()
    {
        BGMAudioSource.Stop();
        SFXAudioSource.Stop();
        float duration = PlayRandomSound(pressStartGame, SFXAudioSource);

        StopAudioRoutine();
        audioRoutine = StartCoroutine(PlayMainMenuMusicWithDelayRoutine(duration));

    }

    private IEnumerator PlayMainMenuMusicWithDelayRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayMainMenuMusic();
    }

    private void PlayMainMenuMusic()
    {
        var clip = GetRandomClip(mainMenuMusic);
        PlayBGMMusic(clip, true);
    }

    private void PlayBattleMusic()
    {
        StopAudioRoutine();
        var clip = GetRandomClip(battleMusic);
        PlayBGMMusic(clip, true);
    }

    private void PlayGameOverMusic()
    {
        StopAudioRoutine();
        var clip = GetRandomClip(gameOverMusic);
        PlayBGMMusic(clip, false);
    }


    // Las funciones sgtes son gen�ricas para cualquier juego - - - - - - - -
    private float PlayRandomSound(AudioClip[] clips, AudioSource audioSource, bool randomPitch = false, float volumeScale = 1f)
    {
        if (clips == null || clips.Length == 0) return 0f; // Programaci�n defensiva nunca est� de m�s
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip, audioSource, randomPitch, volumeScale);
        return clip.length;
    }


    void PlayRandomSoundWithDelay(AudioClip[] clips, AudioSource audioSource, float delay, bool randomPitch = false, float volumeScale = 1f)
    {
        StartCoroutine(PlayRandomSoundWithDelayRoutine(clips, audioSource, delay, randomPitch, volumeScale));
    }

    IEnumerator PlayRandomSoundWithDelayRoutine(AudioClip[] clips, AudioSource audioSource, float delay, bool randomPitch = false, float volumeScale = 1f)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomSound(clips, audioSource, randomPitch, volumeScale);
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        int randomIdx = Random.Range(0, audioClips.Length);
        return audioClips[randomIdx];
    }

    private void SFXPlayOneShot(AudioClip clip, AudioSource audioSource, bool randomPitch = false, float volumeScale = 1f)
    {
        if (!clipsPlayedThisFrame.Contains(clip))
        {
            SFXAudioSource.pitch = randomPitch ? GetRandomPitch() : 1f;
            audioSource.PlayOneShot(clip, volumeScale);
            clipsPlayedThisFrame.Add(clip);

            //print("SFXPlayOneShot " + clip.name + " pitch: " + SFXAudioSource.pitch + " volume: " + volumeScale);
            
        }
    }

    float GetRandomPitch()
    {
        return Random.Range(1f - pitchVariation, 1f + pitchVariation);
    }


    private void PlayBGMMusic(AudioClip clip, bool loop)
    {
        BGMAudioSource.loop = loop;
        BGMAudioSource.Stop();
        BGMAudioSource.clip = clip;
        BGMAudioSource.Play();
    }

    private void StopAudioRoutine()
    {
        if (audioRoutine != null)
            StopCoroutine(audioRoutine);
    }

    private void StopGameMusic()
    {
        StopAudioRoutine();
        BGMAudioSource.Stop();
    }

}
