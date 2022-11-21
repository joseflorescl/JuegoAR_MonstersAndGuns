using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Initialization, MainMenu, PortalCreation, Spawning, Battle, BossBattle, GameOver, Win, Exit }

    [SerializeField] private GameObject canvasLoading;
    [SerializeField] private string[] scenesToLoad;

    // TODO: validar que en este nuevo esquema de comunicación entre GameManagers con los submanagers, usando eventos, 
    //  deberíamos asegurarnos que los submanagers son los que hacen uso del GameManager, pero no al revés, es decir
    //  que el GameManager NO hace uso (no sabe de la existencia) de los submanagers.

    // TODO: borrar los colliders de los objetos que no lo necesiten

    public event Action OnMainMenu;
    public event Action OnPortalCreation;
    public event Action<int, Vector3, Quaternion> OnSpawning; // Recibe el level actual del juego, y lo posición/rotación desde donde hacer el spawner
    public event Action<List<MonsterController>, int> OnBattle; // Recibe la lista de enemigos creados y el level actual del juego
    public event Action OnEnemyDead;



    // Será singleton
    // Y también se configura que su orden de ejecución sea primero que el resto de los scripts que hacen uso de GameManager.Instance
    // porque no hago uso de lazy instantiation
    private static GameManager instance = null;

    public static GameManager Instance  => instance;

    SceneController sceneController;

    GameState gameState;
    int currentLevel;
    GameObject player;
    Transform portal;
    List<MonsterController> enemies;


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
        gameState = GameState.Initialization;
        currentLevel = 1;
        StartCoroutine(InitializationRoutine());
    }



    public void CloseApp()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        if (gameState != GameState.MainMenu) return;

        print(GetType().Name + " StartGame");

        gameState = GameState.PortalCreation;
        OnPortalCreation?.Invoke();
    }

    public void PortalCreated(Transform portal)
    {
        if (gameState != GameState.PortalCreation) return;

        gameState = GameState.Spawning;
        this.portal = portal;
        
        OnSpawning?.Invoke(currentLevel, portal.position, portal.rotation);

    }

    public void EnemiesSpawned(List<MonsterController> enemies)
    {
        if (gameState != GameState.Spawning) { print("EnemiesSpawned en estado incorrecto"); return; }

        print("EnemiesSpawned Pre invoke de OnBattle");
        // Con esto el GM puede comenzar el estado de Battle, gatillando un evento de OnBattle(Lista de enemigos, level del juego)
        gameState = GameState.Battle;
        this.enemies = enemies;
        OnBattle?.Invoke(enemies, currentLevel);
    }

    public void EnemyDead(MonsterController enemy)
    {
        enemies.Remove(enemy);
        OnEnemyDead?.Invoke();
    }

    public Vector3 PlayerPosition()
    {
        if (player)
        {
            return player.transform.position;
        }
        else
            return Vector3.zero;
    }

    public Vector3 PortalForwardDirection()
    {
        if (portal)
        {
            return portal.transform.forward;
        }
        else
            return Vector3.forward;
    }

    IEnumerator InitializationRoutine()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            yield return sceneController.LoadSceneAdditive(scenesToLoad[i]);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        gameState = GameState.MainMenu;

        if (canvasLoading)
            canvasLoading.SetActive(false);

        OnMainMenu?.Invoke();
    }

    




}
