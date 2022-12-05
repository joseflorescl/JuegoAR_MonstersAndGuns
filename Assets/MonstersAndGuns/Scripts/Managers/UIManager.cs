using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Game Panels")]
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject portalCreationPanel;
    [SerializeField] private GameObject HUDPanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject vfxPanel;
    [SerializeField] private GameObject warningBossBattlePanel;
    [SerializeField] private GameObject winLevelPanel;
    [SerializeField] private GameObject nextLevelPanel;

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;    
    [SerializeField] private GameObject pointAtFloorMessage;
    [SerializeField] private GameObject tapToPlacePortalMessage;
    [SerializeField] private GameObject goMessage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image playerHealthBarImage;
    [SerializeField] private Image splatImage;
    [SerializeField] private TMP_Text scoreTextGameOver;
    [SerializeField] private GameObject bossMonsterHealth;
    [SerializeField] private Image bossMonsterHealthBarImage;
    [SerializeField] private TMP_Text scoreTextWinLevel;


    [Space(10)]
    [Header("Settings")]
    [SerializeField] private Sprite[] splatSprites;
    [SerializeField] private float secondsToDeactivateGOMessage = 1;
    [SerializeField] private float timeToFadeBackground = 2f;
    [SerializeField] private float timeToFadeSplat = 1f;
    [SerializeField] private int splatImageRandomOffset = 200;
    [SerializeField] private int showWarningBossBattleCount = 4;
    [SerializeField] private float blinkingDelayWarningBossBattle = 0.5f;
    [SerializeField] private float delayScoreIncrement = 0.1f;
    [SerializeField] private float maxTimeScoreIncrement = 5f;


    GameObject[] messagesPanelCenter;
    int score;

    private void Awake()
    {
        messagesPanelCenter = new GameObject[] { backgroundPanel, mainPanel, portalCreationPanel, HUDPanel, battlePanel, 
            gameOverPanel, vfxPanel, warningBossBattlePanel, winLevelPanel, nextLevelPanel };
        HideAllMessages();
    }


    public void Close() // Llamada por Bot�n
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void StartGame()
    {
        GameManager.Instance.GameStarted();
    }

    public void RestartGame()
    {
        GameManager.Instance.GameRestarted();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreatingHandler;
        GameManager.Instance.OnStatusPortalChanged += StatusPortalHandler;
        GameManager.Instance.OnPortalCreated += PortalCreatedHandler;
        GameManager.Instance.OnBattling += BattlingHandler;
        GameManager.Instance.OnPlayerDamage += PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;
        GameManager.Instance.OnGameOver += GameOverHandler;
        GameManager.Instance.OnScoreUpdated += ScoreUpdatedHandler;
        GameManager.Instance.OnBossBattle += BossBattleHandler;
        GameManager.Instance.OnBossMonsterSpawned += BossMonsterSpawnedHandler;
        GameManager.Instance.OnBossMonsterDamage += BossMonsterDamageHandler;
        GameManager.Instance.OnBossMonsterDead += BossMonsterDeadHandler;
        GameManager.Instance.OnWinLevel += WinLevelHandler;
        GameManager.Instance.OnNextLevel += NextLevelHandler;

    }

  

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreatingHandler;
        GameManager.Instance.OnStatusPortalChanged -= StatusPortalHandler;
        GameManager.Instance.OnPortalCreated -= PortalCreatedHandler;
        GameManager.Instance.OnBattling -= BattlingHandler;
        GameManager.Instance.OnPlayerDamage -= PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead -= PlayerDeadHandler;
        GameManager.Instance.OnGameOver -= GameOverHandler;
        GameManager.Instance.OnScoreUpdated -= ScoreUpdatedHandler;
        GameManager.Instance.OnBossBattle -= BossBattleHandler;
        GameManager.Instance.OnBossMonsterSpawned -= BossMonsterSpawnedHandler;
        GameManager.Instance.OnBossMonsterDamage -= BossMonsterDamageHandler;
        GameManager.Instance.OnBossMonsterDead -= BossMonsterDeadHandler;
        GameManager.Instance.OnWinLevel -= WinLevelHandler;
        GameManager.Instance.OnNextLevel -= NextLevelHandler;
    }

    private void NextLevelHandler()
    {
        //TODO:
        //Mostrar el panel de Next Level y guardar el score del nivel anterior para usarlo cuando se haga el incremento del score
        print("UI Next Level");
    }

    private void WinLevelHandler()
    {
        StartCoroutine(WinLevelHandlerRoutine());
    }

    IEnumerator WinLevelHandlerRoutine()
    {
        bossMonsterHealth.SetActive(false);
        winLevelPanel.SetActive(true);
        battlePanel.SetActive(false);
        
        int currentScore = 0; //TODO: este es el score del level anterior, por ahora un 0
        int deltaScore = score - currentScore;
        int incrementScore = Mathf.CeilToInt(deltaScore * delayScoreIncrement / maxTimeScoreIncrement);
        
        GameManager.Instance.InitIncrementScore();
        while(currentScore < score)
        {
            scoreTextWinLevel.text = currentScore.ToString();
            currentScore += incrementScore;            
            yield return new WaitForSeconds(delayScoreIncrement);
        }
        scoreTextWinLevel.text = score.ToString();
        GameManager.Instance.EndIncrementScore();
    }

    private void BossMonsterDeadHandler(BaseMonsterController obj)
    {
        bossMonsterHealthBarImage.fillAmount = 0f;
    }

    private void BossMonsterDamageHandler(BaseMonsterController bossMonster)
    {
        bossMonsterHealthBarImage.fillAmount = bossMonster.CurrentHealthPercentage;
    }

    private void BossMonsterSpawnedHandler()
    {
        bossMonsterHealth.SetActive(true);
        bossMonsterHealthBarImage.fillAmount = 1f;
    }


    private void BossBattleHandler()
    {
        StartCoroutine(BossBattleHandlerRoutine());
    }

    IEnumerator BossBattleHandlerRoutine()
    {                
        yield return new WaitForSeconds(blinkingDelayWarningBossBattle); // Se espera un poquito para esperar la explosi�n del �ltimo monstruo

        backgroundPanel.SetActive(true);
        FadeGraphic(backgroundImage, 1f, 0.5f, timeToFadeBackground);

        bool state = true;
        for (int i = 0; i < showWarningBossBattleCount*2; i++)
        {
            warningBossBattlePanel.SetActive(state);
            yield return new WaitForSeconds(blinkingDelayWarningBossBattle);
            state = !state;
        }

        backgroundPanel.SetActive(false);

    }


    private void ScoreUpdatedHandler(int score)
    {
        this.score = score;
    }

    private void GameOverHandler()
    {                
        gameOverPanel.SetActive(true);
        scoreTextGameOver.text = score.ToString();
    }

    private void PlayerDeadHandler()
    {
        battlePanel.SetActive(false);
        backgroundPanel.SetActive(true);
        playerHealthBarImage.fillAmount = 0;
        ShowSplatBlood();
        FadeGraphic(backgroundImage, 1f, 0.5f, timeToFadeBackground);
    }

    private void PlayerDamageHandler(float currentHealthPercentage)
    {
        playerHealthBarImage.fillAmount = currentHealthPercentage;
        ShowSplatBlood();
    }

    void ShowSplatBlood()
    {
        int x = Random.Range(splatImageRandomOffset, Screen.width - splatImageRandomOffset);
        int y = Random.Range(splatImageRandomOffset, Screen.height - splatImageRandomOffset);
        splatImage.transform.position = new Vector3(x, y, 0); // el ancla est� en el centro

        int idx = Random.Range(0, splatSprites.Length);
        splatImage.sprite = splatSprites[idx];

        FadeGraphic(splatImage, 1f, 0f, timeToFadeSplat);       

    }

    void FadeGraphic(Graphic graphic, float fromAlpha, float toAlpha, float duration)
    {
        graphic.canvasRenderer.SetAlpha(fromAlpha);
        graphic.CrossFadeAlpha(toAlpha, duration, true);
    }

    private void BattlingHandler(List<MonsterController> monsters, int level)
    {
        StartCoroutine(BattlingRoutine(level));
    }

    IEnumerator BattlingRoutine(int level)
    {
        HideAllMessages();
        HUDPanel.SetActive(true);
        bossMonsterHealth.SetActive(false);
        battlePanel.SetActive(true);
        vfxPanel.SetActive(true);
        levelText.text = level.ToString();
        playerHealthBarImage.fillAmount = 1f; // Por ahora se asume simplemente que cuando parte un nuevo level la salud est� a full
        splatImage.canvasRenderer.SetAlpha(0f);
        goMessage.SetActive(true);
        yield return new WaitForSeconds(secondsToDeactivateGOMessage); // Despu�s de un ratito desactivar el texto de GO!
        goMessage.SetActive(false);
    }

    private void PortalCreatedHandler()
    {
        portalCreationPanel.SetActive(false);
    }

    private void StatusPortalHandler(bool status)
    {
        pointAtFloorMessage.SetActive(!status);
        tapToPlacePortalMessage.SetActive(status);
    }

    private void MainMenuHandler()
    {
        HideAllMessages();
        backgroundPanel.SetActive(true);
        mainPanel.SetActive(true);
        FadeGraphic(backgroundImage, 1f, 0f, timeToFadeBackground);
    }

    private void PortalCreatingHandler()
    {
        HideAllMessages();
        portalCreationPanel.SetActive(true);
    }   

    void HideAllMessages()
    {
        for (int i = 0; i < messagesPanelCenter.Length; i++)
        {
            messagesPanelCenter[i].SetActive(false);
        }
    }

}
