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

    // Nomenclatura de eventos:
    //   OnClosing: a close event that is raised before a window is closed
    //   OnClosed: and one that is raised after the window is closed 
    public event Action OnMainMenuActivating;
    public event Action OnPortalCreating; // Al inicio de la creación del portal en la escena
    public event Action OnPortalCreated; // Una vez que el portal ya ha sido creado
    public event Action<int, Vector3, Quaternion> OnSpawning; // Recibe el level actual del juego, y lo posición/rotación desde donde hacer el spawner
    public event Action<List<MonsterController>, int> OnBattling; // Recibe la lista de enemigos creados y el level actual del juego
    public event Action OnEnemyDead;
    public event Action<bool> OnStatusPortalChanged; // La idea es que la UI refleje cuando el portal está activo/inactivo con un texto diferente en cada caso




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
        OnPortalCreating?.Invoke();
    }

    public void PortalCreated(Transform portal)
    {
        if (gameState != GameState.PortalCreation) return;

        OnPortalCreated?.Invoke();

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
        OnBattling?.Invoke(enemies, currentLevel);
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

    public void StatusPortal(bool status)
    {
        // TODO: lanzar evento para que la UI lo maneje
        OnStatusPortalChanged?.Invoke(status);
    }

    IEnumerator InitializationRoutine()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            yield return sceneController.LoadSceneAdditive(scenesToLoad[i]);        
        }

        //TODO: se debería esperar un ratito para que la cámara del móbil se active bien
        //yield return new WaitForSeconds(5);

        player = GameObject.FindGameObjectWithTag("Player");
        gameState = GameState.MainMenu;

        if (canvasLoading)
            canvasLoading.SetActive(false);

        OnMainMenuActivating?.Invoke();
    }

    




}
