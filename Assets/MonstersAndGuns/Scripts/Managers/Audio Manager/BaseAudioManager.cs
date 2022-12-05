using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAudioManager : MonoBehaviour
{
    [SerializeField] protected AudioSource BGMAudioSource;
    [SerializeField] protected float pitchVariation = 0.2f;

    protected Coroutine audioRoutine;
    HashSet<AudioClip> clipsPlayedThisFrame;

    protected void Awake() => clipsPlayedThisFrame = new HashSet<AudioClip>();

    protected void LateUpdate() => clipsPlayedThisFrame.Clear();

    protected float PlayRandomMusic(AudioClip[] clips, bool loop)
    {
        StopAudioRoutine();
        var clip = GetRandomClip(clips);
        PlayBGMMusic(clip, loop);
        return clip.length;
    }

    protected void PlayRandomSoundWhitLoop(AudioClip[] clips, AudioSource audioSource)
    {
        if (clips == null || clips.Length == 0) return; // Programación defensiva nunca está de más
        var clip = GetRandomClip(clips);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    protected float PlayRandomSound(AudioClip[] clips, AudioSource audioSource, bool randomPitch = false, float volumeScale = 1f)
    {
        if (clips == null || clips.Length == 0) return 0f; // Programación defensiva nunca está de más
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip, audioSource, randomPitch, volumeScale);
        return clip.length;
    }

    protected void PlayRandomSoundWithDelay(AudioClip[] clips, AudioSource audioSource, float delay, bool randomPitch = false, float volumeScale = 1f)
    {
        StartCoroutine(PlayRandomSoundWithDelayRoutine(clips, audioSource, delay, randomPitch, volumeScale));
    }

    IEnumerator PlayRandomSoundWithDelayRoutine(AudioClip[] clips, AudioSource audioSource, float delay, bool randomPitch = false, float volumeScale = 1f)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomSound(clips, audioSource, randomPitch, volumeScale);
    }

    protected AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        int randomIdx = Random.Range(0, audioClips.Length);
        return audioClips[randomIdx];
    }

    void SFXPlayOneShot(AudioClip clip, AudioSource audioSource, bool randomPitch = false, float volumeScale = 1f)
    {
        if (!clipsPlayedThisFrame.Contains(clip))
        {
            audioSource.pitch = randomPitch ? GetRandomPitch() : 1f;            
            audioSource.PlayOneShot(clip, volumeScale);
            clipsPlayedThisFrame.Add(clip);
            //print("SFXPlayOneShot " + clip.name + " pitch: " + SFXAudioSource.pitch + " volume: " + volumeScale);
        }
    }

    float GetRandomPitch()
    {
        return Random.Range(1f - pitchVariation, 1f + pitchVariation);
    }


    protected void PlayBGMMusic(AudioClip clip, bool loop)
    {
        BGMAudioSource.loop = loop;
        BGMAudioSource.Stop();
        BGMAudioSource.clip = clip;
        BGMAudioSource.Play();
    }

    protected void StopAudioRoutine()
    {
        if (audioRoutine != null)
            StopCoroutine(audioRoutine);
    }

    protected void StopGameMusic()
    {
        StopAudioRoutine();
        BGMAudioSource.Stop();
    }

    
}
