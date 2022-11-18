using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Initialization, Menu, PortalCreation, Spawning, Battle, BossBattle, GameOver, Win }

    // TODO: crear evento OnInitialization que se use para que todos los submanagers (UIManager, AudioManager, etc)
    //  seteen las acciones necesarios al arrancar el juego
    // TODO: validar que en este nuevo esquema de comunicación entre GameManagers con los submanagers, usando eventos, 
    //  deberíamos asegurarnos que los submanagers son los que hacen uso del GameManager, pero no al revés, es decir
    //  que el GameManager NO hace uso (no sabe de la existencia) de los submanagers.

    public event Action OnPortalCreation;


    // Será singleton
    private static GameManager instance = null;

   

    public static GameManager Instance  => instance;

    SceneController sceneController;

    GameState gameState;

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
        StartCoroutine(InitializationRoutine());
    }



    public void CloseApp()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        if (gameState != GameState.Menu) return;

        print(GetType().Name + " StartGame");

        gameState = GameState.PortalCreation;
        OnPortalCreation?.Invoke();
    }


    IEnumerator InitializationRoutine()
    {
        // TODO: leer la doc de SceneManager.LoadScene y validar si se debe move el yield más abajo
        yield return null;
        sceneController.LoadARSession();
        sceneController.LoadMenu();
        gameState = GameState.Menu;
    }

    




}
