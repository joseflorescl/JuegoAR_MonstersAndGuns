using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioSource SFXAudioSource;

    [Header("BGM Sounds")]
    public AudioClip[] mainMenuMusic;

    [Space(10)]
    [Header("SFX Sounds")]
    [SerializeField] private AudioClip[] pressStartGame;
    [SerializeField] private AudioClip[] popSound;

    HashSet<AudioClip> clipsPlayedThisFrame;
    Coroutine audioRoutine;

    private void Awake() => clipsPlayedThisFrame = new HashSet<AudioClip>();

    private void LateUpdate() => clipsPlayedThisFrame.Clear();

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreationHandler;
        GameManager.Instance.OnMonsterCreated += MonsterCreatedHandler;

    }

    
    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreationHandler;
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;

    }

    private void MonsterCreatedHandler()
    {
        print("Pop");
        PlayRandomSound(popSound);
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


    // Las funciones sgtes son gen�ricas para cualquier juego - - - - - - - -
    private float PlayRandomSound(AudioClip[] clips)
    {
        // TODO: agregar parametro bool para indicam si se quiere modificar aleatoriamente el pitch, para as� tener m�s variedad de sonidos usando solo 1 clip
        if (clips == null || clips.Length == 0) return 0f; // Programaci�n defensiva nunca est� de m�s
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip);
        return clip.length;
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        int randomIdx = Random.Range(0, audioClips.Length);
        return audioClips[randomIdx];
    }

    private void SFXPlayOneShot(AudioClip clip)
    {
        if (!clipsPlayedThisFrame.Contains(clip))
        {
            SFXAudioSource.PlayOneShot(clip);
            clipsPlayedThisFrame.Add(clip);
        }
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
