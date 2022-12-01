using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AudioManager Data", menuName = "Monsters n Guns/Audio Manager Data")]
public class AudioManagerData : ScriptableObject
{

    [Header("BGM Sounds")]
    public AudioClip[] mainMenuMusic;
    public AudioClip[] battleMusic;
    public AudioClip[] bossBattleMusic;
    public AudioClip[] gameOverMusic;

    [Space(10)]
    [Header("SFX Sounds")]
    public AudioClip[] pressStartGame;
    public AudioClip[] monsterSpawned;
    public AudioClip[] monsterExplosions;
    public AudioClip[] bossMonsterSpawned;    
    public AudioClip[] bossMonsterDamage;
    public AudioClip[] bossMonsterExplosions;
    public AudioClip[] gunFired;
    public AudioClip[] goSound;
    public float volumeScaleGunFired = 0.25f;
    public AudioClip[] playerDamage;
    public float volumeScalePlayerDamage = 1f;
    public float delayPlayerDamageSound = 0.2f;
    public AudioClip[] playerDead;
    public float volumeScalePlayerDead = 1f;
    public float delayPlayerDeadSound = 0.2f;
}
