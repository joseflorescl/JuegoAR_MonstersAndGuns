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
    
    // TODO: crear m�todo que haga el init del men�, y que se subscriba a evento de GameManager

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
        GameManager.Instance.OnPortalCreation += PortalCreationHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPortalCreation -= PortalCreationHandler;
    }


    private void PortalCreationHandler()
    {
        // Se desactiva el Main Menu
        mainPanel.SetActive(false);
        // Se activa el panel de creaci�n de portal
        portalCreationPanel.SetActive(true);
    }


}
