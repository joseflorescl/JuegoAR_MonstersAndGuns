using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Initialization, MainMenu, PortalCreation, Spawning, Battle, BossBattle, GameOver, Win, Exit }

    [SerializeField] private GameObject canvasLoading;

    // TODO: validar que en este nuevo esquema de comunicaci�n entre GameManagers con los submanagers, usando eventos, 
    //  deber�amos asegurarnos que los submanagers son los que hacen uso del GameManager, pero no al rev�s, es decir
    //  que el GameManager NO hace uso (no sabe de la existencia) de los submanagers.

    public event Action OnMainMenu;
    public event Action OnPortalCreation;
    public event Action<int, Vector3, Quaternion> OnSpawning; // Recibe el level actual del juego, y lo posici�n/rotaci�n desde donde hacer el spawner


    // Ser� singleton
    private static GameManager instance = null;

    public static GameManager Instance  => instance;

    SceneController sceneController;

    GameState gameState;
    int currentLevel;
    

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
        
        OnSpawning?.Invoke(currentLevel, portal.position, portal.rotation);

    }


    IEnumerator InitializationRoutine()
    {
        yield return sceneController.LoadARSessionRoutine();
        yield return sceneController.LoadMenu();
        gameState = GameState.MainMenu;
        canvasLoading.SetActive(false);
        OnMainMenu?.Invoke();
    }

    




}
