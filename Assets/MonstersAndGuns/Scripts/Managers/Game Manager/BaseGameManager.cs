using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    protected const string GAMEDATA_KEY = "MonstersAndGunsData";
    public enum GameState { Initialization, MainMenu, PortalCreation, Spawning, Battle, BossBattle, 
        GameOver, Win, Exit, Restart, NextLevel }

    // Nomenclatura de eventos: ejemplo
    //   OnClosing: a close event that is raised before a window is closed
    //   OnClosed: and one that is raised after the window is closed 
    public event Action OnMainMenuActivating;
    public event Action OnPortalCreating; // Al inicio de la creación del portal en la escena
    public event Action OnPortalCreated; // Una vez que el portal ya ha sido creado
    public event Action<int, Vector3, Quaternion> OnSpawning; // Recibe el level actual del juego, y lo posición/rotación desde donde hacer el spawner
    public event Action<List<MonsterController>, int> OnBattling; // Recibe la lista de monsters creados y el level actual del juego
    public event Action OnMonsterCreated;
    public event Action<BaseMonsterController> OnMonsterDead;
    public event Action<BaseMonsterController> OnMonsterAttacking;
    public event Action<BaseMonsterController> OnBossMonsterDead;
    public event Action<BaseMonsterController> OnBossMonsterDamage;
    public event Action<BaseMonsterController> OnMonsterDamage;
    public event Action<MissileController> OnMissileDead;
    public event Action<float> OnPlayerDamage;
    public event Action OnPlayerDead;
    public event Action<bool> OnStatusPortalChanged; // La idea es que la UI refleje cuando el portal está activo/inactivo con un texto diferente en cada caso
    public event Action<int> OnPlayerFired;
    public event Action OnMonsterFired;
    public event Action<float> OnGameOver; // El parámetro es el delay para que los efectos de sonido/UI esperen un poco antes de activarse
    public event Action OnRestart;
    public event Action<int> OnScoreUpdated;
    public event Action OnMonstersSpawned;
    public event Action OnBossMonsterSpawned;
    public event Action OnBossBattle;
    public event Action OnWinLevel;
    public event Action OnScoreIncrementing;
    public event Action OnScoreIncremented;
    public event Action<int> OnNextLevel;

    protected void RaiseMainMenuActivating() => OnMainMenuActivating?.Invoke();
    protected void RaisePortalCreating() => OnPortalCreating?.Invoke();
    protected void RaisePortalCreated() => OnPortalCreated?.Invoke();
    protected void RaiseSpawning(int level, Vector3 position, Quaternion rotation) => OnSpawning?.Invoke(level, position, rotation);
    protected void RaiseBattling(List<MonsterController> monsters, int level) => OnBattling?.Invoke(monsters, level);
    protected void RaiseMonsterCreated() => OnMonsterCreated?.Invoke();
    protected void RaiseMonsterDead(BaseMonsterController monster) => OnMonsterDead?.Invoke(monster);
    protected void RaiseMonsterAttacking(BaseMonsterController monster) => OnMonsterAttacking?.Invoke(monster);
    protected void RaiseBossMonsterDead(BaseMonsterController bossMonster) => OnBossMonsterDead?.Invoke(bossMonster);
    protected void RaiseBossMonsterDamage(BaseMonsterController bossMonster) => OnBossMonsterDamage?.Invoke(bossMonster);
    protected void RaiseMonsterDamage(BaseMonsterController monster) => OnMonsterDamage?.Invoke(monster);
    protected void RaiseMissileDead(MissileController missil) => OnMissileDead?.Invoke(missil);
    protected void RaisePlayerDamage(float currentHealthPercentage) => OnPlayerDamage?.Invoke(currentHealthPercentage);
    protected void RaisePlayerDead() => OnPlayerDead?.Invoke();
    protected void RaiseStatusPortalChanged(bool status) => OnStatusPortalChanged.Invoke(status);
    protected void RaisePlayerFired(int gunIndex) => OnPlayerFired?.Invoke(gunIndex);
    protected void RaiseMonsterFired() => OnMonsterFired?.Invoke();
    protected void RaiseGameOver(float delay) => OnGameOver?.Invoke(delay);
    protected void RaiseRestart() => OnRestart?.Invoke();
    protected void RaiseScoreUpdated(int score) => OnScoreUpdated?.Invoke(score);
    protected void RaiseMonstersSpawned() => OnMonstersSpawned?.Invoke();
    protected void RaiseBossMonsterSpawned() => OnBossMonsterSpawned?.Invoke();
    protected void RaiseBossBattle() => OnBossBattle?.Invoke();
    protected void RaiseWinLevel() => OnWinLevel?.Invoke();
    protected void RaiseScoreIncrementing() => OnScoreIncrementing?.Invoke();
    protected void RaiseScoreIncremented() => OnScoreIncremented?.Invoke();
    protected void RaiseOnNextLevel(int nextLevel) => OnNextLevel?.Invoke(nextLevel);

    protected Transform portal;
    protected Camera arCamera;
    protected Transform player;
    protected List<MonsterController> monsters;
    protected BossMonsterController bossMonster;
    protected GameplayData gameplayData;
    protected SceneController sceneController;

    //NO VA: public List<MonsterController> Monsters => monsters; //TODO: cambiar el tipo de dato por un enumerable de solo lectura
    public ReadOnlyCollection<MonsterController> Monsters => monsters.AsReadOnly();
    public BossMonsterController BossMonster => bossMonster;
    public Transform Player => player;
    public Transform Portal => portal;
    public Camera ARCamera => arCamera;
    public Vector3 PlayerForward => player.forward;
    public Vector3 PlayerPosition => player ? player.position : Vector3.zero;
    protected int Score
    {
        get { return gameplayData.Score; }
        set
        {
            gameplayData.Score = value;
            RaiseScoreUpdated(gameplayData.Score);
        }
    }

    private GameState currentState;

    protected GameState CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;

            switch (currentState)
            {
                case GameState.Initialization:
                    Initialization();
                    break;
                case GameState.MainMenu:
                    MainMenu();
                    break;
                case GameState.PortalCreation:
                    PortalCreation();
                    break;
                case GameState.Spawning:
                    Spawning();
                    break;
                case GameState.Battle:
                    Battle();
                    break;
                case GameState.BossBattle:
                    BossBattle();
                    break;
                case GameState.GameOver:
                    GameOver();
                    break;
                case GameState.Win:
                    Win();
                    break;
                case GameState.Exit:
                    Exit();
                    break;
                case GameState.Restart:
                    Restart();
                    break;
                case GameState.NextLevel:
                    NextLevel();
                    break;
                default:
                    break;
            }
        }
    }

    protected abstract void Initialization();
    protected abstract void MainMenu();
    protected abstract void PortalCreation();
    protected abstract void Spawning();
    protected abstract void Battle();
    protected abstract void BossBattle();
    protected abstract void GameOver();
    protected abstract void Win();
    protected abstract void Exit();
    protected abstract void Restart();
    protected abstract void NextLevel();

}
