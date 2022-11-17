using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Start, Menu, PortalCreation, Spawning, Battle, BossBattle, GameOver, Win }

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
        gameState = GameState.Start;
        StartCoroutine(StartRoutine());
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


    IEnumerator StartRoutine()
    {
        yield return null;
        sceneController.LoadARSession();
        sceneController.LoadMenu();
        gameState = GameState.Menu;
    }

    




}
