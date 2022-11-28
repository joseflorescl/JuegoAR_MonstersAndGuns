using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioSource SFXAudioSource;
    [SerializeField] private float pitchVariation = 0.2f;

    [Header("BGM Sounds")]
    public AudioClip[] mainMenuMusic;
    public AudioClip[] battleMusic;

    [Space(10)]
    [Header("SFX Sounds")]
    [SerializeField] private AudioClip[] pressStartGame;
    [SerializeField] private AudioClip[] popSound;
    [SerializeField] private AudioClip[] monsterExplosions;
    [SerializeField] private AudioClip[] gunFired;

    HashSet<AudioClip> clipsPlayedThisFrame;
    Coroutine audioRoutine;

    private void Awake() => clipsPlayedThisFrame = new HashSet<AudioClip>();

    private void LateUpdate() => clipsPlayedThisFrame.Clear();

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreationHandler;
        GameManager.Instance.OnMonsterCreated += MonsterCreatedHandler;
        GameManager.Instance.OnSpawning += SpawningHandler;
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnGunFired += GunFiredHandler;


    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreationHandler;
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;
        GameManager.Instance.OnSpawning -= SpawningHandler;
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnGunFired -= GunFiredHandler;

    }

    private void GunFiredHandler(int obj)
    {
        PlayRandomSound(gunFired, randomPitch: true);
    }

    private void MonsterDeadHandler(MonsterController obj)
    {
        PlayRandomSound(monsterExplosions);
    }

    private void SpawningHandler(int arg1, Vector3 arg2, Quaternion arg3)
    {
        PlayBattleMusic(); // Queda mejor colocar la música de batalla al inicio del spawning
    }


    private void MonsterCreatedHandler()
    {
        PlayRandomSound(popSound, randomPitch: true);
    }



    private void MainMenuHandler()
    {
        PlayMainMenuMusic();
    }


    private void PortalCreationHandler()
    {
        BGMAudioSource.Stop();
        SFXAudioSource.Stop();
        float duration = PlayRandomSound(pressStartGame);

        StopAudioRoutine();
        audioRoutine = StartCoroutine(PlayMainMenuMusicAfterDurationRoutine(duration));

    }

    private IEnumerator PlayMainMenuMusicAfterDurationRoutine(float duration)
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
        var clip = GetRandomClip(battleMusic);
        PlayBGMMusic(clip, true);
    }


    // Las funciones sgtes son genéricas para cualquier juego - - - - - - - -
    private float PlayRandomSound(AudioClip[] clips, bool randomPitch = false)
    {
        // TODO: agregar parametro bool para indicam si se quiere modificar aleatoriamente el pitch, para así tener más variedad de sonidos usando solo 1 clip
        if (clips == null || clips.Length == 0) return 0f; // Programación defensiva nunca está de más
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip, randomPitch);
        return clip.length;
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        int randomIdx = Random.Range(0, audioClips.Length);
        return audioClips[randomIdx];
    }

    private void SFXPlayOneShot(AudioClip clip, bool randomPitch = false)
    {
        if (!clipsPlayedThisFrame.Contains(clip))
        {
            SFXAudioSource.pitch = randomPitch ? GetRandomPitch() : 1f;
            SFXAudioSource.PlayOneShot(clip);
            clipsPlayedThisFrame.Add(clip);
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

}
