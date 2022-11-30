using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const string GAMEDATA_KEY = "MonstersAndGunsData";
    public enum GameState { Initialization, MainMenu, PortalCreation, Spawning, Battle, BossBattle, GameOver, Win, Exit, Restart }

    [SerializeField] private GameManagerData gameManagerData;
    [SerializeField] private GameObject canvasLoading;
    
    // TODO: borrar los colliders de los objetos que no lo necesiten

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


    // Será singleton
    // Y también se configura que su orden de ejecución sea primero que el resto de los scripts que hacen uso de "GameManager.Instance"
    // porque no estoy haciendo uso de lazy instantiation
    private static GameManager instance = null;

    public static GameManager Instance  => instance;

    SceneController sceneController;
    GameState currentState;
    GameObject player;
    Transform portal;
    List<MonsterController> monsters;
    Camera arCamera;
    GameplayData gameplayData;

    public Transform Portal => portal;
    public Camera ARCamera => arCamera;
    public Vector3 PlayerForward => player.transform.forward;
    public Vector3 PlayerPosition => player ? player.transform.position : Vector3.zero;

    private int Score 
    {
        get { return gameplayData.Score; }
        set 
        { 
            gameplayData.Score = value;
            OnScoreUpdated?.Invoke(gameplayData.Score);
        }
    }

    GameState CurrentState
    {
        get { return currentState; }
        set 
        {
            currentState = value;
            
            switch (currentState)
            {
                case GameState.Initialization:
                    StartCoroutine(InitializationRoutine());
                    break;
                case GameState.MainMenu:
                    if (canvasLoading)
                        canvasLoading.SetActive(false);
                    OnMainMenuActivating?.Invoke();
                    break;
                case GameState.PortalCreation:
                    OnPortalCreating?.Invoke();
                    break;
                case GameState.Spawning:
                    OnSpawning?.Invoke(gameplayData.Level, portal.position, portal.rotation);
                    break;
                case GameState.Battle:
                    StartCoroutine(BattleRoutine());
                    break;
                case GameState.BossBattle:
                    break;
                case GameState.GameOver:
                    StartCoroutine(GameOverRoutine(gameManagerData.waitBeforeGameOver));
                    break;
                case GameState.Win:
                    break;
                case GameState.Exit:
                    break;
                case GameState.Restart:
                    Restart();
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator BattleRoutine()
    {        
        yield return new WaitForSeconds(gameManagerData.waitBeforeInitBattle); // Se les da tiempo a los monsters de moverse un poco antes de dispararles
        OnBattling?.Invoke(monsters, gameplayData.Level);
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        sceneController = FindObjectOfType<SceneController>();
    }


    private void Start()
    {
        CurrentState = GameState.Initialization;
    }

    public void CloseApp() => Application.Quit();
    
    public void GameStarted()
    {
        if (currentState != GameState.MainMenu) return;

        CurrentState = GameState.PortalCreation;        
    }

    public void GameRestarted()
    {
        if (currentState != GameState.GameOver) return;

        CurrentState = GameState.Restart;
    }

    public void PortalCreated(Transform portal)
    {
        if (currentState != GameState.PortalCreation) return;

        this.portal = portal;
        OnPortalCreated?.Invoke();
        CurrentState = GameState.Spawning;        
    }

    public void MonstersSpawned()
    {
        if (currentState != GameState.Spawning) return;

        CurrentState = GameState.Battle;        
    }

    public void DeadNotification(HealthController deadObject, DamageMode damageMode)
    {
        if (deadObject.CompareTag("Monster"))
        {
            var monster = deadObject.GetComponent<MonsterController>();

            if (damageMode == DamageMode.Shooting)
                Score += monster.Score;

            monsters?.Remove(monster);
            OnMonsterDead?.Invoke(monster);
            Destroy(deadObject.gameObject);
            
            if (monsters.Count == 0)
            {
                CurrentState = GameState.BossBattle;
            }

        }
        else if (deadObject.CompareTag("Player"))
        {            
            OnPlayerDead?.Invoke();
            CurrentState = GameState.GameOver;
        }
    }

    IEnumerator GameOverRoutine(float delayGameOver)
    {
        yield return new WaitForSeconds(delayGameOver);
        OnGameOver?.Invoke();
    }

    public void DamageNotification(HealthController deadObject)
    {
        if (deadObject.CompareTag("Monster"))
        {
            OnMonsterDamage?.Invoke();
        }
        else if (deadObject.CompareTag("Player"))
        {
            OnPlayerDamage?.Invoke(deadObject.CurrentHealthPercentage);
        }
    }

    public void StatusPortal(bool status)
    {
        OnStatusPortalChanged?.Invoke(status);
    }

    public void GunFired(int gunIndex)
    {
        OnGunFired?.Invoke(gunIndex);
    }

    public void MonsterCreated(MonsterController monster)
    {
        monsters.Add(monster);
        OnMonsterCreated?.Invoke();
    }

    IEnumerator InitializationRoutine()
    {
        monsters = new List<MonsterController>();

        for (int i = 0; i < gameManagerData.scenesToLoad.Length; i++)
        {
            yield return sceneController.LoadSceneAdditive(gameManagerData.scenesToLoad[i]);        
        }

        player = GameObject.FindGameObjectWithTag("Player");
        arCamera = Camera.main;
        gameplayData = GameDataRepository.GetById(GAMEDATA_KEY);
        OnScoreUpdated?.Invoke(gameplayData.Score);

        CurrentState = GameState.MainMenu;        
    }

    void Restart()
    {
        for (int i = 0; i < monsters.Count; i++)
            Destroy(monsters[i].gameObject);

        monsters.Clear();

        gameplayData.Level = 1;
        Score = 0;

        OnRestart?.Invoke();

        CurrentState = GameState.PortalCreation;
    }
}
