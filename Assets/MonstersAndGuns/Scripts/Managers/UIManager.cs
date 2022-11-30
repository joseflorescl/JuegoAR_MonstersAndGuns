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

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;    
    [SerializeField] private GameObject pointAtFloorMessage;
    [SerializeField] private GameObject tapToPlacePortalMessage;
    [SerializeField] private GameObject goMessage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image splatImage;
    [SerializeField] private TMP_Text scoreText;

    [Space(10)]
    [Header("Settings")]
    [SerializeField] private Sprite[] splatSprites;
    [SerializeField] private float secondsToDeactivateGOMessage = 1;
    [SerializeField] private float timeToFadeBackground = 2f;
    [SerializeField] private float timeToFadeSplat = 1f;
    [SerializeField] private int splatImageRandomOffset = 200;


    GameObject[] messagesPanelCenter;
    int score;

    private void Awake()
    {
        messagesPanelCenter = new GameObject[] { backgroundPanel, mainPanel, portalCreationPanel, HUDPanel, battlePanel, gameOverPanel, vfxPanel };
        HideAllMessages();
    }


    public void Close()
    {
        GameManager.Instance.CloseApp();
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

    }

    private void ScoreUpdatedHandler(int score)
    {
        this.score = score;
    }

    private void GameOverHandler()
    {                
        gameOverPanel.SetActive(true);
        scoreText.text = score.ToString();
    }

    private void PlayerDeadHandler()
    {
        battlePanel.SetActive(false);
        backgroundPanel.SetActive(true);
        healthBarImage.fillAmount = 0;
        ShowSplatBlood();
        FadeGraphic(backgroundImage, 1f, 0.5f, timeToFadeBackground);
    }

    private void PlayerDamageHandler(float currentHealthPercentage)
    {
        healthBarImage.fillAmount = currentHealthPercentage;
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
        StartCoroutine(BattlingRoutine(level));
    }

    IEnumerator BattlingRoutine(int level)
    {
        HideAllMessages();
        HUDPanel.SetActive(true);
        battlePanel.SetActive(true);
        vfxPanel.SetActive(true);
        levelText.text = level.ToString();
        healthBarImage.fillAmount = 1f; // Por ahora se asume simplemente que cuando parte un nuevo level la salud está a full
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
