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
    [SerializeField] private float waitBeforeInitBattle = 2f;

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
    public event Action<List<MonsterController>, int> OnBattling; // Recibe la lista de monsters creados y el level actual del juego
    public event Action<GameObject> OnMonsterDead;
    public event Action OnMonsterDamage;
    public event Action<bool> OnStatusPortalChanged; // La idea es que la UI refleje cuando el portal está activo/inactivo con un texto diferente en cada caso
    public event Action<int> OnGunFired;





    // Será singleton
    // Y también se configura que su orden de ejecución sea primero que el resto de los scripts que hacen uso de GameManager.Instance
    // porque no hago uso de lazy instantiation
    private static GameManager instance = null;

    public static GameManager Instance  => instance;

    SceneController sceneController;
    GameState currentState;
    int currentLevel;
    GameObject player;
    Transform portal;
    List<MonsterController> monsters;
    Camera arCamera;

    GameState CurrentState
    {
        get { return currentState; }
        set 
        {
            currentState = value;

            //StopAllCoroutines(); // Esto se debe validar, no siempre hay que detener las corutinas del GM
            // TODO: colocar el código repartido abajo en una función/corutina y llamarla desde este switch.
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
                    OnSpawning?.Invoke(currentLevel, portal.position, portal.rotation);
                    break;
                case GameState.Battle:
                    StartCoroutine(BattleRoutine());
                    break;
                case GameState.BossBattle:
                    break;
                case GameState.GameOver:
                    break;
                case GameState.Win:
                    break;
                case GameState.Exit:
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator BattleRoutine()
    {
        // Se espera un ratito antes de pasar al estado Battle, para darle tiemppo a los monsters de moverse un poco
        yield return new WaitForSeconds(waitBeforeInitBattle);
        OnBattling?.Invoke(monsters, currentLevel);
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

    public void PortalCreated(Transform portal)
    {
        if (currentState != GameState.PortalCreation) return;
        this.portal = portal;
        OnPortalCreated?.Invoke();

        CurrentState = GameState.Spawning;        
    }

    public void MonstersSpawned(List<MonsterController> monsters)
    {
        if (currentState != GameState.Spawning) return;

        this.monsters = monsters;
        
        CurrentState = GameState.Battle;        
    }

    public void DeadNotification(GameObject deadObject)
    {
        if (deadObject.CompareTag("Monster"))
        {
            monsters?.Remove(deadObject.GetComponent<MonsterController>());
            OnMonsterDead?.Invoke(deadObject);
        }
    }

    public void DamageNotification(GameObject deadObject)
    {
        if (deadObject.CompareTag("Monster"))
        {
            OnMonsterDamage?.Invoke();
        }
    }


    public Vector3 PlayerPosition()
    {
        if (player)
            return player.transform.position;
        else
        {
            print("No player yet");
            return Vector3.zero;
        }
    }

    public Transform Player()
    {
        return player.transform;
    }

    // TODO: borra esta función
    public Vector3 PortalForwardDirection()
    {
        if (portal)
            return portal.transform.forward;
        else
            return Vector3.forward;
    }

    public void StatusPortal(bool status)
    {
        // TODO: lanzar evento para que la UI lo maneje
        OnStatusPortalChanged?.Invoke(status);
    }

    public void GunFired(int gunIndex)
    {
        OnGunFired?.Invoke(gunIndex);
    }

    IEnumerator InitializationRoutine()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            yield return sceneController.LoadSceneAdditive(scenesToLoad[i]);        
        }

        //TODO: se debería esperar un ratito para que la cámara del móbil se active bien
        //yield return new WaitForSeconds(5);
        //  o lo que hago ahora es dejar la cámara con el Solid Color rojo igual que la imagen de fade

        currentLevel = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        arCamera = Camera.main;

        CurrentState = GameState.MainMenu;        
    }

    public Camera GetARCamera()
    {
        return arCamera;
    }

}
