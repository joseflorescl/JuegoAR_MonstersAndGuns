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
    
    private void Start()
    {
        float targetAlpha = 0f;
        backgroundImage.CrossFadeAlpha(targetAlpha, timeToFade, true);
    }


    public void Close()
    {
        GameManager.Instance.CloseApp();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }



    private void OnEnable()
    {
        GameManager.Instance.OnMainMenu += OnMainMenuHandler;
        GameManager.Instance.OnPortalCreation += PortalCreationHandler;
    }

    private void OnMainMenuHandler()
    {
        mainPanel.SetActive(true);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMainMenu -= OnMainMenuHandler;
        GameManager.Instance.OnPortalCreation -= PortalCreationHandler;
    }


    private void PortalCreationHandler()
    {
        // Se desactiva el Main Menu
        mainPanel.SetActive(false);
    }


}
