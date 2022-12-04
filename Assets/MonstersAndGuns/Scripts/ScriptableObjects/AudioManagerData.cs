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
    public AudioClip[] winMusic;

    [Space(10)]
    [Header("Gameplay SFX Sounds")]
    public AudioClip[] pressStartGame;   
    public AudioClip[] goSound;
    public AudioClip[] scoreIncrementSound;

    [Space(10)]
    [Header("Player SFX Sounds")]
    public AudioClip[] playerFired;
    public AudioClip[] playerDamage;
    public AudioClip[] playerDead;

    [Space(10)]
    [Header("Monsters SFX Sounds")]
    public AudioClip[] monsterSpawned;
    public AudioClip[] monsterExplosions;
    public AudioClip[] bossMonsterSpawned;
    public AudioClip[] bossMonsterDamage;
    public AudioClip[] bossMonsterExplosions;
    public AudioClip[] monsterFired;
    public AudioClip[] missileExplosions;
    public AudioClip[] monsterAttacking;

    [Space(10)]
    [Header("Settings")]
    public float volumeScalePlayerFired = 0.25f;
    public float volumeScalePlayerDamage = 1f;
    public float volumeScalePlayerDead = 1f;
    public float delayPlayerDamageSound = 0.2f;    
    public float delayPlayerDeadSound = 0.2f;
}
