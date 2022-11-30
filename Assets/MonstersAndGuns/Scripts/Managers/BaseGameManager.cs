using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameManager : MonoBehaviour
{
    protected const string GAMEDATA_KEY = "MonstersAndGunsData";
    public enum GameState { Initialization, MainMenu, PortalCreation, Spawning, Battle, BossBattle, GameOver, Win, Exit, Restart }

    // Nomenclatura de eventos: ejemplo
    //   OnClosing: a close event that is raised before a window is closed
    //   OnClosed: and one that is raised after the window is closed 
    public event Action OnMainMenuActivating;
    public event Action OnPortalCreating; // Al inicio de la creación del portal en la escena
    public event Action OnPortalCreated; // Una vez que el portal ya ha sido creado
    public event Action<int, Vector3, Quaternion> OnSpawning; // Recibe el level actual del juego, y lo posición/rotación desde donde hacer el spawner
    public event Action<List<MonsterController>, int> OnBattling; // Recibe la lista de monsters creados y el level actual del juego
    public event Action OnMonsterCreated;
    public event Action<MonsterController> OnMonsterDead;
    public event Action OnMonsterDamage;
    public event Action<float> OnPlayerDamage;
    public event Action OnPlayerDead;
    public event Action<bool> OnStatusPortalChanged; // La idea es que la UI refleje cuando el portal está activo/inactivo con un texto diferente en cada caso
    public event Action<int> OnGunFired;
    public event Action OnGameOver;
    public event Action OnRestart;
    public event Action<int> OnScoreUpdated;
    public event Action OnMonstersSpawned;

    protected void RaiseMainMenuActivating() => OnMainMenuActivating?.Invoke();
    protected void RaisePortalCreating() => OnPortalCreating?.Invoke();
    protected void RaisePortalCreated() => OnPortalCreated?.Invoke();
    protected void RaiseSpawning(int level, Vector3 position, Quaternion rotation) => OnSpawning?.Invoke(level, position, rotation);
    protected void RaiseBattling(List<MonsterController> monsters, int level) => OnBattling?.Invoke(monsters, level);
    protected void RaiseMonsterCreated() => OnMonsterCreated?.Invoke();
    protected void RaiseMonsterDead(MonsterController monster) => OnMonsterDead?.Invoke(monster);
    protected void RaiseMonsterDamage() => OnMonsterDamage?.Invoke();
    protected void RaisePlayerDamage(float currentHealthPercentage) => OnPlayerDamage?.Invoke(currentHealthPercentage);
    protected void RaisePlayerDead() => OnPlayerDead?.Invoke();
    protected void RaiseStatusPortalChanged(bool status) => OnStatusPortalChanged.Invoke(status);
    protected void RaiseGunFired(int gunIndex) => OnGunFired?.Invoke(gunIndex);
    protected void RaiseGameOver() => OnGameOver?.Invoke();
    protected void RaiseRestart() => OnRestart?.Invoke();
    protected void RaiseScoreUpdated(int score) => OnScoreUpdated?.Invoke(score);
    protected void RaiseMonstersSpawned() => OnMonstersSpawned?.Invoke();

    protected Transform portal;
    protected Camera arCamera;
    protected GameObject player;
    protected List<MonsterController> monsters;
    protected GameplayData gameplayData;
    protected SceneController sceneController;

    public Transform Portal => portal;
    public Camera ARCamera => arCamera;
    public Vector3 PlayerForward => player.transform.forward;
    public Vector3 PlayerPosition => player ? player.transform.position : Vector3.zero;
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

}
