using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float timeToFade = 2f;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject portalCreationPanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject pointAtFloorMessage;
    [SerializeField] private GameObject tapToPlacePortalMessage;
    [SerializeField] private float secondsToDeactivateGOMessage = 1;
    [SerializeField] private GameObject goMessage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image healthBarImage;


    GameObject[] messagesPanelCenter;

    private void Awake()
    {
        messagesPanelCenter = new GameObject[] { mainPanel, portalCreationPanel, battlePanel };
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

    private void OnEnable()
    {
        GameManager.Instance.OnMainMenuActivating += MainMenuHandler;
        GameManager.Instance.OnPortalCreating += PortalCreatingHandler;
        GameManager.Instance.OnStatusPortalChanged += StatusPortalHandler;
        GameManager.Instance.OnPortalCreated += PortalCreatedHandler;
        GameManager.Instance.OnBattling += BattlingHandler;
        GameManager.Instance.OnPlayerDamage += PlayerDamageHandler;
        GameManager.Instance.OnPlayerDead += PlayerDeadHandler;


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

    }

    private void PlayerDeadHandler()
    {
        healthBarImage.fillAmount = 0;
    }

    private void PlayerDamageHandler(float currentHealthPercentage)
    {
        healthBarImage.fillAmount = currentHealthPercentage;
    }

    private void BattlingHandler(List<MonsterController> monsters, int level)
    {
        StartCoroutine(BattlingRoutine(level));
        
    }

    IEnumerator BattlingRoutine(int level)
    {
        HideAllMessages();
        battlePanel.SetActive(true);
        levelText.text = level.ToString();
        healthBarImage.fillAmount = 1f; // Por ahora se asume simplemente que cuando parte un nuevo level la salud está a full
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
        mainPanel.SetActive(true);
        FadeBackground();
    }

    private void PortalCreatingHandler()
    {
        HideAllMessages();
        portalCreationPanel.SetActive(true);
    }

    void FadeBackground()
    {
        float targetAlpha = 0f;
        backgroundImage.CrossFadeAlpha(targetAlpha, timeToFade, true);
    }

    void HideAllMessages()
    {
        for (int i = 0; i < messagesPanelCenter.Length; i++)
        {
            messagesPanelCenter[i].SetActive(false);
        }
    }

}
