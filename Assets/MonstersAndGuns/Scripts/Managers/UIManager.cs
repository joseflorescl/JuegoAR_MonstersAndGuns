using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    }

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenuActivating -= MainMenuHandler;
        GameManager.Instance.OnPortalCreating -= PortalCreatingHandler;
        GameManager.Instance.OnStatusPortalChanged -= StatusPortalHandler;
        GameManager.Instance.OnPortalCreated -= PortalCreatedHandler;
        GameManager.Instance.OnBattling -= BattlingHandler;
    }

    private void BattlingHandler(List<MonsterController> monsters, int level)
    {
        StartCoroutine(BattlingRoutine());
        
    }

    IEnumerator BattlingRoutine()
    {
        battlePanel.SetActive(true);
        // Después de un ratito desactivar el texto de GO!
        yield return new WaitForSeconds(secondsToDeactivateGOMessage);
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
        mainPanel.SetActive(true);
        FadeBackground();
        portalCreationPanel.SetActive(false);
    }

    private void PortalCreatingHandler()
    {
        mainPanel.SetActive(false);
        portalCreationPanel.SetActive(true);
    }

    void FadeBackground()
    {
        float targetAlpha = 0f;
        backgroundImage.CrossFadeAlpha(targetAlpha, timeToFade, true);
    }

    //TODO: meter los paneles ppales en un array de tal forma que sea más simple desactivarlos todos de una (como en juego Planet Force)
}
