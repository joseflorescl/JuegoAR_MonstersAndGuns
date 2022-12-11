using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private const string LEVEL_TEXT = "Level";

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
    [SerializeField] private TMP_Text nextLevelText;


    [Space(10)]
    [Header("Settings")]
    [SerializeField] private Sprite[] splatSprites;
    [SerializeField] private float secondsToDeactivateGOMessage = 1;
    [SerializeField] private float timeToFadeBackground = 2f;
    [SerializeField] private float timeToFadeSplat = 1f;
    [SerializeField] private int splatImageRandomOffset = 200;
    [SerializeField] private int showWarningBossBattleCount = 4;
    [SerializeField] private float blinkingDelayWarningBossBattle = 0.5f;    
    [SerializeField] private float durationScoreIncrement = 5f;
    [SerializeField] private float delayNextLevelPanel = 0.2f;
    [SerializeField] private float minAlphaBackground = 0.5f;


    GameObject[] messagesPanelCenter;
    int score;
    int scorePreviousLevel;
    Coroutine uiRoutine;

    private void Awake()
    {
        messagesPanelCenter = new GameObject[] { backgroundPanel, mainPanel, portalCreationPanel, HUDPanel, battlePanel, 
            gameOverPanel, vfxPanel, warningBossBattlePanel, winLevelPanel, nextLevelPanel };
        HideAllMessages();
    }


    public void Close() // Llamada por Botón
    {
        GameManager.Instance.Close();
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
        GameManager.Instance.OnRestart += RestartHandler;

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
        GameManager.Instance.OnRestart -= RestartHandler;
    }

    private void RestartHandler()
    {
        scorePreviousLevel = 0;
    }

    private void NextLevelHandler(int nextLevel)
    {
        uiRoutine = StartCoroutine(NextLevelHandlerRoutine(nextLevel));
    }

    IEnumerator NextLevelHandlerRoutine(int nextLevel)
    {        
        scorePreviousLevel = score;
        winLevelPanel.SetActive(false);
        yield return new WaitForSeconds(delayNextLevelPanel);
        nextLevelPanel.SetActive(true);
        nextLevelText.text = LEVEL_TEXT + " " + nextLevel.ToString();
        levelText.text = nextLevel.ToString();
    }

    private void WinLevelHandler()
    {
        uiRoutine = StartCoroutine(WinLevelHandlerRoutine());
    }

    IEnumerator WinLevelHandlerRoutine()
    {
        bossMonsterHealth.SetActive(false);
        winLevelPanel.SetActive(true);
        battlePanel.SetActive(false);
        
        GameManager.Instance.InitIncrementScore();

        yield return StartCoroutine(TweenRoutine(scorePreviousLevel, score, durationScoreIncrement,
            tween: (value) => scoreTextWinLevel.text = Mathf.CeilToInt(value).ToString(), 
            postTween: () => scoreTextWinLevel.text = score.ToString()));

        GameManager.Instance.EndIncrementScore();
    }

    IEnumerator TweenRoutine(float startValue, float targetValue, float duration, System.Action<float> tween, System.Action postTween)
    {
        // Notar que esta función es genérica para hacer cualquier tipo de tween.
        float timeElapsed = 0;        
        while (timeElapsed < duration)
        {
            float value = Mathf.Lerp(startValue, targetValue, timeElapsed / duration);
            tween(value);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        postTween();
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
        uiRoutine = StartCoroutine(BossBattleHandlerRoutine());
    }

    IEnumerator BossBattleHandlerRoutine()
    {                
        yield return new WaitForSeconds(blinkingDelayWarningBossBattle); // Se espera un poquito para esperar la explosión del último monstruo

        backgroundPanel.SetActive(true);
        FadeGraphic(backgroundImage, 1f, minAlphaBackground, timeToFadeBackground);

        bool state = true;
        for (int i = 0; i < showWarningBossBattleCount*2 - 1; i++)
        {
            warningBossBattlePanel.SetActive(state);
            yield return new WaitForSeconds(blinkingDelayWarningBossBattle);
            state = !state;
        }
        warningBossBattlePanel.SetActive(false);
        backgroundPanel.SetActive(false);

    }


    private void ScoreUpdatedHandler(int score)
    {
        this.score = score;
    }

    private void GameOverHandler(float delay)
    {
        StopCoroutine(uiRoutine);
        uiRoutine = StartCoroutine(GameOverHandlerRoutine(delay));
    }

    IEnumerator GameOverHandlerRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideAllMessages();
        backgroundPanel.SetActive(true);
        backgroundImage.canvasRenderer.SetAlpha(minAlphaBackground);
        HUDPanel.SetActive(true);
        gameOverPanel.SetActive(true);
        scoreTextGameOver.text = score.ToString();
    }

    private void PlayerDeadHandler()
    {
        battlePanel.SetActive(false);
        backgroundPanel.SetActive(true);
        playerHealthBarImage.fillAmount = 0;
        ShowSplatBlood();
        FadeGraphic(backgroundImage, 1f, minAlphaBackground, timeToFadeBackground);
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
        splatImage.transform.position = new Vector3(x, y, 0); // el ancla está en el centro

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
        uiRoutine = StartCoroutine(BattlingRoutine(level));
    }

    IEnumerator BattlingRoutine(int level)
    {
        HideAllMessages();
        HUDPanel.SetActive(true);
        bossMonsterHealth.SetActive(false);
        battlePanel.SetActive(true);
        vfxPanel.SetActive(true);
        levelText.text = level.ToString();
        playerHealthBarImage.fillAmount = 1f; // Por ahora se asume simplemente que cuando parte un nuevo level la salud está a full
        splatImage.canvasRenderer.SetAlpha(0f);
        goMessage.SetActive(true);
        yield return new WaitForSeconds(secondsToDeactivateGOMessage); // Después de un ratito desactivar el texto de GO!
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
