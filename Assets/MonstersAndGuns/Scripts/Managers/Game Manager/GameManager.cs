using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseGameManager
{    
    [SerializeField] private GameManagerData gameManagerData;
    [SerializeField] private GameObject canvasLoading;

    bool isScoreIncrementEnded = false;
    bool isWinLevelMusicEnded = false;

    // Ser� singleton
    // Y tambi�n se configura que su orden de ejecuci�n sea primero que el resto de los scripts que hacen uso de "GameManager.Instance"
    // porque no estoy haciendo uso de lazy instantiation
    private static GameManager instance = null;

    public static GameManager Instance => instance;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        sceneController = FindObjectOfType<SceneController>();
    }
    

    private void Start()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 30;
#endif
        CurrentState = GameState.Initialization;
    }

    protected override void Initialization() => StartCoroutine(InitializationRoutine());
    
    protected override void MainMenu()
    {
        if (canvasLoading)
            canvasLoading.SetActive(false);
        RaiseMainMenuActivating();
    }

    protected override void PortalCreation() => RaisePortalCreating();
    
    protected override void Spawning()
    {
        isScoreIncrementEnded = false;
        isWinLevelMusicEnded = false;
        RaiseSpawning(gameplayData.Level, portal.position, portal.rotation);
    }

    protected override void Battle() => StartCoroutine(BattleRoutine());
    
    protected override void BossBattle() => RaiseBossBattle();
    
    protected override void GameOver() => RaiseGameOver(gameManagerData.waitBeforeShowGameOver);        
    
    protected override void Win() => RaiseWinLevel();
    
    protected override void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    protected override void Restart()
    {                
        // Notar que los monsters y los misiles se destruyen solos en el evento Restart.
        monsters.Clear();
        missiles.Clear();
        bossMonster = null;

        gameplayData.Level = 1;
        Score = 0;

        RaiseRestart();

        CurrentState = GameState.PortalCreation;
    }

    protected override void NextLevel()
    {
        gameplayData.Level++;
        RaiseOnNextLevel(gameplayData.Level);
        CurrentState = GameState.Spawning;        
    }

    IEnumerator BattleRoutine()
    {        
        yield return new WaitForSeconds(gameManagerData.waitBeforeInitBattle); // Se les da tiempo a los monsters de moverse un poco antes de dispararles
        RaiseBattling(gameplayData.Level);
    }    

    IEnumerator InitializationRoutine()
    {
        monsters = new List<MonsterController>();
        missiles = new List<MissileController>();

        for (int i = 0; i < gameManagerData.scenesToLoad.Length; i++)
        {
            yield return sceneController.LoadSceneAdditive(gameManagerData.scenesToLoad[i]);        
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        arCamera = Camera.main;
        gameplayData = GameDataRepository.GetById(GAMEDATA_KEY);
        RaiseScoreUpdated(gameplayData.Score);

        CurrentState = GameState.MainMenu;
    }

    public void GameStarted()
    {
        if (CurrentState != GameState.MainMenu) return;

        CurrentState = GameState.PortalCreation;
    }

    public void GameRestarted()
    {
        if (CurrentState != GameState.GameOver) return;

        CurrentState = GameState.Restart;
    }

    public void PortalCreated(Transform portal)
    {
        if (CurrentState != GameState.PortalCreation) return;

        this.portal = portal;
        RaisePortalCreated();
        CurrentState = GameState.Spawning;
    }

    public void MonstersSpawned()
    {
        if (CurrentState != GameState.Spawning) return;

        RaiseMonstersSpawned();
        CurrentState = GameState.Battle;
    }


    public void BossMonsterSpawned()
    {
        if (CurrentState != GameState.BossBattle) return;

        RaiseBossMonsterSpawned();
    }

    public void DeadNotification(HealthController deadObject, DamageMode damageMode)
    {
        // Nota: si se quisiera eliminar este if-else por el uso de clases especializadas,
        //  habr�a que crear para cada if una clase que herede de HealthController,
        //  y que llame cada una a un m�todo espec�fico de GM, por ej: GameManager.Instance.PlayerHasDead()
        if (deadObject.CompareTag("Player")) 
        {
            RaisePlayerDead();
            CurrentState = GameState.GameOver;
        }
        else if (deadObject.CompareTag("Monster"))
        {
            var monster = deadObject.GetComponent<MonsterController>();

            if (damageMode == DamageMode.Shooting)
                Score += monster.Score;

            monsters?.Remove(monster);
            RaiseMonsterDead(monster);            
            PoolManager.Instance.Release(deadObject.gameObject);

            if (monsters.Count == 0 && CurrentState == GameState.Battle)
                CurrentState = GameState.BossBattle;
        }
        else if(deadObject.CompareTag("BossMonster"))
        {            
            var bossMonster = deadObject.GetComponent<BossMonsterController>();
            RaiseBossMonsterDead(bossMonster);            
            this.bossMonster = null;
            PoolManager.Instance.Release(deadObject.gameObject);

            if (damageMode == DamageMode.Shooting)
                Score += bossMonster.Score;

            CurrentState = GameState.Win;
        }        
        else if (deadObject.CompareTag("Missile"))
        {
            var missil = deadObject.GetComponent<MissileController>();
            missiles?.Remove(missil);
            RaiseMissileDead(missil);            
            PoolManager.Instance.Release(deadObject.gameObject);
        }

    }

    public void DamageNotification(HealthController deadObject)
    {
        if (deadObject.CompareTag("Monster"))
            RaiseMonsterDamage(deadObject.GetComponent<BaseMonsterController>());
        else if (deadObject.CompareTag("BossMonster"))
            RaiseBossMonsterDamage(deadObject.GetComponent<BossMonsterController>());
        else if (deadObject.CompareTag("Player"))
            RaisePlayerDamage(deadObject.CurrentHealthPercentage);            
    }

    public void StatusPortal(bool status) => RaiseStatusPortalChanged(status);
    public void PlayerFired(int gunIndex) => RaisePlayerFired(gunIndex);
    public void MonsterFired() => RaiseMonsterFired();

    public void MonsterCreated(MonsterController monster)
    {
        monsters.Add(monster);
        RaiseMonsterCreated(monster);
    }

    public void MissileCreated(MissileController missile) => missiles.Add(missile);
    
    public void BossMonsterCreated(BossMonsterController bossMonster) => this.bossMonster = bossMonster;
    
    public void MonsterAttacking(BaseMonsterController monster) => RaiseMonsterAttacking(monster);
    
    public void InitIncrementScore()
    {
        if (CurrentState != GameState.Win) return;
        
        RaiseScoreIncrementing();
    }

    public void EndIncrementScore()
    {
        if (CurrentState != GameState.Win) return;

        isScoreIncrementEnded = true;
        RaiseScoreIncremented();        
        ValidateNextLevel();
    }

    public void EndWinLevelMusic()
    {
        if (CurrentState != GameState.Win) return;

        isWinLevelMusicEnded = true;        
        ValidateNextLevel();
    }

    void ValidateNextLevel()
    {
        if (isWinLevelMusicEnded && isScoreIncrementEnded)
            CurrentState = GameState.NextLevel;
    }

    public void Close() => CurrentState = GameState.Exit;
}
