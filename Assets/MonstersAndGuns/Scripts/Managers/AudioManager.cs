using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] protected AudioSource BGMAudioSource;
    [SerializeField] protected AudioSource SFXAudioSource;

    // TODO: esta data se puede pasar a Scriptable Object
    [Header("BGM Sounds")]
    public AudioClip[] menuMusic;

    [Space(10)]
    [Header("SFX Sounds")]
    public AudioClip[] pressStartGame;



    HashSet<AudioClip> clipsPlayedThisFrame;

    private void Awake()
    {
        clipsPlayedThisFrame = new HashSet<AudioClip>();
    }

    private void LateUpdate()
    {
        clipsPlayedThisFrame.Clear();
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

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return; // Programación defensiva nunca está de más
        var clip = GetRandomClip(clips);
        SFXPlayOneShot(clip);
    }

    private void PlayBGMMusic(AudioClip clip, bool loop)
    {
        BGMAudioSource.loop = loop;
        BGMAudioSource.Stop();
        BGMAudioSource.clip = clip;
        BGMAudioSource.Play();
    }





}
