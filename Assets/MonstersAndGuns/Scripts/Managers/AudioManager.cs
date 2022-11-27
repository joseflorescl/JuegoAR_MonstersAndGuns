using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] protected AudioSource BGMAudioSource;
    [SerializeField] protected AudioSource SFXAudioSource;

    [Header("BGM Sounds")]
    public AudioClip[] mainMenuMusic;

    [Space(10)]
    [Header("SFX Sounds")]
    public AudioClip[] pressStartGame;

    HashSet<AudioClip> clipsPlayedThisFrame;
    Coroutine audioRoutine;

    private void Awake() => clipsPlayedThisFrame = new HashSet<AudioClip>();

    private void LateUpdate() => clipsPlayedThisFrame.Clear();

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreationHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreationHandler;
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


    // Las funciones sgtes son genéricas para cualquier juego
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

    private float PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return 0f; // Programación defensiva nunca está de más
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip);
        return clip.length;
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
